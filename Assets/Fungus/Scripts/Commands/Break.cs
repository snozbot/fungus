// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Force a loop to terminate immediately.
    /// </summary>
    [CommandInfo("Flow", 
                 "Break", 
                 "Force a loop to terminate immediately.")]
    [AddComponentMenu("")]
    public class Break : Command
    {
        #region Public members

        public override void OnEnter()
        {
            Condition loopingCond = null;
            // Find index of previous looping command
            for (int i = CommandIndex - 1; i >=0; --i)
            {
                Condition cond = ParentBlock.CommandList[i] as Condition;
                if (cond != null && cond.IsLooping)
                {
                    loopingCond = cond;
                    break;
                }
            }

            if (loopingCond == null)
            {
                // No enclosing loop command found, just continue
                Continue();
            }
            else
            {
                loopingCond.MoveToEnd();
            }
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }    
}