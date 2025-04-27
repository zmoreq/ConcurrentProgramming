using System.ComponentModel;

namespace ConcurrentProgramming.Data
{
    public class Ball : IBall, INotifyPropertyChanged
    {
        private float _x;
        private float _y;
        public float Radius { get; private set; }

        public float X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        public float Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public Ball(IVector startPosition, float radius)
        {
            _x = startPosition.X;
            _y = startPosition.Y;
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
