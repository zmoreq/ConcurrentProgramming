using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Presentation.Models
{
    public class BallModel
    {
        public IVector Position { get; set; }
        public float Radius { get; set; }

        public BallModel(IVector position, float radius)
        {
            Position = position;
            Radius = radius;
        }
    }
}
