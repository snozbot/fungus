// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    /// <summary>
    /// Calls the mix TransitionToSnapshots.
    /// </summary>
    [CommandInfo("Audio",
                 "Mixer Transition To Snapshots",
                     "Calls the mix TransitionToSnapshots.")]
    [AddComponentMenu("")]
    public class AudioMixerTransitionToSnapshots : Command, ICollectionCompatible
    {
        [SerializeField] protected AudioMixerData mixer;
        [SerializeField] protected AudioMixerSnapshot[] snapShots;
        [SerializeField] protected float[] floatArr;

        [Tooltip("Optional, if set values will be copied into floatArr before TransitionToSnapshots is called")]
        [SerializeField] protected CollectionData floatCollection;

        [SerializeField] protected FloatData timeToTransition;

        [Tooltip("Wait for the transition to complete before continuing.")]
        [SerializeField] protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            if (floatCollection.Value != null)
            {
                System.Array.Resize(ref floatArr, snapShots.Length);
                floatCollection.Value.CopyTo(floatArr, 0);
            }

            mixer.Value.TransitionToSnapshots(snapShots, floatArr, timeToTransition.Value);

            if (waitUntilFinished)
            {
                StartCoroutine(WaitForTransition());
            }
            else
            {
                Continue();
            }
        }

        protected IEnumerator WaitForTransition()
        {
            yield return new WaitForSeconds(timeToTransition.Value);
            Continue();
        }

        public override string GetSummary()
        {
            if (mixer.Value == null)
                return "Error: no mixer set";

            var retval =  mixer.Value.name + " " + snapShots.Length.ToString() + " in " + timeToTransition.Value.ToString() + "s";

            if (waitUntilFinished)
                retval += " waits";

            return retval;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return mixer.audioMixerRef == variable ||
                floatCollection.collectionRef == variable ||
                timeToTransition.floatRef == variable ||
                base.HasReference(variable);
        }

        public bool IsVarCompatibleWithCollection(Variable variableInQuestion, string compatibleWith)
        {
            if (compatibleWith == "floatCollection")
                return floatCollection.Value != null && floatCollection.Value.IsElementCompatible(variableInQuestion);
            else
                return true;
        }
    }
}