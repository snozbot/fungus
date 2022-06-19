// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;

namespace Fungus
{
    /// <summary>
    /// Set the content of the Narrative Log entry to be posed if a menu timer elapses.
    /// </summary>
    [CommandInfo("Narrative",
                 "Set Timer Elapsed Narrative Log Text",
                 "Set the content of the Narrative Log entry to be posed if a menu timer elapses.")]
    [AddComponentMenu("")]
    public class SetMenuElapsedNarrativeLog : Command, ILocalizable
    {
        [Tooltip("Name of save profile to make active.")]
        [SerializeField] protected NarrativeLogEntry manualEntry = new NarrativeLogEntry();

        public static NarrativeLogEntry menuTimerElapsedEntry;

        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";



        #region Public members

        public override void OnEnter()
        {
            menuTimerElapsedEntry = manualEntry;

            Continue();
        }

        public override string GetSummary()
        {
            return manualEntry.name + ": " + manualEntry.text;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }
        #endregion

        #region ILocalizable implementation
        public string GetStandardText()
        {
            return manualEntry.name + Environment.NewLine + manualEntry.text;
        }

        public void SetStandardText(string standardText)
        {
            var splitPos = standardText.IndexOf(Environment.NewLine);
            manualEntry.name = standardText.Substring(0, splitPos);
            manualEntry.text = standardText.Substring(splitPos + Environment.NewLine.Length);
        }

        public string GetDescription()
        {
            return description;
        }

        public string GetStringId()
        {
            // String id for Menu commands is MENU.<Localization Id>.<Command id>
            return "TIMERNARLOG." + GetFlowchartLocalizationId() + "." + itemId;
        }
        #endregion
    }
}