using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.Input;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using ConcurrentProgramming.Presentation.ViewModels;
using Xunit;

namespace ConcurrentProgramming.Tests.Presentation;

public class BallViewModelTests : IDisposable
{
    private class TestBallMovementService : IBallMovementService, IDisposable
    {
        public int AddBallCalls { get; private set; }
        public bool PositionChangedSubscribed { get; private set; }

        public void AddBall(IBall ball) => AddBallCalls++;

        public void MoveBalls() { }

        public void StopAll() { }

        public void Dispose() { }

        public IObservable<Unit> PositionChanged =>
            Observable.Return(Unit.Default)
                      .Do(_ => PositionChangedSubscribed = true);
    }


    private readonly TestBallMovementService _testService;
    private readonly BallViewModel _viewModel;

    public BallViewModelTests()
    {
        _testService = new TestBallMovementService();
        _viewModel = new BallViewModel(_testService);
    }

    public void Dispose() => _viewModel.Dispose();

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
    public void AddBallCommand_Should_Add_New_Ball()
    {
        _viewModel.AddBallCommand.Execute(null);
        Assert.Single(_viewModel.Balls);
    }

    [Fact]
    public void AddBallCommand_Should_Trigger_Service()
    {
        _viewModel.AddBallCommand.Execute(null);
        Assert.Equal(1, _testService.AddBallCalls);
    }

    [Fact]
    public void AddBallCommand_Should_Have_RelayCommand()
    {
        Assert.IsType<RelayCommand>(_viewModel.AddBallCommand);
        Assert.True(_viewModel.AddBallCommand.CanExecute(null));
    }

    [Fact]
    public void Should_Subscribe_To_PositionChanged()
    {
        Assert.True(_testService.PositionChangedSubscribed);
    }

    [Fact]
    public void Dispose_Should_Release_Resources()
    {
        _viewModel.Dispose();
    }
}
