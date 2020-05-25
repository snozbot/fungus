// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// AudioSource variable type.
    /// </summary>
    [VariableInfo("Other", "AudioSource")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class AudioSourceVariable : VariableBase<AudioSource>
    {
    }

    /// <summary>
    /// Container for an AudioSource variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct AudioSourceData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(AudioSourceVariable))]
        public AudioSourceVariable audioSourceRef;
        
        [SerializeField]
        public AudioSource audioSourceVal;

        public static implicit operator AudioSource(AudioSourceData audioSourceData)
        {
            return audioSourceData.Value;
        }

        public AudioSourceData(AudioSource v)
        {
            audioSourceVal = v;
            audioSourceRef = null;
        }

        public AudioSource Value
        {
            get { return (audioSourceRef == null) ? audioSourceVal : audioSourceRef.Value; }
            set { if (audioSourceRef == null) { audioSourceVal = value; } else { audioSourceRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (audioSourceRef == null)
            {
                return audioSourceVal != null ? audioSourceVal.ToString() : "Null";
            }
            else
            {
                return audioSourceRef.Key;
            }
        }
    }
}