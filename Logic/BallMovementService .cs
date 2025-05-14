using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    public class BallMovementService : IBallMovementService
    {
        private readonly ConcurrentDictionary<IBall, BallData> _balls = new();
        private readonly Subject<Unit> _positionChanged = new();
        private readonly Random _random = new();
        private readonly int _width;
        private readonly int _height;
        private readonly object _collisionLock = new object();

        public IObservable<Unit> PositionChanged => _positionChanged.AsObservable();

        public BallMovementService(int width, int height)
        {
            _width = width;
            _height = height;

            Observable.Interval(TimeSpan.FromMilliseconds(16))
                .Subscribe(_ => MoveBalls());
        }

        public void AddBall(IBall ball)
        {
            float speedX = (float)(_random.NextDouble() * 4 - 2);
            float speedY = (float)(_random.NextDouble() * 4 - 2);
            _balls.TryAdd(ball, new BallData(ball, speedX, speedY));
        }

        private void MoveBalls()
        {
            Parallel.ForEach(_balls.Values, ballData =>
            {
                ballData.Ball.UpdatePosition(new Vector(ballData.SpeedX, ballData.SpeedY));

                // Kolizje z granicami - teraz używamy UpdatePosition
                if (ballData.Ball.X <= 0)
                {
                    ballData.Ball.UpdatePosition(new Vector(-ballData.Ball.X, 0));
                    ballData.SpeedX = -ballData.SpeedX;
                }
                else if (ballData.Ball.X >= _width - ballData.Ball.Radius)
                {
                    ballData.Ball.UpdatePosition(new Vector(_width - ballData.Ball.Radius - ballData.Ball.X, 0));
                    ballData.SpeedX = -ballData.SpeedX;
                }

                if (ballData.Ball.Y <= 0)
                {
                    ballData.Ball.UpdatePosition(new Vector(0, -ballData.Ball.Y));
                    ballData.SpeedY = -ballData.SpeedY;
                }
                else if (ballData.Ball.Y >= _height - ballData.Ball.Radius)
                {
                    ballData.Ball.UpdatePosition(new Vector(0, _height - ballData.Ball.Radius - ballData.Ball.Y));
                    ballData.SpeedY = -ballData.SpeedY;
                }
            });

            // Następnie sprawdzaj kolizje między piłkami
            var ballList = _balls.Values.ToArray();
            for (int i = 0; i < ballList.Length; i++)
            {
                for (int j = i + 1; j < ballList.Length; j++)
                {
                    if (CheckCollision(ballList[i], ballList[j]))
                    {
                        HandleCollision(ballList[i], ballList[j]);
                    }
                }
            }

            _positionChanged.OnNext(Unit.Default);
        }

        private bool CheckCollision(BallData a, BallData b)
        {
            float dx = a.Ball.X + a.Ball.Radius / 2 - (b.Ball.X + b.Ball.Radius / 2);
            float dy = a.Ball.Y + a.Ball.Radius / 2 - (b.Ball.Y + b.Ball.Radius / 2);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            return distance <= (a.Ball.Radius / 2 + b.Ball.Radius / 2);
        }

        private void HandleCollision(BallData a, BallData b)
        {
            lock (_collisionLock)
            {
                // Oblicz wektor normalny kolizji
                float nx = (b.Ball.X - a.Ball.X) / (a.Ball.Radius + b.Ball.Radius);
                float ny = (b.Ball.Y - a.Ball.Y) / (a.Ball.Radius + b.Ball.Radius);

                // Oblicz względną prędkość
                float dvx = b.SpeedX - a.SpeedX;
                float dvy = b.SpeedY - a.SpeedY;

                // Oblicz iloczyn skalarny
                float p = dvx * nx + dvy * ny;

                if (p > 0) return; // Piłki oddalają się od siebie

                // Wymiana prędkości
                (a.SpeedX, b.SpeedX) = (b.SpeedX, a.SpeedX);
                (a.SpeedY, b.SpeedY) = (b.SpeedY, a.SpeedY);

                // Minimalne odsunięcie piłek - teraz używamy UpdatePosition zamiast bezpośredniego przypisania
                float overlap = (a.Ball.Radius + b.Ball.Radius) -
                              (float)Math.Sqrt(Math.Pow(b.Ball.X - a.Ball.X, 2) +
                                       Math.Pow(b.Ball.Y - a.Ball.Y, 2));

                if (overlap > 0)
                {
                    float moveX = overlap * nx * 0.5f;
                    float moveY = overlap * ny * 0.5f;

                    // Używamy UpdatePosition zamiast bezpośredniego przypisania
                    a.Ball.UpdatePosition(new Vector(-moveX, -moveY));
                    b.Ball.UpdatePosition(new Vector(moveX, moveY));
                }
            }
        }
    }
}