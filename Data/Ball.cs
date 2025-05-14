using System.ComponentModel;
using System.Threading;

namespace ConcurrentProgramming.Data
{
    public class Ball : IBall, INotifyPropertyChanged
    {
        private readonly object _lock = new object();
        private float _x;
        private float _y;
        public float Radius { get; }

        public float X
        {
            get { lock (_lock) return _x; }
            set // Dodajemy setter
            {
                lock (_lock)
                {
                    if (Math.Abs(_x - value) > float.Epsilon)
                    {
                        _x = value;
                        OnPropertyChanged(nameof(X));
                    }
                }
            }
        }

        public float Y
        {
            get { lock (_lock) return _y; }
            set // Dodajemy setter
            {
                lock (_lock)
                {
                    if (Math.Abs(_y - value) > float.Epsilon)
                    {
                        _y = value;
                        OnPropertyChanged(nameof(Y));
                    }
                }
            }
        }

        public Ball(IVector startPosition, float radius)
        {
            X = startPosition.X;
            Y = startPosition.Y;
            Radius = radius;
        }

        public void UpdatePosition(IVector velocity)
        {
            X += velocity.X;
            Y += velocity.Y;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}