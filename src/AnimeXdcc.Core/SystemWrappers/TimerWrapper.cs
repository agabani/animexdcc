using System;
using System.Timers;

namespace AnimeXdcc.Core.SystemWrappers
{
    public class TimerWrapper : ITimer
    {
        private readonly Timer _timer;

        public TimerWrapper(double interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += TimerOnElapsed;
        }

        public double Interval
        {
            get { return _timer.Interval; }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public event EventHandler<TimeElapsedEventArgs> Elapsed;

        protected virtual void OnElapsed(ElapsedEventArgs e)
        {
            var handler = Elapsed;
            if (handler != null) handler(this, new TimeElapsedEventArgs(e.SignalTime));
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            OnElapsed(elapsedEventArgs);
        }

        public void Dispose()
        {
            ((IDisposable) _timer).Dispose();
        }
    }
}