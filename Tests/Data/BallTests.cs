using System.ComponentModel;
using ConcurrentProgramming.Data;
using Xunit;

namespace ConcurrentProgramming.Tests.Data;

public class BallTests
{
    [Fact]
    public void Ball_ShouldImplementINotifyPropertyChanged()
    {
        var ball = new Ball(new Vector(0, 0), 10);

        Xunit.Assert.IsAssignableFrom<INotifyPropertyChanged>(ball);
    }

    [Fact]
    public void UpdatePosition_ShouldChangeCoordinates()
    {
        var ball = new Ball(new Vector(0, 0), 10);
        float initialX = ball.X;
        float initialY = ball.Y;

        ball.UpdatePosition(new Vector(5, 3));

        Xunit.Assert.NotEqual(initialX, ball.X);
        Xunit.Assert.NotEqual(initialY, ball.Y);
        Xunit.Assert.Equal(5, ball.X);
        Xunit.Assert.Equal(3, ball.Y);
    }

    [Fact]
    public void PropertyChanged_ShouldBeRaisedOnPositionUpdate()
    {
        var ball = new Ball(new Vector(0, 0), 10);
        bool xChanged = false;
        bool yChanged = false;

        ball.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(ball.X)) xChanged = true;
            if (args.PropertyName == nameof(ball.Y)) yChanged = true;
        };

        ball.UpdatePosition(new Vector(1, 1));

        Xunit.Assert.True(xChanged);
        Xunit.Assert.True(yChanged);
    }
}