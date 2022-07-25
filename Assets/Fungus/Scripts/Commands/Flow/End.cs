// This code is part of the Fungus library (https://github.com/snozbot/fungus)
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

        /// <summary>
        /// Set to true by looping constructs to allow for loops to occur
        /// </summary>
        public virtual bool Loop { get; set; }

        /// <summary>
        /// Set to the index of the owning looping construct
        /// </summary>
        public virtual int LoopBackIndex { get; set; }

        public override void OnEnter()
        {
            if (Loop)
            {
                Continue(LoopBackIndex);
                return;
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