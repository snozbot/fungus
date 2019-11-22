// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
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
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign };

        public virtual bool Evaluate(CompareOperator compareOperator, AudioSource value)
        {
            bool condition = false;

            switch (compareOperator)
            {
                case CompareOperator.Equals:
                    condition = Value == value;
                    break;
                case CompareOperator.NotEquals:
                    condition = Value != value;
                    break;
                default:
                    Debug.LogError("The " + compareOperator.ToString() + " comparison operator is not valid.");
                    break;
            }

            return condition;
        }

        public override void Apply(SetOperator setOperator, AudioSource value)
        {
            switch (setOperator)
            {
                case SetOperator.Assign:
                    Value = value;
                    break;
                default:
                    Debug.LogError("The " + setOperator.ToString() + " set operator is not valid.");
                    break;
            }
        }
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
                return audioSourceVal.ToString();
            }
            else
            {
                return audioSourceRef.Key;
            }
        }
    }
}