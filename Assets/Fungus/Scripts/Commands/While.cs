// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Continuously loop through a block of commands while the condition is true. Use the Break command to force the loop to terminate immediately.
    /// </summary>
    [CommandInfo("Flow", 
                 "While", 
                 "Continuously loop through a block of commands while the condition is true. Use the Break command to force the loop to terminate immediately.")]
    [AddComponentMenu("")]
    public class While : If
    {
        #region Public members

        public override bool IsLooping { get { return true; } }

        protected override void OnFalse()
        {
            MoveToEnd();
        }

        protected override bool HasNeededProperties()
        {
            return HasRequiredEnd(true);
        }

        #endregion
    }    
}