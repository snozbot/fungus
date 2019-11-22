using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Copy the value of the Priority Count to a local IntegerVariable, intended primarily to assist with debugging use of Priority.
    /// </summary>
    [CommandInfo("Priority Signals",
                 "Get Priority Count",
                 "Copy the value of the Priority Count to a local IntegerVariable, intended primarily to assist with debugging use of Priority.")]
    public class FungusPriorityCount : Command
    {
        [VariableProperty(typeof(IntegerVariable))]
        public IntegerVariable outVar;

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