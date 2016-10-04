// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Marks the end of a conditional block.
    /// </summary>
    [CommandInfo("Flow", 
                 "End", 
                 "Marks the end of a conditional block.")]
    [AddComponentMenu("")]
    public class End : Command
    {
        #region Public members

        public virtual bool Loop { get; set; }

        public override void OnEnter()
        {
            if (Loop)
            {
                for (int i = CommandIndex - 1; i >= 0; --i)
                {
                    System.Type commandType = ParentBlock.CommandList[i].GetType();
                    if (commandType == typeof(While))
                    {
                        Continue(i);
                        return;
                    }
                }
            }

            Continue();
        }

        public override bool CloseBlock()
        {
            return true;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}