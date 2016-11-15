// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Fungus
{
    [CommandInfo("Variable", 
                 "Save Point", 
                 "Creates a save point which can be saved to persistant storage and loaded again later.")]
    public class SavePoint : Command
    {
        [SerializeField] protected string saveKey;

        [SerializeField] protected string description;

        [SerializeField] protected bool saveNow;

        #region Public members

        public override void OnEnter()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            saveManager.PopulateSaveBuffer(GetFlowchart(), saveKey, description);

            if (saveNow)
            {
                saveManager.Save();
            }

            Continue();
        }

        public override string GetSummary()
        {
            return saveKey;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}