// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Play a source, optionaly setting the clip and delay when called.
    /// </summary>
    [CommandInfo("Audio",
                 "Play Source",
                     "Play a source, optionaly setting the clip and delay when called.")]
    [AddComponentMenu("")]
    public class AudioSourcePlay : Command
    {
        [SerializeField] protected AudioSourceData audioSource;

        [Tooltip("Optional clip to set on the source before playing")]
        [SerializeField] protected AudioClipData audioClip;

        [Tooltip("Optional, if non-zero will call PlayDelayed with delay value.")]
        [SerializeField] protected FloatData delay = new FloatData(0);

        [Tooltip("If true, will change the target source loop to matching the given 'loop' variable below.")]
        [SerializeField] protected bool modifySourceLooping = false;

        [Tooltip("Wait for the length of the clip that has been played before continuing.")]
        [SerializeField] protected bool loop = false;

        [Tooltip("Wait for the length of the clip that has been played before continuing.")]
        [SerializeField] protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            if (audioClip.Value != null)
            {
                audioSource.Value.clip = audioClip.Value;
            }

            if (delay.Value != 0)
            {
                audioSource.Value.PlayDelayed(delay);
            }
            else
            {
                audioSource.Value.Play();
            }

            if(modifySourceLooping)
            {
                audioSource.Value.loop = loop;
            }

            if (waitUntilFinished)
            {
                StartCoroutine(WaitForClipLength());
            }
            else
            {
                Continue();
            }
        }

        protected IEnumerator WaitForClipLength()
        {
            yield return new WaitForSeconds(audioSource.Value.clip.length);
            Continue();
        }

        public override string GetSummary()
        {
            if (audioSource.Value == null)
                return "Error: no source set";

            var retval = audioSource.Value.name;

            if (audioClip.Value != null)
                retval += ": " + audioClip.Value.name;

            if (delay.Value != 0)
                retval += ", " + delay.Value.ToString() + "s";

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
            return audioSource.audioSourceRef == variable ||
                audioClip.audioClipRef == variable ||
                delay.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}