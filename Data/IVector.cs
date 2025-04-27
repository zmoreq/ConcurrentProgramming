namespace ConcurrentProgramming.Data
{
    public interface IVector
    {
        float X { get; }
        float Y { get; }
        float Length { get; }
        IVector Normalize();
    }
}
