// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// PlayOneShot with given clip on given source
    /// </summary>
    [CommandInfo("Audio",
                 "Play Source One Shot",
                     "PlayOneShot with given clip on given source")]
    [AddComponentMenu("")]
    public class AudioSourcePlayOneShot : Command
    {
        [SerializeField] protected AudioSourceData audioSource;
        [SerializeField] protected AudioClipData audioClip;

        [Tooltip("Optional, volume scale passed into the PlayOneShot.")]
        [SerializeField] protected FloatData volumeScale = new FloatData(1);

        [Tooltip("Wait for the length of the clip that has been played before continuing.")]
        [SerializeField] protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            audioSource.Value.PlayOneShot(audioClip.Value, volumeScale.Value);

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
            yield return new WaitForSeconds(audioClip.Value.length);
            Continue();
        }

        public override string GetSummary()
        {
            if (audioSource.Value == null)
                return "Error: no source set";

            if (audioClip.Value == null)
                return "Error: no clip set";

            var retval = audioSource.Value.name + ": " + audioClip.Value.name + " @ " + volumeScale.Value.ToString();

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
                volumeScale.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}