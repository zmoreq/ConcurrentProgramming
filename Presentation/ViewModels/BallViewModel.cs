using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using System;
using System.Windows;
using System.Windows.Threading;

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
                .Subscribe(_ =>
                {
                    if (Application.Current != null && Application.Current.Dispatcher != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            OnPropertyChanged(nameof(Balls));
                        });
                    }
                    else
                    {
                        OnPropertyChanged(nameof(Balls));
                    }
                });
        }

        public ICommand AddBallCommand => new RelayCommand(AddBall);

        private void AddBall()
        {
            var random = new Random();
            float radius = 25;
            float x, y;
            bool positionFound = false;
            int maxAttempts = 100;

            for (int i = 0; i < maxAttempts; i++)
            {
                x = random.Next((int)radius, (int)(700 - radius));
                y = random.Next((int)radius, (int)(400 - radius));

                var tempBall = new Ball(new Data.Vector(x, y), radius);
                bool overlaps = false;
                foreach (var existingBall in Balls)
                {
                    float dx = tempBall.X + tempBall.Radius / 2 - (existingBall.X + existingBall.Radius / 2);
                    float dy = tempBall.Y + tempBall.Radius / 2 - (existingBall.Y + existingBall.Radius / 2);
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (distance < (tempBall.Radius / 2 + existingBall.Radius / 2))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    var ball = new Ball(new Data.Vector(x, y), radius);
                    Balls.Add(ball);
                    _movementService.AddBall(ball);
                    positionFound = true;
                    break;
                }
            }

            if (!positionFound)
            {
                Console.WriteLine("Could not add new ball: no suitable position found without overlap.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _movementService.StopAll();
            (_movementService as IDisposable)?.Dispose();
        }
    }
}