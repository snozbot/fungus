using UnityEngine;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

namespace Fungus
{
    public class PlaytimeTracker : MonoBehaviour
    {
        public virtual void StartTracking()
        {
            startDate = DateTime.Now;
        }

        protected DateTime startDate;

        public virtual TimeSpan PlaytimeRecorded
        {
            get { return DateTime.Now - startDate; }
        }
    }
}