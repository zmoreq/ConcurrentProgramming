using ConcurrentProgramming.Data;
using System.Diagnostics;

namespace ConcurrentProgramming.Logic
{
    public class BallData
    {
        public IBall Ball { get; }
        public float SpeedX { get; set; }
        public float SpeedY { get; set; }
        public long LastUpdateTime { get; set; }

        public BallData(IBall ball, float speedX, float speedY)
        {
            Ball = ball;
            SpeedX = speedX;
            SpeedY = speedY;
            LastUpdateTime = Stopwatch.GetTimestamp();
        }
    }
}