using UnityEngine;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

namespace Fungus.TimeSys
{
    /// <summary>
    /// Helps keep track of time.
    /// </summary>
    [System.Serializable]
    public class Timer
    {
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        [SerializeField]
        int id;

        public virtual TimerMode TimerMode
        {
            get { return timerMode; }
            set { timerMode = value; }
        }

        [SerializeField]
        TimerMode timerMode = TimerMode.countdown;

        public virtual TimerState TimerState
        {
            get { return timerState; }
            protected set { timerState = value; }
        }

        [SerializeField]
        TimerState timerState = TimerState.stopped;

        // Since TimeSpans can't be serialized by default, we have to maintain string versions
        // of these objects so that we can properly restore them right after deserializing them
        protected DateTime endDate = DateTime.Now;

        public virtual void Update()
        {
            bool thisIsRunning = timerState == TimerState.running;

            if (thisIsRunning)
            {
                endDate = DateTime.Now;
                UpdateTimeRecorded();
                lastUpdate = DateTime.Now;
            }
        }

        protected DateTime lastUpdate = DateTime.Now;

        protected virtual void UpdateTimeRecorded()
        {
            TimeSpan timeElapsedSinceLastUpdate = endDate - lastUpdate;

            if (timerMode == TimerMode.countdown)
            {
                TimeRecorded = TimeRecorded.Subtract(timeElapsedSinceLastUpdate);
                StopSelfAsNeeded();

            }
            else if (timerMode == TimerMode.countup)
            {
                TimeRecorded = TimeRecorded.Add(timeElapsedSinceLastUpdate);
            }
        }

        public TimeSpan TimeRecorded
        {
            get { return timeRecorded; }
            set
            {
                timeRecorded = value;
                UpdateTimeRecordedString();
            }
        }

        protected TimeSpan timeRecorded;

        protected virtual void UpdateTimeRecordedString()
        {
            timeRecordedString = timeRecorded.ToString();
            // ^Good thing we don't have to worry about any fancy formats
        }

        public virtual string TimeRecordedString
        {
            get { return timeRecordedString; }
        }
        [SerializeField]
        string timeRecordedString = "";

        protected virtual void StopSelfAsNeeded()
        {
            bool timeIsNegative = TimeRecorded.TotalMilliseconds < 0;
            bool shouldStopCountingDown = timeIsNegative;

            if (shouldStopCountingDown)
            {
                TimeRecorded = new TimeSpan();
                this.Stop();
            }
        }

        public virtual void Start()
        {
            if (ShouldStartOnRequest())
            {
                ResetDates();
                timerState = TimerState.running;
                OnAnyTimerStart(this);
            }
        }

        protected virtual bool ShouldStartOnRequest()
        {
            bool alreadyRunning = timerState == TimerState.running;
            if (alreadyRunning)
                return false;

            // We don't want a countdown timer to be able to start while stopped at zero, so...
            bool isCountdownTimer = this.timerMode == TimerMode.countdown;
            bool atCountdownEnd = isCountdownTimer && TimeRecordedIsZero();
            if (atCountdownEnd)
                return false;

            return true;
        }

        protected virtual bool TimeRecordedIsZero()
        {
            return TimeRecorded.Milliseconds == 0 && TimeRecorded.Seconds == 0 &&
                TimeRecorded.Minutes == 0 && TimeRecorded.Hours == 0 &&
                TimeRecorded.Days == 0;
        }

        protected virtual void ResetDates()
        {
            endDate = lastUpdate = DateTime.Now;
        }

        public static System.Action<Timer> OnAnyTimerStart = delegate { };
        public static System.Action<Timer> OnAnyTimerReset = delegate { };
        public static System.Action<Timer> OnAnyTimerStop = delegate { };

        public virtual void Reset()
        {
            ResetDates();

            if (timerMode == TimerMode.countdown)
                TimeRecorded = CountdownStartingTime;
            else if (timerMode == TimerMode.countup)
                TimeRecorded = new TimeSpan();

            OnAnyTimerReset(this);
        }

        public TimeSpan CountdownStartingTime
        {
            get { return countdownStartingTime; }
            set
            {
                countdownStartingTime = value;
                bool shouldSetRecordToCountdown = timerMode == TimerMode.countdown && this.timerState == TimerState.stopped;
                if (shouldSetRecordToCountdown)
                    TimeRecorded = countdownStartingTime;
                UpdateCountdownStartingTimeString();
            }
        }

        protected TimeSpan countdownStartingTime;
        // ^Specifically for countdown timers. We need to keep track of this to have those 
        // be reset properly when needed

        protected virtual void UpdateCountdownStartingTimeString()
        {
            countdownStartingTimeString = countdownStartingTime.ToString();
        }

        public virtual string CountdownStartingTimeString
        {
            get { return countdownStartingTimeString; }
        }
        [SerializeField]
        string countdownStartingTimeString = "";

        public virtual void Stop()
        {
            bool isAlreadyStopped = timerState == TimerState.stopped;
            if (isAlreadyStopped)
                return;

            timerState = TimerState.stopped;
            OnAnyTimerStop(this);
        }

    }

    public enum TimerMode
    {
        countdown,
        countup
    }

    public enum TimerState
    {
        running,
        stopped
    }
}