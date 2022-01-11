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
        TimerMode timerMode = TimerMode.Countdown;

        public virtual TimerState TimerState
        {
            get { return timerState; }
            protected set { timerState = value; }
        }

        [SerializeField]
        TimerState timerState = TimerState.Stopped;

        // Since TimeSpans can't be serialized by default, we have to maintain string versions
        // of these objects so that we can properly restore them right after deserializing them
        protected DateTime endDate = DateTime.Now;

        public virtual void Update()
        {
            bool thisIsRunning = timerState == TimerState.Running;

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

            if (timerMode == TimerMode.Countdown)
            {
                TimeRecorded = TimeRecorded.Subtract(timeElapsedSinceLastUpdate);
                StopSelfAsNeeded();

            }
            else if (timerMode == TimerMode.Countup)
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
                EndCountdown();
            }
        }

        public virtual void EndCountdown()
        {
            bool thisIsForCountdowns = this.timerMode == TimerMode.Countdown;
            // ^No point having this func do anything when this timer isn't for countdowns, after all
        
            if (thisIsForCountdowns)
            {
                stoppingSelfDueToCountdown = true; // <- To make sure that the event just for normal timer-stops doesn't proc
                TimeRecorded = new TimeSpan(); // <- Might as well make the current time a perfect zero instead of a negative
                this.Stop();
                OnAnyTimerCountdownEnd(this);
                stoppingSelfDueToCountdown = false;
            }
        }

        protected bool stoppingSelfDueToCountdown = false;

        public virtual void Start()
        {
            if (ShouldStartOnRequest())
            {
                ResetDates();
                timerState = TimerState.Running;
                OnAnyTimerStart(this);
            }
        }

        protected virtual bool ShouldStartOnRequest()
        {
            bool alreadyRunning = timerState == TimerState.Running;
            if (alreadyRunning)
                return false;

            // We don't want a countdown timer to be able to start while stopped at zero, so...
            bool isCountdownTimer = this.timerMode == TimerMode.Countdown;
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
        public static System.Action<Timer> OnAnyTimerCountdownEnd = delegate { };

        public virtual void Reset()
        {
            ResetDates();

            if (timerMode == TimerMode.Countdown)
                TimeRecorded = CountdownStartingTime;
            else if (timerMode == TimerMode.Countup)
                TimeRecorded = new TimeSpan();

            OnAnyTimerReset(this);
        }

        public TimeSpan CountdownStartingTime
        {
            get { return countdownStartingTime; }
            set
            {
                countdownStartingTime = value;
                bool shouldSetRecordToCountdown = timerMode == TimerMode.Countdown && this.timerState == TimerState.Stopped;
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
            bool isAlreadyStopped = timerState == TimerState.Stopped;
            if (isAlreadyStopped)
                return;

            timerState = TimerState.Stopped;

            if (!stoppingSelfDueToCountdown)
                OnAnyTimerStop(this);
        }

        public static Timer Clone(Timer toClone)
        {
            Timer theClone = new Timer();
            theClone.countdownStartingTime = toClone.countdownStartingTime;
            theClone.countdownStartingTimeString = toClone.countdownStartingTimeString;
            theClone.id = toClone.id;
            theClone.lastUpdate = toClone.lastUpdate;
            theClone.timeRecorded = toClone.timeRecorded;
            theClone.timeRecordedString = toClone.timeRecordedString;
            theClone.timerMode = toClone.timerMode;
            theClone.timerState = toClone.timerState;

            return theClone;
        }

    }

    public enum TimerMode
    {
        Countdown,
        Countup
    }

    public enum TimerState
    {
        Running,
        Stopped
    }
}