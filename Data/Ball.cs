namespace ConcurrentProgramming.Data
{
    public class Ball : IBall
    {
        public IVector Position { get; private set; }
        public float Radius { get; private set; }

        public Ball(IVector startPosition, float radius)
        {
            Position = startPosition;
            Radius = radius;
        }

        public void UpdatePosition(IVector velocity)
        {
            Position = new Vector(Position.X + velocity.X, Position.Y + velocity.Y);
        }
    }
}
