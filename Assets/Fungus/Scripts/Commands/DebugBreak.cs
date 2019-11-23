// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Writes a log message to the debug console.
    /// </summary>
    [CommandInfo("Scripting",
                 "Debug Break",
                 "Calls Debug.Break if enabled. Also useful for putting a visual studio breakbpoint within.")]
    [AddComponentMenu("")]
    public class DebugBreak : Command
    {
        [SerializeField] new protected BooleanData enabled = new BooleanData(true);

        public override void OnEnter()
        {
            if (enabled.Value)
                Debug.Break();

            Continue();
        }

        public override string GetSummary()
        {
            return enabled.Value ? "enabled" : "disabled";
        }

        public override bool HasReference(Variable variable)
        {
            return variable == enabled.booleanRef;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}