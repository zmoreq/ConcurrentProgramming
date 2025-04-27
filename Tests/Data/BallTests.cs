using ConcurrentProgramming.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentProgramming.Tests.Data
{
    [TestClass]
    public class BallTests
    {
        [TestMethod]
        public void BallConstructor_ShouldInitializePositionAndRadius()
        {
            
            var initialPosition = new Vector(10f, 20f);
            var radius = 25f;

            var ball = new Ball(initialPosition, radius);

            Assert.AreEqual(10f, ball.X);
            Assert.AreEqual(20f, ball.Y);
            Assert.AreEqual(25f, ball.Radius);
        }

        [TestMethod]
        public void UpdatePosition_ShouldUpdatePositionCorrectly()
        {
            var initialPosition = new Vector(10f, 20f);
            var radius = 25f;
            var ball = new Ball(initialPosition, radius);
            var velocity = new Vector(5f, -3f);

            ball.UpdatePosition(velocity);

            Assert.AreEqual(15f, ball.X);
            Assert.AreEqual(17f, ball.Y);
        }

        [TestMethod]
        public void PropertyChanged_ShouldBeCalledWhenPositionChanges()
        {
            var initialPosition = new Vector(10f, 20f);
            var radius = 25f;
            var ball = new Ball(initialPosition, radius);

            bool propertyChangedCalled = false;
            ball.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Ball.X) || args.PropertyName == nameof(Ball.Y))
                {
                    propertyChangedCalled = true;
                }
            };

            ball.UpdatePosition(new Vector(5f, 0f));

            Assert.IsTrue(propertyChangedCalled);
        }
    }
}
