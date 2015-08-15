using System;
using System.Timers;

namespace AnimeXdcc.Core.SystemWrappers.Timer
{
    public class TimerWrapper : ITimer
    {
        private System.Timers.Timer _timer;

        public TimerWrapper(double interval)
        {
            _timer = new System.Timers.Timer(interval);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TimerWrapper()
        {
            Dispose(false);
        }

        protected virtual void OnElapsed(ElapsedEventArgs e)
        {
            var handler = Elapsed;
            if (handler != null) handler(this, new TimeElapsedEventArgs(e.SignalTime));
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            OnElapsed(elapsedEventArgs);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
    }
}