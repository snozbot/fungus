using UnityEngine;
using System.Globalization;
using DateTime = System.DateTime;

namespace Fungus
{
    /// <summary>
    /// Handles displaying the last time a slot was saved to. The user can choose the format 
    /// that the date gets displayed in.
    /// </summary>
    public class SaveDateView : SlotTextView
    {
        public virtual string StandardFormat
        {
            get { return standardFormat; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    NullOrEmptyFormatAlert();
                else
                    standardFormat = value;
            }
        }

        [SerializeField]
        [Tooltip("The standard format this will display the date in. The default is G.")]
        string standardFormat = "G";

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
                readableDate = date.ToString(StandardFormat, localCulture);
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

        protected DateTime date;
        protected string readableDate = "";
        protected CultureInfo localCulture = CultureInfo.CurrentUICulture;

        protected virtual void DisplayDate()
        {
            textField.text = readableDate;
        }

    }
}