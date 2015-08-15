﻿using System;

namespace AnimeXdcc.Core.SystemWrappers.Timer
{
    public interface ITimer : IDisposable
    {
        double Interval { get; }
        event EventHandler<TimeElapsedEventArgs> Elapsed;
        void Start();
        void Stop();
    }
}