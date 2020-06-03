// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Set a named float on a mixer
    /// </summary>
    [CommandInfo("Audio",
                 "Mixer Set Float",
                     "Set a named float on a mixer")]
    [AddComponentMenu("")]
    public class AudioMixerSetFloat : Command
    {
        [SerializeField] protected AudioMixerData mixer;
        [SerializeField] protected StringData paramName;
        [SerializeField] protected FloatData val;

        public override void OnEnter()
        {
            mixer.Value.SetFloat(paramName.Value, val.Value);
            Continue();
        }

        public override string GetSummary()
        {
            if (mixer.Value == null)
                return "Error: no mixer set";

            return mixer.Value.name + "." + paramName.Value + "=" + val.Value.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return mixer.audioMixerRef == variable ||
                paramName.stringRef == variable ||
                val.floatRef == variable ||
                base.HasReference(variable);
        }
    }
}