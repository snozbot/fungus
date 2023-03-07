// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Globalization;
using UnityEngine;
using DateTime = System.DateTime;

namespace Fungus
{
    /// <summary>
    /// Handles displaying the last time a slot was saved to. The user can choose the format
    /// that the date gets displayed in.
    /// </summary>
    public class SaveDateView : SlotTextView
    {
        /// <summary>
        /// The format this view is supposed to display the date in. The valid ones are those
        /// officially recognized and documented by Microsoft.
        /// </summary>
        public virtual string Format
        {
            get { return format; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    NullOrEmptyFormatAlert();
                else
                    format = value;
            }
        }

        [SerializeField]
        [Tooltip("The format this will display the date in. The default is G.")]
        private string format = "G";

        // See here for examples: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings

        protected virtual void NullOrEmptyFormatAlert()
        {
            var errorMessage = "Cannot have a null or empty date format!";
            throw new System.ArgumentException(errorMessage);
        }

        protected override void UpdateText()
        {
            base.UpdateText();
            UpdateReadableDate();
            DisplayDate();
        }

        protected virtual void UpdateReadableDate()
        {
            if (this.HasValidDate)
            {
                date = SaveData.lastWritten;
                readableDate = date.ToString(Format, localCulture);
            }
            else
            {
                readableDate = "";
            }
        }

        protected virtual bool HasValidDate
        {
            get { return this.HasSaveData && SaveData.lastWritten != default(DateTime); }
        }

        public virtual DateTime Date { get { return date; } protected set { date = value; } }
        protected DateTime date;
        protected string readableDate = "";
        protected CultureInfo localCulture = CultureInfo.CurrentUICulture;

        protected virtual void DisplayDate()
        {
            textField.text = readableDate;
        }
    }
}
