using System;
using System.Reactive;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    public interface IBallMovementService
    {
        void AddBall(IBall ball);
        IObservable<Unit> PositionChanged { get; }
    }
}