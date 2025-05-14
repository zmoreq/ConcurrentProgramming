using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;

namespace ConcurrentProgramming.Presentation.ViewModels
{
    public class BallViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IBallMovementService _movementService;
        private readonly IDisposable _subscription;
        public ObservableCollection<IBall> Balls { get; } = new();

        public BallViewModel(IBallMovementService movementService)
        {
            _movementService = movementService;
            _subscription = _movementService.PositionChanged
                .ObserveOn(SynchronizationContext.Current) // Alternatywne rozwiązanie
                .Subscribe(_ => Dispatcher.CurrentDispatcher.Invoke(() => OnPropertyChanged(nameof(Balls))));
        }

        public ICommand AddBallCommand => new RelayCommand(AddBall);

        private void AddBall()
        {
            var random = new Random();
            var x = random.Next(0, 700);
            var y = random.Next(0, 400);
            var ball = new Ball(new Vector(x, y), 25);

            Balls.Add(ball);
            _movementService.AddBall(ball);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}