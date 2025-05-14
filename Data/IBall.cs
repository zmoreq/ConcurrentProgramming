namespace ConcurrentProgramming.Data
{
    public interface IBall
    {
        float X { get; set; } // Zmieniamy na get/set
        float Y { get; set; } // Zmieniamy na get/set
        float Radius { get; }
        void UpdatePosition(IVector velocity);
    }
}