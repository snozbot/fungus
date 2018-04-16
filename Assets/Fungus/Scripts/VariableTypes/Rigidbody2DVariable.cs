using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Rigidbody2D variable type.
    /// </summary>
    [VariableInfo("Other", "Rigidbody2D")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class Rigidbody2DVariable : VariableBase<Rigidbody2D>
    {
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign };

        public virtual bool Evaluate(CompareOperator compareOperator, Rigidbody2D value)
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

        public override void Apply(SetOperator setOperator, Rigidbody2D value)
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
    /// Container for a Rigidbody2D variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct Rigidbody2DData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(Rigidbody2DVariable))]
        public Rigidbody2DVariable rigidbody2DRef;

        [SerializeField]
        public Rigidbody2D rigidbody2DVal;

        public static implicit operator Rigidbody2D(Rigidbody2DData rigidbody2DData)
        {
            return rigidbody2DData.Value;
        }

        public Rigidbody2DData(Rigidbody2D v)
        {
            rigidbody2DVal = v;
            rigidbody2DRef = null;
        }

        public Rigidbody2D Value
        {
            get { return (rigidbody2DRef == null) ? rigidbody2DVal : rigidbody2DRef.Value; }
            set { if (rigidbody2DRef == null) { rigidbody2DVal = value; } else { rigidbody2DRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (rigidbody2DRef == null)
            {
                return rigidbody2DVal.ToString();
            }
            else
            {
                return rigidbody2DRef.Key;
            }
        }
    }
}