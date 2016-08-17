/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Narrative", 
                 "Set Menu Dialog", 
                 "Sets a custom menu dialog to use when displaying multiple choice menus")]
    [AddComponentMenu("")]
    public class SetMenuDialog : Command 
    {
        [Tooltip("The Menu Dialog to use for displaying menu buttons")]
        public MenuDialog menuDialog;

        public override void OnEnter()
        {
            if (menuDialog != null)
            {
                MenuDialog.activeMenuDialog = menuDialog;
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
    }

}