using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    public interface IBallMovementService
    {
        void MoveBall(IBall ball, IVector velocity);
    }
}
