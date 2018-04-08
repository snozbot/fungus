using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// </summary>
    [CommandInfo("Scripting",
                 "Priority Reset",
                 "Resets the FungusPriority count to zero. Useful if you are among logic that is hard to have matching increase and decreases.")]
    public class FungusPrirorityReset : Command
    {
        public override void OnEnter()
        {
            FungusPrioritySignals.DoResetPriority();

            Continue();
        }
    }
}