// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Material variable type.
    /// </summary>
    [VariableInfo("Other", "Material")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class MaterialVariable : VariableBase<Material>
    {
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign };

        public virtual bool Evaluate(CompareOperator compareOperator, Material value)
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

        public override void Apply(SetOperator setOperator, Material value)
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
    /// Container for a Material variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct MaterialData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(MaterialVariable))]
        public MaterialVariable materialRef;
        
        [SerializeField]
        public Material materialVal;

        public MaterialData(Material v)
        {
            materialVal = v;
            materialRef = null;
        }
        
        public static implicit operator Material(MaterialData materialData)
        {
            return materialData.Value;
        }

        public Material Value
        {
            get { return (materialRef == null) ? materialVal : materialRef.Value; }
            set { if (materialRef == null) { materialVal = value; } else { materialRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (materialRef == null)
            {
                return materialVal.ToString();
            }
            else
            {
                return materialRef.Key;
            }
        }
    }
}