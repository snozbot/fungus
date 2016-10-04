// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stop executing the Block that contains this command.
    /// </summary>
    [CommandInfo("Flow", 
                 "Stop", 
                 "Stop executing the Block that contains this command.")]
    [AddComponentMenu("")]
    public class Stop : Command
    {
        #region Public members

        public override void OnEnter()
        {
            StopParentBlock();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}