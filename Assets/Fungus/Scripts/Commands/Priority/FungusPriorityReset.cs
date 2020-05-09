// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Resets the FungusPriority count to zero. Useful if you are among logic that is hard to have matching increase and decreases.
    /// </summary>
    [CommandInfo("PrioritySignals",
                 "Priority Reset",
                 "Resets the FungusPriority count to zero. Useful if you are among logic that is hard to have matching increase and decreases.")]
    public class FungusPriorityReset : Command
    {
        public override void OnEnter()
        {
            FungusPrioritySignals.DoResetPriority();

            Continue();
        }
    }
}