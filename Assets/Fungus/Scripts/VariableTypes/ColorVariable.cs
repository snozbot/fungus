// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Color variable type.
    /// </summary>
    [VariableInfo("Other", "Color")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class ColorVariable : VariableBase<Color>
    {
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = {
            SetOperator.Assign,
            SetOperator.Add,
            SetOperator.Subtract,
            SetOperator.Multiply
        };

        protected static bool ColorsEqual(Color a, Color b) {
            return ColorUtility.ToHtmlStringRGBA(a) == ColorUtility.ToHtmlStringRGBA(b);
        }

        public virtual bool Evaluate(CompareOperator compareOperator, Color value)
        {
            bool condition = false;

            switch (compareOperator)
            {
                case CompareOperator.Equals:
                    condition = ColorsEqual(Value, value);
                    break;
                case CompareOperator.NotEquals:
                    condition = !ColorsEqual(Value, value);
                    break;
                default:
                    Debug.LogError("The " + compareOperator.ToString() + " comparison operator is not valid.");
                    break;
            }

            return condition;
        }

        public override void Apply(SetOperator setOperator, Color value)
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
                case SetOperator.Multiply:
                    Value *= value;
                    break;
                default:
                    Debug.LogError("The " + setOperator.ToString() + " set operator is not valid.");
                    break;
            }
        }
    }

    /// <summary>
    /// Container for a Color variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct ColorData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(ColorVariable))]
        public ColorVariable colorRef;
        
        [SerializeField]
        public Color colorVal;

        public ColorData(Color v)
        {
            colorVal = v;
            colorRef = null;
        }
        
        public static implicit operator Color(ColorData colorData)
        {
            return colorData.Value;
        }

        public Color Value
        {
            get { return (colorRef == null) ? colorVal : colorRef.Value; }
            set { if (colorRef == null) { colorVal = value; } else { colorRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (colorRef == null)
            {
                return colorVal.ToString();
            }
            else
            {
                return colorRef.Key;
            }
        }
    }
}