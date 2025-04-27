using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using ConcurrentProgramming.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ConcurrentProgramming.Tests.Presentation
{
    [TestClass]
    public class BallViewModelTests
    {
        [TestMethod]
        public void AddBallCommand_ShouldAddBallToCollection()
        {
            var movementService = new BallMovementService();
            var viewModel = new BallViewModel(movementService);

            viewModel.AddBallCommand.Execute(null);

            Assert.AreEqual(1, viewModel.Balls.Count);
        }

        [TestMethod]
        public void AddBallCommand_ShouldNotifyPropertyChanged()
        {
            var movementService = new BallMovementService();
            var viewModel = new BallViewModel(movementService);
            bool propertyChangedCalled = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(BallViewModel.Balls))
                {
                    propertyChangedCalled = true;
                }
            };

            viewModel.AddBallCommand.Execute(null);

            Assert.IsTrue(propertyChangedCalled);
        }
    }
}
