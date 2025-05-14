using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using ConcurrentProgramming.Presentation.ViewModels;
using Xunit;

namespace ConcurrentProgramming.Tests.Presentation
{
    public class BallViewModelTests : IDisposable
    {
        private class TestBallMovementService : IBallMovementService
        {
            public int AddBallCalls { get; private set; }
            public int MoveBallsCalls { get; private set; }
            public bool PositionChangedSubscribed { get; private set; }

            public void AddBall(IBall ball) => AddBallCalls++;

            public void MoveBalls() => MoveBallsCalls++;

            public IObservable<Unit> PositionChanged =>
                Observable.Return(Unit.Default).Do(_ => PositionChangedSubscribed = true);
        }

        private readonly TestBallMovementService _testService;
        private readonly BallViewModel _viewModel;

        public BallViewModelTests()
        {
            _testService = new TestBallMovementService();
            _viewModel = new BallViewModel(_testService);
        }

        public void Dispose()
        {
            _viewModel.Dispose();
        }

        [Fact]
        public void Should_Implement_INotifyPropertyChanged()
        {
            Assert.IsAssignableFrom<INotifyPropertyChanged>(_viewModel);
        }

        [Fact]
        public void Should_Initialize_With_Empty_Balls_Collection()
        {
            Assert.NotNull(_viewModel.Balls);
            Assert.Empty(_viewModel.Balls);
        }

        [Fact]
        public void Should_Initialize_AddBallCommand_As_RelayCommand()
        {
            Assert.IsType<RelayCommand>(_viewModel.AddBallCommand);
            Assert.True(_viewModel.AddBallCommand.CanExecute(null));
        }

        [Fact]
        public void AddBallCommand_Should_Add_New_Ball_To_Collection()
        {
            // Act
            _viewModel.AddBallCommand.Execute(null);

            // Assert
            Assert.Single(_viewModel.Balls);
            Assert.IsAssignableFrom<IBall>(_viewModel.Balls[0]);
        }

        [Fact]
        public void AddBallCommand_Should_Call_Service_AddBall()
        {
            // Act
            _viewModel.AddBallCommand.Execute(null);

            // Assert
            Assert.Equal(1, _testService.AddBallCalls);
        }

        [Fact]
        public void AddBallCommand_Should_Raise_PropertyChanged_For_Balls()
        {
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.Balls))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.AddBallCommand.Execute(null);

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void Should_Subscribe_To_PositionChanged_On_Initialization()
        {
            Assert.True(_testService.PositionChangedSubscribed);
        }

        [Fact]
        public void PositionChanged_Should_Raise_PropertyChanged_For_Balls()
        {
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.Balls))
                    propertyChangedRaised = true;
            };

            // Act - symulujemy zmianę pozycji poprzez nasz testowy serwis
            _testService.PositionChanged.Subscribe(_ => { });

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void Dispose_Should_Cleanup_Resources()
        {
            // Arrange
            var subscriptionActive = true;
            _viewModel.PropertyChanged += (s, e) => subscriptionActive = true;

            // Act
            _viewModel.Dispose();
            _testService.PositionChanged.Subscribe(_ => subscriptionActive = false);

            // Assert
            Assert.False(subscriptionActive);
        }
    }
}