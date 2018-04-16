// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Animator variable type.
    /// </summary>
    [VariableInfo("Other", "Animator")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class AnimatorVariable : VariableBase<Animator>
    {
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign };

        public virtual bool Evaluate(CompareOperator compareOperator, Animator value)
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

        public override void Apply(SetOperator setOperator, Animator value)
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
    /// Container for an Animator variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct AnimatorData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(AnimatorVariable))]
        public AnimatorVariable animatorRef;
        
        [SerializeField]
        public Animator animatorVal;

        public static implicit operator Animator(AnimatorData animatorData)
        {
            return animatorData.Value;
        }

        public AnimatorData(Animator v)
        {
            animatorVal = v;
            animatorRef = null;
        }
            
        public Animator Value
        {
            get { return (animatorRef == null) ? animatorVal : animatorRef.Value; }
            set { if (animatorRef == null) { animatorVal = value; } else { animatorRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (animatorRef == null)
            {
                return animatorVal.ToString();
            }
            else
            {
                return animatorRef.Key;
            }
        }
    }
}