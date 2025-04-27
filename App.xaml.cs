using System.Windows;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using ConcurrentProgramming.Presentation.Views;
using ConcurrentProgramming.Presentation.ViewModels;
using Vector = ConcurrentProgramming.Data.Vector;

namespace ConcurrentProgramming
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var movementService = new BallMovementService();
            var viewModel = new BallViewModel(movementService);

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            mainWindow.Show();
        }

    }
}
