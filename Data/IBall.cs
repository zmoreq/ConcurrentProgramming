namespace ConcurrentProgramming.Data
{
    public interface IBall
    {
        float X { get; }
        float Y { get; }
        float Radius { get; }
        void UpdatePosition(IVector velocity);
    }
}
