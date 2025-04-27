using CommunityToolkit.Mvvm.Input;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ConcurrentProgramming.Presentation.ViewModels
{
    public class BallViewModel : INotifyPropertyChanged
    {
        

        private readonly IBallMovementService _movementService;
        public ObservableCollection<IBall> Balls { get; } = new();
        
        public BallViewModel(IBallMovementService movementService)
        {
            _movementService = movementService;
            
        }
        public ICommand AddBallCommand => new RelayCommand(AddBall);

        private void AddBall()
        {
            var random = new Random();
            var x = random.Next(0, 700);
            var y = random.Next(0, 400);
            var ball = new Ball(new Vector(x, y), 25);

            Balls.Add(ball);
            OnPropertyChanged(nameof(Balls));
            _movementService.AddBall(ball);
        }
        

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
