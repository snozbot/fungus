// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Call TransitionTo on given Snapshot.
    /// </summary>
    [CommandInfo("Audio",
                 "Mixer Snapshot Transition To",
                     "Call TransitionTo on given Snapshot.")]
    [AddComponentMenu("")]
    public class AudioMixerSnapshotTransitionTo : Command
    {
        [SerializeField] protected AudioMixerSnapshotData snapshot;
        [SerializeField] protected FloatData timeToReach;

        public override void OnEnter()
        {
            snapshot.Value.TransitionTo(timeToReach.Value);
            Continue();
        }

        public override string GetSummary()
        {
            if (snapshot.Value == null)
                return "Error: no snapshot set";

            return snapshot.Value.name + " in " + timeToReach.Value.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return snapshot.audioMixerSnapshotRef == variable ||
                timeToReach.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}