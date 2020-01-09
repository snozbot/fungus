// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Copy the value of the Priority Count to a local IntegerVariable, intended primarily to assist with debugging use of Priority.
    /// </summary>
    [CommandInfo("PrioritySignals",
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

        public override bool HasReference(Variable variable)
        {
            return outVar == variable;
        }
    }
}