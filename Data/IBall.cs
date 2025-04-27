namespace ConcurrentProgramming.Data
{
    public interface IBall
    {
        IVector Position { get; }
        float Radius { get; }
        void UpdatePosition(IVector velocity);
    }
}
