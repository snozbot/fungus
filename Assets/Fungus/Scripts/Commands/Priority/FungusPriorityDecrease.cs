// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Decrease the FungusPriority count, causing the related FungusPrioritySignals to fire.
    /// Intended to be used to notify external systems that fungus is doing something important and they should perhaps resume.
    /// </summary>
    [CommandInfo("PrioritySignals",
                 "Priority Down",
                 "Decrease the FungusPriority count, causing the related FungusPrioritySignals to fire. " +
                "Intended to be used to notify external systems that fungus is doing something important and they should perhaps resume.")]
    public class FungusPriorityDecrease : Command
    {
        public override void OnEnter()
        {
            FungusPrioritySignals.DoDecreasePriorityDepth();

            Continue();
        }
    }
}