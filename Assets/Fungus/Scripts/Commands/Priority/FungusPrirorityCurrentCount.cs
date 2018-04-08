using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// </summary>
    [CommandInfo("Scripting",
                 "Priority Count",
                 "Resets the FungusPriority count to zero. Useful if you are among logic that is hard to have matching increase and decreases.")]
    public class FungusPrirorityCurrentCount : Command
    {
        public FloatVar outVar;

        public override void OnEnter()
        {
            outVar.Value = FungusPrioritySignals.CurrentPriorityDepth;

            Continue();
        }

        public override string GetSummary()
        {
            if(outVar == null)
            {
                return "Error: No out var supplied";
            }
            return outVar.Key;
        }
    }
}