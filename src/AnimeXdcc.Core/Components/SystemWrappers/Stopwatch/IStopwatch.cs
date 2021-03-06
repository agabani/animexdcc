﻿namespace AnimeXdcc.Core.Components.SystemWrappers.Stopwatch
{
    public interface IStopwatch
    {
        long ElapsedMilliseconds { get; }
        void Start();
        void Stop();
        void Reset();
        void Restart();
    }
}