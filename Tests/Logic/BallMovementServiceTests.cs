using System;
using System.Reactive.Linq;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using Xunit;

namespace ConcurrentProgramming.Tests.Logic;

public class BallMovementServiceTests
{
    private class TestBall : IBall
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; } = 10;
        public void UpdatePosition(IVector velocity)
        {
            X += velocity.X;
            Y += velocity.Y;
        }
    }

    [Fact]
    public void AddBall_ShouldTriggerPositionChanged()
    {
        // Arrange
        var service = new BallMovementService(800, 450);
        var ball = new TestBall();
        bool eventTriggered = false;
        var subscription = service.PositionChanged.Subscribe(_ => eventTriggered = true);

        // Act
        service.AddBall(ball);

        // Assert
        Assert.True(eventTriggered);
        subscription.Dispose();
    }

    [Fact]
    public void MoveBalls_ShouldChangePosition()
    {
        // Arrange
        var service = new BallMovementService(800, 450);
        var ball = new TestBall { X = 100, Y = 100 };
        service.AddBall(ball);
        float initialX = ball.X;
        float initialY = ball.Y;

        // Act
        service.MoveBalls(); // Teraz możemy wywołać tę metodę

        // Assert
        Assert.NotEqual(initialX, ball.X);
        Assert.NotEqual(initialY, ball.Y);
    }

    [Fact]
    public void MoveBalls_ShouldTriggerPositionChanged()
    {
        // Arrange
        var service = new BallMovementService(800, 450);
        var ball = new TestBall();
        service.AddBall(ball);
        bool eventTriggered = false;
        var subscription = service.PositionChanged.Subscribe(_ => eventTriggered = true);

        // Act
        service.MoveBalls();

        // Assert
        Assert.True(eventTriggered);
        subscription.Dispose();
    }
}