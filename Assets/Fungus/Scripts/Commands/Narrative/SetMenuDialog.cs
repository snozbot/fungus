// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets a custom menu dialog to use when displaying multiple choice menus.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Set Menu Dialog", 
                 "Sets a custom menu dialog to use when displaying multiple choice menus")]
    [AddComponentMenu("")]
    public class SetMenuDialog : Command 
    {
        [Tooltip("The Menu Dialog to use for displaying menu buttons")]
        [SerializeField] protected MenuDialog menuDialog;

        #region Public members

        public override void OnEnter()
        {
            if (menuDialog != null)
            {
                MenuDialog.ActiveMenuDialog = menuDialog;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (menuDialog == null)
            {
                return "Error: No menu dialog selected";
            }

            return menuDialog.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        #endregion
    }
}