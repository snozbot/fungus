using UnityEngine;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

namespace Fungus.PlaytimeSys
{
    public class PlaytimeTracker : MonoBehaviour
    {
        // Note that LateUpdate cannot run if this component is disabled
        public virtual void LateUpdate()
        {
            UpdatePlaytimeRecorded();
        }

        protected virtual void UpdatePlaytimeRecorded()
        {
            if (this.IsTracking)
            {
                endDate = DateTime.Now;
                PlaytimeRecorded = endDate - startDate;
            }
        }

        public virtual bool IsTracking { get; set; } = false;

        public virtual TimeSpan PlaytimeRecorded { get; protected set; }
        protected DateTime endDate, startDate;

        public virtual void StartTracking()
        {
            bool shouldActuallyStart = this.CanTrack && !this.IsTracking;

            if (shouldActuallyStart)
            {
                startDate = endDate = DateTime.Now;
                this.IsTracking = true;
            }
        }

        protected virtual bool CanTrack
        {
            get { return this.isActiveAndEnabled; }
        }

        public virtual void StopTracking()
        {
            IsTracking = false;
        }

        protected virtual void OnDisable()
        {
            StopTracking();
        }

    }
}