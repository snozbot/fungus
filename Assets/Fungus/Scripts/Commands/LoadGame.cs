// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [CommandInfo("Variable", 
                 "Load Game", 
                 "Loads a previously saved game. The original scene is loaded and the resume block is executed.")]
    public class LoadGame : Command
    {
        [SerializeField] protected IntegerData saveSlot = new IntegerData(0);

        #region Public members

        public override void OnEnter()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            saveManager.Load(saveSlot.Value);
        }

        public override string GetSummary()
        {
            return saveSlot.Value.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}