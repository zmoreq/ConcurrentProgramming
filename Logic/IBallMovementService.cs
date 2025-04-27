using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    public interface IBallMovementService
    {
        void AddBall(IBall ball);
        void MoveBalls();
    }
}
