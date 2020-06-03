// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Get a named float on a mixer
    /// </summary>
    [CommandInfo("Audio",
                 "Mixer Get Float",
                     "Get a named float on a mixer")]
    [AddComponentMenu("")]
    public class AudioMixerGetFloat : Command
    {
        [SerializeField] protected AudioMixerData mixer;
        [SerializeField] protected StringData paramName;

        [VariableProperty(typeof(FloatVariable))]
        [SerializeField] protected FloatVariable val;

        public override void OnEnter()
        {
            mixer.Value.GetFloat(paramName.Value, out var f);
            val.Value = f;
            Continue();
        }

        public override string GetSummary()
        {
            if (mixer.Value == null)
                return "Error: no mixer set";

            if (val == null)
                return "Error: no out float val set";

            return val.name + "=" + mixer.Value.name + "." + paramName.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return mixer.audioMixerRef == variable ||
                paramName.stringRef == variable ||
                val == variable ||
                base.HasReference(variable);
        }
    }
}