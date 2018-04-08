using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// </summary>
    [CommandInfo("Scripting",
                 "Priority Up",
                 "Increases the FungusPriority count, causing the related FungusPrioritySignals to fire. " +
                "Intended to be used to notify external systems that fungus is doing something important and they should perhaps pause.")]
    public class FungusPrirorityIncrease : Command
    {
        public override void OnEnter()
        {
            FungusPrioritySignals.DoIncreasePriorityDepth();

            Continue();
        }
    }
}