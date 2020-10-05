// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Set or get a named float on a mixer
    /// </summary>
    [CommandInfo("Audio",
                 "Mixer Float",
                     "Set or get a named float on a mixer")]
    [AddComponentMenu("")]
    public class AudioMixerFloat : Command
    {
        [SerializeField] protected BaseVariableProperty.GetSet getOrSet;
        [SerializeField] protected AudioMixerData mixer;
        [SerializeField] protected StringData paramName;
        [SerializeField] protected FloatData val;

        public override void OnEnter()
        {
            switch (getOrSet)
            {
            case BaseVariableProperty.GetSet.Get:
                mixer.Value.GetFloat(paramName.Value, out var f);
                val.Value = f;
                break;
            case BaseVariableProperty.GetSet.Set:
                mixer.Value.SetFloat(paramName.Value, val.Value);
                break;
            default:
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (mixer.Value == null)
                return "Error: no mixer set";
            
            switch (getOrSet)
            {
            case BaseVariableProperty.GetSet.Get:
                if (val.floatRef == null)
                    return "Error: no float reference set";
                else
                    return val.floatRef.Key + "=" + mixer.Value.name + "." + paramName.Value;
            case BaseVariableProperty.GetSet.Set:
                return mixer.Value.name + "." + paramName.Value + "=" + val.Value.ToString();
            default:
                break;
            }
            return "Error: Unkown";
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