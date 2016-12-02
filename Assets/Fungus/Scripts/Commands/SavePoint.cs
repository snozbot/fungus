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
        [SerializeField] protected string savePointKey;

        [SerializeField] protected string savePointDescription;

        [SerializeField] protected bool resumeFromHere = true;

        #region Public members

        public string SavePointKey { get { return savePointKey; } }

        public bool ResumeFromHere { get { return resumeFromHere; } }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(savePointKey))
            {
                Continue();
                return;
            }

            var saveManager = FungusManager.Instance.SaveManager;

            saveManager.AddSavePoint(savePointKey, savePointDescription);

            Continue();
        }

        public override string GetSummary()
        {
            if (string.IsNullOrEmpty(savePointKey))
            {
                return "Error: Save Point Key not specified";
            }

            return savePointKey + " : " + savePointDescription;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}