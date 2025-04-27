using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentProgramming.Tests.Logic
{
    [TestClass]
    public class BallMovementServiceTests
    {
        [TestMethod]
        public void AddBall_ShouldAddBallToTheService()
        {
            var ballMovementService = new BallMovementService();
            var ball = new Ball(new Vector(100, 100), 25);

            ballMovementService.AddBall(ball);

            Assert.AreEqual(1, ballMovementService.Balls.Count);
            Assert.AreSame(ball, ballMovementService.Balls[0].Ball);
        }

        [TestMethod]
        public void MoveBalls_ShouldMoveBallsCorrectly()
        {
            var ballMovementService = new BallMovementService();
            var ball = new Ball(new Vector(100, 100), 25);
            ballMovementService.AddBall(ball);

            ballMovementService.MoveBalls();

            Assert.IsTrue(ball.X != 100 || ball.Y != 100);
        }

        [TestMethod]
        public void MoveBalls_ShouldReflectSpeedAndBounds()
        {
            var ballMovementService = new BallMovementService();
            var ball = new Ball(new Vector(100, 100), 25);
            ballMovementService.AddBall(ball);

            var initialX = ball.X;
            var initialY = ball.Y;

            ballMovementService.MoveBalls();

            Assert.AreNotEqual(initialX, ball.X);
            Assert.AreNotEqual(initialY, ball.Y);
            Assert.IsTrue(ball.X >= 0 && ball.X <= 800 - ball.Radius);
            Assert.IsTrue(ball.Y >= 0 && ball.Y <= 450 - ball.Radius);
        }
    }
}
