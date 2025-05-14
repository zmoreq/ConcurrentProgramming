using System;

namespace ConcurrentProgramming.Data
{
    public class Vector : IVector
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public float Length => (float)Math.Sqrt(X * X + Y * Y);

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public IVector Normalize()
        {
            var length = Length;
            return new Vector(X / length, Y / length);
        }
    }
}