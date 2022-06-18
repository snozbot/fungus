// This code is part of the Fungus library (https://github.com/snozbot/fungus)
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
        public override bool IsArithmeticSupported(SetOperator setOperator)
        {
            return setOperator != SetOperator.Negate;
        }

        public override void Apply(SetOperator setOperator, Color value)
        {
            switch (setOperator)
            {
            case SetOperator.Add:
                Value += value;
                break;
            case SetOperator.Subtract:
                Value -= value;
                break;
            case SetOperator.Multiply:
                Value *= value;
                break;
            case SetOperator.Divide:
                Value *= new Color(1.0f/value.r, 1.0f / value.g, 1.0f / value.b, 1.0f / value.a);
                break;
            default:
                base.Apply(setOperator, value);
                break;
            }
        }

        public override bool IsSerializable { get { return true; } }

        public override string GetStringifiedValue()
        {
            return StringifyFloatArray(new float[] { Value.r, Value.g, Value.b, Value.a });
        }

        public override void RestoreFromStringifiedValue(string stringifiedValue)
        {
            var fs = FloatArrayFromString(stringifiedValue);
            if (fs.Length != 4)
                throw new System.Exception("Incorrect param count for ColorVaraiable.");

            Value = new Color(fs[0], fs[1], fs[2], fs[3]);
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