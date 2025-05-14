using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    public class BallData
    {
        public IBall Ball { get; }
        public float SpeedX { get; set; }
        public float SpeedY { get; set; }

        public BallData(IBall ball, float speedX, float speedY)
        {
            Ball = ball;
            SpeedX = speedX;
            SpeedY = speedY;
        }
    }
}