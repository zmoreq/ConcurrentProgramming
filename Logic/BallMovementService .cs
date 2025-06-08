using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
        private readonly System.Timers.Timer _movementTimer;

        public IObservable<Unit> PositionChanged => _positionChanged.AsObservable();

        public BallMovementService(int width, int height)
        {
            _width = width;
            _height = height;

            _movementTimer = new System.Timers.Timer(16);
            _movementTimer.Elapsed += async (sender, e) => await OnTimedEvent();
            _movementTimer.AutoReset = true;
            _movementTimer.Start();
        }

        private async Task OnTimedEvent()
        {
            await Task.Run(() =>
            {
                long currentTime = Stopwatch.GetTimestamp();
                foreach (var ballData in _balls.Values)
                {
                    float deltaTime = (float)(currentTime - ballData.LastUpdateTime) / Stopwatch.Frequency;
                    UpdateBallPosition(ballData, deltaTime);
                    ballData.LastUpdateTime = currentTime;
                }

                lock (_collisionLock)
                {
                    var ballsList = _balls.Values.ToList();
                    for (int i = 0; i < ballsList.Count; i++)
                    {
                        for (int j = i + 1; j < ballsList.Count; j++)
                        {
                            if (CheckCollision(ballsList[i], ballsList[j]))
                            {
                                HandleCollision(ballsList[i], ballsList[j]);
                            }
                        }
                    }
                }
                _positionChanged.OnNext(Unit.Default);
            }, _cts.Token);
        }


        public void AddBall(IBall ball)
        {
            float speedX = (float)(_random.NextDouble() * 4 - 2);
            float speedY = (float)(_random.NextDouble() * 4 - 2);
            var ballData = new BallData(ball, speedX, speedY);
            _balls.TryAdd(ball, ballData);
        }

        private void UpdateBallPosition(BallData ballData, float deltaTime)
        {
            float newX = ballData.Ball.X + ballData.SpeedX * deltaTime * 60;
            float newY = ballData.Ball.Y + ballData.SpeedY * deltaTime * 60;

            // Update position
            ballData.Ball.X = newX;
            ballData.Ball.Y = newY;

            // Boundary checks
            if (ballData.Ball.X < 0)
            {
                ballData.Ball.X = 0;
                ballData.SpeedX = -ballData.SpeedX;
            }
            else if (ballData.Ball.X > _width - ballData.Ball.Radius)
            {
                ballData.Ball.X = _width - ballData.Ball.Radius;
                ballData.SpeedX = -ballData.SpeedX;
            }

            if (ballData.Ball.Y < 0)
            {
                ballData.Ball.Y = 0;
                ballData.SpeedY = -ballData.SpeedY;
            }
            else if (ballData.Ball.Y > _height - ballData.Ball.Radius)
            {
                ballData.Ball.Y = _height - ballData.Ball.Radius;
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

        private void HandleCollision(BallData a, BallData b)
        {
            float dx = b.Ball.X - a.Ball.X;
            float dy = b.Ball.Y - a.Ball.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            float minDistance = (a.Ball.Radius + b.Ball.Radius) / 2f;

            if (distance == 0f || distance >= minDistance)
                return;

            float nx = dx / distance;
            float ny = dy / distance;

            float dvx = b.SpeedX - a.SpeedX;
            float dvy = b.SpeedY - a.SpeedY;

            float dotProduct = dvx * nx + dvy * ny;

            if (dotProduct > 0)
                return;

            float impulse = 2 * dotProduct / 2f;

            a.SpeedX += impulse * nx;
            a.SpeedY += impulse * ny;
            b.SpeedX -= impulse * nx;
            b.SpeedY -= impulse * ny;

            float overlap = minDistance - distance;
            float correctionX = nx * overlap / 2;
            float correctionY = ny * overlap / 2;

            a.Ball.X -= correctionX;
            a.Ball.Y -= correctionY;
            b.Ball.X += correctionX;
            b.Ball.Y += correctionY;
        }

        public void StopAll()
        {
            _cts.Cancel();
            _movementTimer.Stop();
            _movementTimer.Dispose();
        }

        public void Dispose()
        {
            StopAll();
            _positionChanged.OnCompleted();
            _positionChanged.Dispose();
        }
    }
}

