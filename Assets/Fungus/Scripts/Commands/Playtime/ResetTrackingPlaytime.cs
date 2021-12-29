using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.PlaytimeSys
{
    [CommandInfo("Playtime", "Reset Tracking Playtime", "Resets the time recorded for tracking playtime")]
    public class ResetTrackingPlaytime : PlaytimeCommand
    {
        public override void OnEnter()
        {
            base.OnEnter();
            tracker.ResetTracking();
            Continue();
        }
    }
}