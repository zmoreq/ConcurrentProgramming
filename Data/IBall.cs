using System.ComponentModel;

namespace ConcurrentProgramming.Data
{
    public interface IBall : INotifyPropertyChanged
    {
        float X { get; set; }
        float Y { get; set; }
        float Radius { get; }
        void UpdatePosition(IVector velocity);
    }
}