using System.Windows;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using ConcurrentProgramming.Presentation;
using ConcurrentProgramming.Presentation.ViewModels;
using Vector = ConcurrentProgramming.Data.Vector;

namespace ConcurrentProgramming
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var ball = new Ball(new Vector(100, 100), 25);
            var movementService = new BallMovementService();
            var viewModel = new BallViewModel(ball, movementService);

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            mainWindow.Show();
        }
    }
}
