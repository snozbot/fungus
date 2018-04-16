// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Vector3 variable type.
    /// </summary>
    [VariableInfo("Other", "Vector3")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class Vector3Variable : VariableBase<Vector3>
    {
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign, SetOperator.Add, SetOperator.Subtract };

        public virtual bool Evaluate(CompareOperator compareOperator, Vector3 value)
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

        public override void Apply(SetOperator setOperator, Vector3 value)
        {
            switch (setOperator)
            {
                case SetOperator.Assign:
                    Value = value;
                    break;
                case SetOperator.Add:
                    Value += value;
                    break;
                case SetOperator.Subtract:
                    Value -= value;
                    break;
                default:
                    Debug.LogError("The " + setOperator.ToString() + " set operator is not valid.");
                    break;
            }
        }
    }

    /// <summary>
    /// Container for a Vector3 variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct Vector3Data
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(Vector3Variable))]
        public Vector3Variable vector3Ref;
        
        [SerializeField]
        public Vector3 vector3Val;

        public Vector3Data(Vector3 v)
        {
            vector3Val = v;
            vector3Ref = null;
        }
        
        public static implicit operator Vector3(Vector3Data vector3Data)
        {
            return vector3Data.Value;
        }

        public Vector3 Value
        {
            get { return (vector3Ref == null) ? vector3Val : vector3Ref.Value; }
            set { if (vector3Ref == null) { vector3Val = value; } else { vector3Ref.Value = value; } }
        }

        public string GetDescription()
        {
            if (vector3Ref == null)
            {
                return vector3Val.ToString();
            }
            else
            {
                return vector3Ref.Key;
            }
        }
    }
}