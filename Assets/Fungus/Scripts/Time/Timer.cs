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

        // Since DateTimes and TimeSpans can't be serialized by default, we have to maintain string versions
        // of these objects so that we can properly restore them right after deserializing them
        protected DateTime StartDate
        {
            get { return startDate; }
            set 
            { 
                startDate = value;
                UpdateStartDateString();
            }
        }

        DateTime startDate = DateTime.Now;

        protected virtual void UpdateStartDateString()
        {
            // We need to have the date strings in the round trip format to prevent deserialization issues
            startDateString = startDate.ToString(roundTripFormat);
        }

        public virtual string StartDateString
        {
            get { return startDateString; }
        }

        protected string startDateString = "";
        protected static string roundTripFormat = "O";

        protected DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                UpdateEndDateString();
            }

        }

        DateTime endDate = DateTime.Now;

        protected virtual void UpdateEndDateString()
        {
            endDateString = endDate.ToString(roundTripFormat);
        }

        public virtual string EndDateString
        {
            get { return endDateString; }
        }
        [SerializeField]
        string endDateString = "";

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
            if (timerMode == TimerMode.countdown)
            {
                TimeSpan timeElapsedSinceLastUpdate = endDate - lastUpdate;
                TimeRecorded = TimeRecorded.Subtract(timeElapsedSinceLastUpdate);
            }
            else if (timerMode == TimerMode.countup)
            {
                TimeSpan timeElapsedSinceLastStart = endDate - startDate;
                TimeRecorded = timeElapsedSinceLastStart;
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

        public virtual void Start()
        {
            bool alreadyRunning = timerState == TimerState.running;
            if (alreadyRunning)
                return;

            Reset();
            timerState = TimerState.running;
        }

        public virtual void Reset()
        {
            startDate = endDate = lastUpdate = DateTime.Now;

            if (timerMode == TimerMode.countdown)
                TimeRecorded = CountdownStartingTime;
            else if (timerMode == TimerMode.countup)
                TimeRecorded = new TimeSpan();
        }

        public TimeSpan CountdownStartingTime
        {
            get { return countdownStartingTime; }
            set
            {
                countdownStartingTime = value;
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