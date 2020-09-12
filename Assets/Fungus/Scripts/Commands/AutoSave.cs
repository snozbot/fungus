// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// AutoSave, requests a new AutoSave from the save manager, this is still governed by the limits in place in 
    /// the Save Manager.
    /// </summary>
    [CommandInfo("Save",
                 "Auto Save",
                 "Creates an Auto Save. Player can then load or continue with them via the SaveMenu/SaveController")]
    public class AutoSave : ProgressMarker
    {
        /// <summary>
        /// Supported modes for specifying a Save Point Description.
        /// </summary>
        public enum DescriptionMode
        {
            /// <summary> Uses the current date and time as the save point description.</summary>
            Timestamp,

            /// <summary> Use a custom string for the save point description.</summary>
            Custom
        }

        [Tooltip("How the description for this Save Point is defined.")]
        [SerializeField] protected DescriptionMode descriptionMode = DescriptionMode.Timestamp;

        [Tooltip("A short description of this save point.")]
        [SerializeField] protected string customDescription;

        /// <summary>
        /// Gets the save point description.
        /// </summary>
        public string SavePointDescription
        {
            get
            {
                if (descriptionMode == DescriptionMode.Timestamp)
                {
                    return TimeStampDesc;
                }
                else
                {
                    return customDescription;
                }
            }
        }

        public static string TimeStampDesc
        {
            get
            {
                return System.DateTime.Now.ToString("HH:mm dd MMMM, yyyy");
            }
        }

        public override void OnEnter()
        {
            if (ProgressMarker.LatestExecuted != this)
            {
                //do progressmarker
                ProgressMarker.LatestExecuted = this;

                var saveManager = FungusManager.Instance.SaveManager;

                saveManager.SaveAuto(CustomKey, SavePointDescription);
            }

            Continue();
        }
    }
}