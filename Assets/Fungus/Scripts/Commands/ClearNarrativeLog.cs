using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Narrative", 
                 "Clear Narrative Log", 
                 "Clear the Narrative Log, erasing all entries")]
    public class ClearNarrativeLog : Command
    {
        public override void OnEnter()
        {
            // This should lead to the UI clearing all the entries; it should do so as a response
            // to the Narrative Log clearing its own history
            base.OnEnter();
            var manager = FungusManager.Instance;
            var narrLog = manager.NarrativeLog;

            narrLog.Clear();

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }
    }
}