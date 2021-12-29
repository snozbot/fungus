using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.PlaytimeSys
{
    [CommandInfo("Playtime", "Stop Tracking Playtime", "As it sounds.")]
    public class StopTrackingPlaytime : Command
    {
        public override void OnEnter()
        {
            base.OnEnter();
            tracker = FungusManager.Instance.PlaytimeTracker;
            tracker.StopTracking();
            Continue();
        }

        protected PlaytimeTracker tracker;

    }
}