// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Attempts to parse a string into a given fungus variable type, such as integer or float
    /// </summary>
    [CommandInfo("Variable",
                 "From String",
                 "Attempts to parse a string into a given fungus variable type, such as integer or float")]
    [AddComponentMenu("")]
    public class FromString : Command
    {
        [Tooltip("Source of string data to parse into another variables value")]
        [VariableProperty(typeof(StringVariable))]
        [SerializeField] protected StringVariable sourceString;

        [Tooltip("The variable type to be parsed and value stored within")]
        [VariableProperty(typeof(IntegerVariable), typeof(FloatVariable))]
        [SerializeField] protected Variable outValue;

        public override void OnEnter()
        {
            if (sourceString != null && outValue != null)
            {
                double asDouble = 0;
                try
                {
                    asDouble = System.Convert.ToDouble(sourceString.Value, System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (System.Exception)
                {
                    Debug.LogWarning("Failed to parse as number: " + sourceString.Value);
                }

                IntegerVariable intOutVar = outValue as IntegerVariable;
                if (intOutVar != null)
                {
                    intOutVar.Value = (int)asDouble;
                }
                else
                {
                    FloatVariable floatOutVar = outValue as FloatVariable;
                    if (floatOutVar != null)
                    {
                        floatOutVar.Value = (float)asDouble;
                    }
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (sourceString == null)
            {
                return "Error: No source string selected";
            }

            if (outValue == null)
            {
                return "Error: No type and storage variable selected";
            }

            return outValue.Key + ".Parse " + sourceString.Key;
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == sourceString) || (variable == outValue);
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }
    }
}