// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Transform variable type.
    /// </summary>
    [VariableInfo("Other", "Transform")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class TransformVariable : VariableBase<Transform>
    {
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign };

        public virtual bool Evaluate(CompareOperator compareOperator, Transform value)
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

        public override void Apply(SetOperator setOperator, Transform value)
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
    /// Container for a Transform variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct TransformData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(TransformVariable))]
        public TransformVariable transformRef;
        
        [SerializeField]
        public Transform transformVal;

        public TransformData(Transform v)
        {
            transformVal = v;
            transformRef = null;
        }
        
        public static implicit operator Transform(TransformData vector3Data)
        {
            return vector3Data.Value;
        }

        public Transform Value
        {
            get { return (transformRef == null) ? transformVal : transformRef.Value; }
            set { if (transformRef == null) { transformVal = value; } else { transformRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (transformRef == null)
            {
                return transformVal.ToString();
            }
            else
            {
                return transformRef.Key;
            }
        }
    }
}