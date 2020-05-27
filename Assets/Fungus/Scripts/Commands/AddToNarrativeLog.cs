// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;

namespace Fungus
{
    /// <summary>
    /// Manually add an item to the Narrative log.
    /// </summary>
    [CommandInfo("Narrative",
                 "Add To Log",
                 "Manually add an item to the Narrative log.")]
    [AddComponentMenu("")]
    public class AddToNarrativeLog : Command, ILocalizable
    {
        [Tooltip("Name of save profile to make active.")]
        [SerializeField] protected NarrativeLogEntry manualEntry = new NarrativeLogEntry();

        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";



        #region Public members

        public override void OnEnter()
        {
            FungusManager.Instance.NarrativeLog.AddLine(manualEntry);

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
            return "NARLOG." + GetFlowchartLocalizationId() + "." + itemId;
        }
        #endregion
    }
}