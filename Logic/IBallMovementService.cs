using ConcurrentProgramming.Data;
using System;
using System.Reactive;

namespace ConcurrentProgramming.Logic
{
    public interface IBallMovementService
    {
        void AddBall(IBall ball);
        IObservable<Unit> PositionChanged { get; }
        //void MoveBalls(); // Dodajemy metodę do interfejsu
    }
}