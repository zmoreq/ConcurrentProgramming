using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    public class BallMovementService : IBallMovementService
    {
        public void MoveBall(IBall ball, IVector velocity)
        {
            ball.UpdatePosition(velocity);
        }
    }
}
