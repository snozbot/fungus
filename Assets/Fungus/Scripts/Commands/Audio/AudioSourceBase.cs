// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public class AudioSourceBase : Command
    {
        [SerializeField] protected AudioSourceData audioSource;

        public override string GetSummary()
        {
            if (audioSource.Value == null)
                return "Error: no source set";

            return audioSource.Value.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return audioSource.audioSourceRef == variable ||
                base.HasReference(variable);
        }
    }
}