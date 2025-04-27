using ConcurrentProgramming.Data;
using System.Windows.Threading;

namespace ConcurrentProgramming.Logic
{
    public class BallMovementService : IBallMovementService
    {
        
        private readonly List<BallData> _balls = new();


        private readonly DispatcherTimer _timer;
        private readonly Random _random = new();
        public event Action PositionChanged;
        public IReadOnlyList<BallData> Balls => _balls.AsReadOnly();


        public BallMovementService()
        {
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };

            _timer.Tick += (sender, args) => MoveBalls();
            _timer.Start();
        }

        public void AddBall(IBall ball)
        {
            float speedX = (float)(_random.NextDouble() * 4 - 2); // [-2, 2]
            float speedY = (float)(_random.NextDouble() * 4 - 2); // [-2, 2]
            _balls.Add(new BallData(ball, speedX, speedY));
        }
        

        public void MoveBalls()
        {
            foreach (var ballData in _balls)
            {
                ballData.Ball.UpdatePosition(new Vector(ballData.SpeedX, ballData.SpeedY));

                if (ballData.Ball.X < 0 || ballData.Ball.X > 800 - ballData.Ball.Radius)
                    ballData.SpeedX = -ballData.SpeedX;

                if (ballData.Ball.Y < 0 || ballData.Ball.Y > 450 - ballData.Ball.Radius)
                    ballData.SpeedY = -ballData.SpeedY;

                PositionChanged?.Invoke();
            }

            
        }

    }
}
