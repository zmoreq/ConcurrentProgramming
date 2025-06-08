using System;
using System.ComponentModel;
using System.Threading;
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
        public float Radius { get; }

        public TestBall(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public void UpdatePosition(IVector velocity)
        {
            X += velocity.X;
            Y += velocity.Y;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

   

    [Fact]
    public void Ball_IsMovedAfterAdd()
    {
        var service = new BallMovementService(500, 500);
        var ball = new TestBall(50, 50, 10);

        service.AddBall(ball);
        Thread.Sleep(100);
        service.StopAll();

        Assert.NotEqual(50, ball.X);
        Assert.NotEqual(50, ball.Y);
    }

    [Fact]
    public void HandleCollision_ChangesSpeed()
    {
        var service = new BallMovementService(500, 500);

        var ball1 = new TestBall(100, 100, 10);
        var ball2 = new TestBall(108, 100, 10);

        var data1 = new BallData(ball1, 2, 0);
        var data2 = new BallData(ball2, -2, 0);

        var method = typeof(BallMovementService)
            .GetMethod("HandleCollision", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        method!.Invoke(service, new object[] { data1, data2 });

        Assert.True(data1.SpeedX < 2);
        Assert.True(data2.SpeedX > -2);
    }
}
