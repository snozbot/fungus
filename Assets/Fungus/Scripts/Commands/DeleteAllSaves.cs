// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Requests SaveManager delete all saves for the current profile and reset.
    /// </summary>
    [CommandInfo("Save",
                 "DeleteAll",
                 "Requests SaveManager delete all saves for the current profile and reset.")]
    public class DeleteAllSaves : Command
    {
        public override void OnEnter()
        {
            FungusManager.Instance.SaveManager.Restart(true);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}