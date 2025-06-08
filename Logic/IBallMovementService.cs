using ConcurrentProgramming.Data;
using System;
using System.Reactive;

namespace ConcurrentProgramming.Logic
{
    public interface IBallMovementService : IDisposable
    {
        IObservable<Unit> PositionChanged { get; }
        void AddBall(IBall ball);
        void StopAll();
    }
}