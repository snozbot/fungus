using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeSpan = System.TimeSpan;

namespace Fungus.TimeSys
{
    public class TimerManager : MonoBehaviour
    {
        protected virtual void Awake()
        {
            SetUpPlaytimeTimer();
        }

        protected virtual void SetUpPlaytimeTimer()
        {
            CreateTimerWithID(playtimeTimerID);
            timers[0].TimerMode = TimerMode.countup;
        }

        protected virtual void CreateTimerWithID(int id)
        {
            timers[id] = new Timer();
            timers[id].ID = id;
        }

        protected int playtimeTimerID = 0;

        public IReadOnlyDictionary<int, Timer> Timers
        {
            get { return timers; }
        }
        protected Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

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

        protected virtual void Update()
        {
            foreach (Timer timerEl in timers.Values)
            {
                timerEl.Update();
            }
        }

    }
}