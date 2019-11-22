// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets a custom say dialog to use when displaying story text.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Set Say Dialog", 
                 "Sets a custom say dialog to use when displaying story text")]
    [AddComponentMenu("")]
    public class SetSayDialog : Command 
    {
        [Tooltip("The Say Dialog to use for displaying Say story text")]
        [SerializeField] protected SayDialog sayDialog;

        #region Public members

        public override void OnEnter()
        {
            if (sayDialog != null)
            {
                SayDialog.ActiveSayDialog = sayDialog;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (sayDialog == null)
            {
                return "Error: No say dialog selected";
            }

            return sayDialog.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        #endregion
    }
}