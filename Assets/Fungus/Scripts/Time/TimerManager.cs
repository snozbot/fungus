using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeSpan = System.TimeSpan;

namespace Fungus.TimeSys
{
    public class TimerManager : MonoBehaviour
    {
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

        protected virtual void CreateTimerWithID(int id)
        {
            timers[id] = new Timer();
        }

        public virtual void SetModeOfTimerWithID(int id, TimerMode timerMode)
        {
            EnsureTimerExistsWithID(id);
            timers[id].TimerMode = timerMode;
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