using System.Collections.Generic;
using UnityEngine;
using TimeSpan = System.TimeSpan;

namespace Fungus.TimeSys
{
    public class TimerManager : MonoBehaviour
    {
        protected virtual void Awake()
        {
            bool timerManagerAlreadyInScene = Instance != null;
            if (timerManagerAlreadyInScene)
            {
                Destroy(this.gameObject);
                return;
            }

            SetUpPlaytimeTimer();
        }

        public static TimerManager Instance { get; private set; }

        protected virtual void SetUpPlaytimeTimer()
        {
            CreateTimerWithID(playtimeTimerID);
            timers[0].TimerMode = TimerMode.Countup;
        }

        protected virtual void CreateTimerWithID(int id)
        {
            timers[id] = new Timer();
            timers[id].ID = id;
        }

        protected int playtimeTimerID = 0;

        /// <summary>
        /// The compiler might not tell you, but attempts to alter the state of the Timers 
        /// through this getter will fail.
        /// </summary>
        public IDictionary<int, Timer> Timers
        {
            get { return CreateCopyOfTimers(); }
        }

        protected IDictionary<int, Timer> timers = new Dictionary<int, Timer>();

        protected virtual IDictionary<int, Timer> CreateCopyOfTimers()
        {
            // Since we want to make sure that clients cannot mess with the state of the timers
            // without going through this manager first
            IDictionary<int, Timer> holderOfCopies = new Dictionary<int, Timer>();
            IDictionary<int, Timer> theOriginal = timers;

            foreach (int id in theOriginal.Keys)
            {
                Timer originalTimer = theOriginal[id];
                Timer copyOfTimer = Timer.Clone(originalTimer);

                holderOfCopies[id] = copyOfTimer;
            }

            return holderOfCopies;
        }

        public virtual void StartTimerWithID(int id)
        {
            EnsureTimerExistsWithID(id);
            timers[id].Start();
        }

        public virtual void EnsureTimerExistsWithID(int id)
        {
            bool doesNotExist = !timers.ContainsKey(id);

            if (doesNotExist)
                CreateTimerWithID(id);
        }

        public virtual void SetModeOfTimerWithID(int id, TimerMode timerMode)
        {
            bool changingPlaytimeTimerMode = id == playtimeTimerID;

            if (changingPlaytimeTimerMode)
            {
                LetUserKnowPlaytimeTimerModeIsStatic();
                return;
            }

            EnsureTimerExistsWithID(id);
            timers[id].TimerMode = timerMode;
        }

        protected virtual void LetUserKnowPlaytimeTimerModeIsStatic()
        {
            string message = "Cannot change the mode of the playtime timer. That always has to be Countup.";
            Debug.LogWarning(message);
        }

        public virtual void ResetTimerWithID(int id)
        {
            EnsureTimerExistsWithID(id);
            timers[id].Reset();
        }

        public virtual void StopTimerWithID(int id)
        {
            EnsureTimerExistsWithID(id);
            timers[id].Stop();
        }

        public virtual void SetCountdownStartingTimeOfTimerWithID(int id, ref TimeSpan countdownTime)
        {
            EnsureTimerExistsWithID(id);
            timers[id].CountdownStartingTime = countdownTime;
        }

        public virtual void EndCountdownOfTimerWithID(int id)
        {
            EnsureTimerExistsWithID(id);
            Timer timerToWorkWith = timers[id];

            bool notForCountdowns = timerToWorkWith.TimerMode != TimerMode.Countdown;
            if (notForCountdowns)
            {
                string messageFormat = "Cannot force countdown-ending of Timer with ID {0} since it's not a Countdown timer.";
                string message = string.Format(messageFormat, timerToWorkWith.ID);
                Debug.LogWarning(message);
                return;
            }

            bool itIsCountingDown = timerToWorkWith.TimerState == TimerState.Running;

            if (itIsCountingDown)
            {
                timerToWorkWith.EndCountdown();
            }
        }

        protected virtual void Update()
        {
            foreach (Timer timerEl in timers.Values)
            {
                timerEl.Update();
            }
        }

    }
}