using System.Windows;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using ConcurrentProgramming.Presentation.ViewModels;
using ConcurrentProgramming.Presentation.Views;

namespace ConcurrentProgramming
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var movementService = new BallMovementService(800, 450);
            var viewModel = new BallViewModel(movementService);

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            mainWindow.Show();
        }
    }
}