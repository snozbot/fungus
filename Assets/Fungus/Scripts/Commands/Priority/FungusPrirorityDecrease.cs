using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// </summary>
    [CommandInfo("Scripting",
                 "Priority Down",
                 "Decrease the FungusPriority count, causing the related FungusPrioritySignals to fire. " +
                "Intended to be used to notify external systems that fungus is doing something important and they should perhaps resume.")]
    public class FungusPrirorityDecrease : Command
    {
        public override void OnEnter()
        {
            FungusPrioritySignals.DoDecreasePriorityDepth();

            Continue();
        }
    }
}