using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
    public async Task AddBall_ShouldEventuallyTriggerPositionChanged()
    {
        // Arrange
        var service = new BallMovementService(800, 450);
        var ball = new TestBall();
        bool eventTriggered = false;

        using var subscription = service.PositionChanged.Subscribe(_ => eventTriggered = true);

        // Act
        service.AddBall(ball);
        await Task.Delay(50); // Poczekaj aż zdąży się ruszyć

        // Assert
        Assert.True(eventTriggered);
    }

    [Fact]
    public async Task Ball_ShouldChangePositionAfterSomeTime()
    {
        // Arrange
        var service = new BallMovementService(800, 450);
        var ball = new TestBall { X = 100, Y = 100 };
        service.AddBall(ball);

        float initialX = ball.X;
        float initialY = ball.Y;

        // Act
        await Task.Delay(50); // Poczekaj na co najmniej jeden tick ruchu

        // Assert
        Assert.NotEqual(initialX, ball.X);
        Assert.NotEqual(initialY, ball.Y);
    }

    [Fact]
    public async Task MoveBall_ShouldTriggerPositionChanged()
    {
        // Arrange
        var service = new BallMovementService(800, 450);
        var ball = new TestBall();
        bool eventTriggered = false;

        using var subscription = service.PositionChanged.Subscribe(_ => eventTriggered = true);

        service.AddBall(ball);

        // Act
        await Task.Delay(50);

        // Assert
        Assert.True(eventTriggered);
    }
}
