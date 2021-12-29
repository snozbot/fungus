using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.PlaytimeSys
{
    [CommandInfo("Playtime", "Reset Playtime Tracking", "Resets the time recorded for tracking playtime")]
    public class ResetPlaytimeTracking : PlaytimeCommand
    {
        public override void OnEnter()
        {
            base.OnEnter();
            tracker.ResetRecording();
            Continue();
        }
    }
}