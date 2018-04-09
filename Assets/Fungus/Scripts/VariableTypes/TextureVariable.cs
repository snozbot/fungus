// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Texture variable type.
    /// </summary>
    [VariableInfo("Other", "Texture")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class TextureVariable : VariableBase<Texture>
    {
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign };

        public virtual bool Evaluate(CompareOperator compareOperator, Texture value)
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

        public override void Apply(SetOperator setOperator, Texture value)
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
    /// Container for a Texture variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct TextureData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(TextureVariable))]
        public TextureVariable textureRef;
        
        [SerializeField]
        public Texture textureVal;

        public TextureData(Texture v)
        {
            textureVal = v;
            textureRef = null;
        }
        
        public static implicit operator Texture(TextureData textureData)
        {
            return textureData.Value;
        }

        public Texture Value
        {
            get { return (textureRef == null) ? textureVal : textureRef.Value; }
            set { if (textureRef == null) { textureVal = value; } else { textureRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (textureRef == null)
            {
                return textureVal.ToString();
            }
            else
            {
                return textureRef.Key;
            }
        }
    }
}