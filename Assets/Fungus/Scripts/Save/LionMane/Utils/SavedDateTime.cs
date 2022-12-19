using UnityEngine;
using DateTime = System.DateTime;
using System.Globalization;

namespace Fungus.LionManeSaveSys
{
    /** Provides a more convenient way to save date times in SaveUnits **/
    [System.Serializable]
    public class SavedDateTime: System.IEquatable<SavedDateTime>
    {
        public virtual DateTime LastWritten
        {
            get { return lastWritten; }
            set
            {
                lastWritten = value;
                UpdateLastWrittenString();
            }
        }
        
        protected DateTime lastWritten;

        protected virtual void UpdateLastWrittenString()
        {
            lastWrittenString = lastWritten.ToString(roundTripFormat, Culture);
        }

        [SerializeField]
        protected string lastWrittenString;
        // ^Since we can't serialize DateTimes, we'll need this when the time comes to deserialize

        protected static string roundTripFormat = "O";

        protected static CultureInfo Culture = CultureInfo.CurrentUICulture;

        public virtual void OnDeserialize()
        {
            lastWritten = DateTime.Parse(lastWrittenString, Culture);
        }

        public virtual bool Equals(SavedDateTime other)
        {
            return this.lastWrittenString == other.lastWrittenString;
        }

    }
}