using System.ComponentModel;
using ConcurrentProgramming.Data;
using Xunit;

namespace ConcurrentProgramming.Tests.Data;

public class BallTests
{
    [Fact]
    public void Ball_ShouldImplementINotifyPropertyChanged()
    {
        // Arrange
        var ball = new Ball(new Vector(0, 0), 10);

        // Act & Assert
        Xunit.Assert.IsAssignableFrom<INotifyPropertyChanged>(ball);
    }

    [Fact]
    public void UpdatePosition_ShouldChangeCoordinates()
    {
        // Arrange
        var ball = new Ball(new Vector(0, 0), 10);
        float initialX = ball.X;
        float initialY = ball.Y;

        // Act
        ball.UpdatePosition(new Vector(5, 3));

        // Assert
        Xunit.Assert.NotEqual(initialX, ball.X);
        Xunit.Assert.NotEqual(initialY, ball.Y);
        Xunit.Assert.Equal(5, ball.X);
        Xunit.Assert.Equal(3, ball.Y);
    }

    [Fact]
    public void PropertyChanged_ShouldBeRaisedOnPositionUpdate()
    {
        // Arrange
        var ball = new Ball(new Vector(0, 0), 10);
        bool xChanged = false;
        bool yChanged = false;

        ball.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(ball.X)) xChanged = true;
            if (args.PropertyName == nameof(ball.Y)) yChanged = true;
        };

        // Act
        ball.UpdatePosition(new Vector(1, 1));

        // Assert
        Xunit.Assert.True(xChanged);
        Xunit.Assert.True(yChanged);
    }
}