using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
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
        private readonly CancellationTokenSource _cts = new();

        public IObservable<Unit> PositionChanged => _positionChanged.AsObservable();

        public BallMovementService(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void AddBall(IBall ball)
        {
            float speedX = (float)(_random.NextDouble() * 4 - 2);
            float speedY = (float)(_random.NextDouble() * 4 - 2);
            var ballData = new BallData(ball, speedX, speedY);
            _balls.TryAdd(ball, ballData);

            // Tworzymy osobny Task dla każdej piłki
            Task.Run(() => MoveBallLoop(ballData, _cts.Token));
        }

        private async Task MoveBallLoop(BallData ballData, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                MoveBall(ballData);
                CheckCollisions(ballData);
                _positionChanged.OnNext(Unit.Default);

                await Task.Delay(16, token); // 60 FPS
            }
        }

        private void MoveBall(BallData ballData)
        {
            ballData.Ball.UpdatePosition(new Vector(ballData.SpeedX, ballData.SpeedY));

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
        }

        

        private bool CheckCollision(BallData a, BallData b)
        {
            float dx = a.Ball.X + a.Ball.Radius / 2 - (b.Ball.X + b.Ball.Radius / 2);
            float dy = a.Ball.Y + a.Ball.Radius / 2 - (b.Ball.Y + b.Ball.Radius / 2);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            return distance <= (a.Ball.Radius / 2 + b.Ball.Radius / 2);
        }

        private void CheckCollisions(BallData currentBall)
        {
            foreach (var otherBallData in _balls.Values)
            {
                if (currentBall == otherBallData)
                    continue;

                // Zasada: tylko piłka z "mniejszym adresem" obsługuje kolizję
                if (currentBall.GetHashCode() < otherBallData.GetHashCode())
                {
                    if (CheckCollision(currentBall, otherBallData))
                    {
                        HandleCollision(currentBall, otherBallData);
                    }
                }
            }
        }

        private void HandleCollision(BallData a, BallData b)
        {
            lock (_collisionLock)
            {
                float dx = b.Ball.X - a.Ball.X;
                float dy = b.Ball.Y - a.Ball.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                float minDistance = (a.Ball.Radius + b.Ball.Radius) / 2f;

                if (distance == 0f || distance >= minDistance)
                    return;

                // Wektor jednostkowy kolizji
                float nx = dx / distance;
                float ny = dy / distance;

                // Prędkość względna
                float dvx = b.SpeedX - a.SpeedX;
                float dvy = b.SpeedY - a.SpeedY;

                // Rzut prędkości względnej na wektor normalny
                float dotProduct = dvx * nx + dvy * ny;

                // Jeśli oddalają się – nie kolidują
                if (dotProduct > 0)
                    return;

                // Odbicie – zakładamy masy równe
                float impulse = 2 * dotProduct / 2f; // 2 piłki

                // Zmiana prędkości wzdłuż normalnej
                a.SpeedX += impulse * nx;
                a.SpeedY += impulse * ny;
                b.SpeedX -= impulse * nx;
                b.SpeedY -= impulse * ny;

                // Odsunięcie – żeby nie nakładały się
                float overlap = minDistance - distance;
                float correctionX = nx * overlap / 2;
                float correctionY = ny * overlap / 2;

                a.Ball.UpdatePosition(new Vector(-correctionX, -correctionY));
                b.Ball.UpdatePosition(new Vector(correctionX, correctionY));
            }
        }



        public void StopAll()
        {
            _cts.Cancel();
        }
    }
}
