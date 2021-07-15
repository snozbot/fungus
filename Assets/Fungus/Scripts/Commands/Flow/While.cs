// This code is part of the Fungus library (https://github.com/snozbot/fungus)
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

        #endregion
    }    
}