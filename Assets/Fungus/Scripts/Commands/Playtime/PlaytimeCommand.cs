using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.PlaytimeSys
{
    public abstract class PlaytimeCommand : Command
    {
        public override void OnEnter()
        {
            base.OnEnter();
            tracker = FungusManager.Instance.PlaytimeTracker;
        }

        protected PlaytimeTracker tracker;

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 184, 255);
        }
    }
}