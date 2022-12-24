using UnityEngine;
using DateTime = System.DateTime;
using System.Globalization;

namespace Fungus.LionManeSaveSys
{
    /** Provides a more convenient way to save date times in SaveUnits **/
    [System.Serializable]
    public class SavedDateTime: System.IEquatable<SavedDateTime>
    {
        public virtual DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                UpdateLastWrittenString();
            }
        }
        
        protected DateTime dateTime;

        protected virtual void UpdateLastWrittenString()
        {
            lastWrittenString = dateTime.ToString(roundTripFormat, Culture);
        }

        [SerializeField]
        protected string lastWrittenString;
        // ^Since we can't serialize DateTimes, we'll need this when the time comes to deserialize

        protected static string roundTripFormat = "O";

        protected static CultureInfo Culture = CultureInfo.CurrentUICulture;

        public virtual void OnDeserialize()
        {
            if (string.IsNullOrEmpty(lastWrittenString))
                // Since the client may not want all save units to have any registered lastWritten dates
                return;

            dateTime = DateTime.Parse(lastWrittenString, Culture);
        }

        public virtual bool Equals(SavedDateTime other)
        {
            return this.lastWrittenString == other.lastWrittenString;
        }

    }
}