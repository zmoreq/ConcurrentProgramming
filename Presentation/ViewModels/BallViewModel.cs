using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Windows.Input;

namespace ConcurrentProgramming.Presentation.ViewModels
{
    public class BallViewModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;
        private readonly IBallMovementService _movementService;

        public IVector BallPosition => _ball.Position;

        public BallViewModel(IBall ball, IBallMovementService movementService)
        {
            _ball = ball;
            _movementService = movementService;
        }

        public ICommand MoveBallCommand => new RelayCommand(MoveBall);

        private void MoveBall()
        {
            var velocity = new Vector(1, 0); // Przykładowa prędkość
            _movementService.MoveBall(_ball, velocity);
            OnPropertyChanged(nameof(BallPosition));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
