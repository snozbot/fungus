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
                 "Add To Log Custom Strings",
                 "Manually add an item to the Narrative log.")]
    [AddComponentMenu("")]
    public class CustomStrings : Command, ILocalizable
    {
        [SerializeField] protected StringData nameString;
        [SerializeField] protected StringDataMulti bodyString;

        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";



        #region Public members

        public override void OnEnter()
        {
            FungusManager.Instance.NarrativeLog.AddLine(new NarrativeLogEntry() { name = nameString.Value, text = bodyString.Value });

            Continue();
        }

        public override string GetSummary()
        {
            return nameString.Value + ": " + bodyString.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }
        #endregion

        #region ILocalizable implementation
        public string GetStandardText()
        {
            return nameString.Value + Environment.NewLine + bodyString.Value;
        }

        public void SetStandardText(string standardText)
        {
            var splitPos = standardText.IndexOf(Environment.NewLine);
            nameString.Value = standardText.Substring(0, splitPos);
            bodyString.Value = standardText.Substring(splitPos + Environment.NewLine.Length);
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