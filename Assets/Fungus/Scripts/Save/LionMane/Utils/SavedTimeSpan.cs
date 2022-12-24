using UnityEngine;
using TimeSpan = System.TimeSpan;

namespace Fungus.LionManeSaveSys
{
    [System.Serializable]
    public class SavedTimeSpan
    {
        public virtual TimeSpan TimeSpan
        {
            get { return timeSpan; }
            set
            {
                timeSpan = value;
                UpdateTimeSpanString();
            }
        }
        protected TimeSpan timeSpan;

        protected virtual void UpdateTimeSpanString()
        {
            timeSpanString = timeSpan.ToString(invariant);
        }

        [SerializeField]
        protected string timeSpanString;

        protected static string invariant = "c";

        public virtual void OnDeserialize()
        {
            timeSpan = TimeSpan.Parse(timeSpanString);
        }
    }
}