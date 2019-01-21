// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    public abstract class VariableCondition : Condition
    {
        [Tooltip("The type of comparison to be performed")]
        [SerializeField] protected CompareOperator compareOperator;

        [Tooltip("Variable to use in expression")]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable),
                          typeof(FloatVariable),
                          typeof(StringVariable),
                          typeof(AnimatorVariable),
                          typeof(AudioSourceVariable),
                          typeof(ColorVariable),
                          typeof(GameObjectVariable),
                          typeof(MaterialVariable),
                          typeof(ObjectVariable),
                          typeof(Rigidbody2DVariable),
                          typeof(SpriteVariable),
                          typeof(TextureVariable),
                          typeof(TransformVariable),
                          typeof(Vector2Variable),
                          typeof(Vector3Variable))]
        [SerializeField] protected Variable variable;

        [Tooltip("Boolean value to compare against")]
        [SerializeField] protected BooleanData booleanData;

        [Tooltip("Integer value to compare against")]
        [SerializeField] protected IntegerData integerData;

        [Tooltip("Float value to compare against")]
        [SerializeField] protected FloatData floatData;

        [Tooltip("String value to compare against")]
        [SerializeField] protected StringDataMulti stringData;

        [Tooltip("Animator value to compare against")]
        [SerializeField] protected AnimatorData animatorData;

        [Tooltip("AudioSource value to compare against")]
        [SerializeField] protected AudioSourceData audioSourceData;

        [Tooltip("Color value to compare against")]
        [SerializeField] protected ColorData colorData;

        [Tooltip("GameObject value to compare against")]
        [SerializeField] protected GameObjectData gameObjectData;

        [Tooltip("Material value to compare against")]
        [SerializeField] protected MaterialData materialData;

        [Tooltip("Object value to compare against")]
        [SerializeField] protected ObjectData objectData;

        [Tooltip("Rigidbody2D value to compare against")]
        [SerializeField] protected Rigidbody2DData rigidbody2DData;

        [Tooltip("Sprite value to compare against")]
        [SerializeField] protected SpriteData spriteData;

        [Tooltip("Texture value to compare against")]
        [SerializeField] protected TextureData textureData;

        [Tooltip("Transform value to compare against")]
        [SerializeField] protected TransformData transformData;

        [Tooltip("Vector2 value to compare against")]
        [SerializeField] protected Vector2Data vector2Data;

        [Tooltip("Vector3 value to compare against")]
        [SerializeField] protected Vector3Data vector3Data;

        protected override bool EvaluateCondition()
        {
            if (variable == null)
            {
                return false;
            }

            bool condition = false;

            var t = variable.GetType();

            if (t == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = (variable as BooleanVariable);
                condition = booleanVariable.Evaluate(compareOperator, booleanData.Value);
            }
            else if (t == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = (variable as IntegerVariable);
                condition = integerVariable.Evaluate(compareOperator, integerData.Value);
            }
            else if (t == typeof(FloatVariable))
            {
                FloatVariable floatVariable = (variable as FloatVariable);
                condition = floatVariable.Evaluate(compareOperator, floatData.Value);
            }
            else if (t == typeof(StringVariable))
            {
                StringVariable stringVariable = (variable as StringVariable);
                condition = stringVariable.Evaluate(compareOperator, stringData.Value);
            }
            else if (t == typeof(AnimatorVariable))
            {
                AnimatorVariable animatorVariable = (variable as AnimatorVariable);
                condition = animatorVariable.Evaluate(compareOperator, animatorData.Value);
            }
            else if (t == typeof(AudioSourceVariable))
            {
                AudioSourceVariable audioSourceVariable = (variable as AudioSourceVariable);
                condition = audioSourceVariable.Evaluate(compareOperator, audioSourceData.Value);
            }
            else if (t == typeof(ColorVariable))
            {
                ColorVariable colorVariable = (variable as ColorVariable);
                condition = colorVariable.Evaluate(compareOperator, colorData.Value);
            }
            else if (t == typeof(GameObjectVariable))
            {
                GameObjectVariable gameObjectVariable = (variable as GameObjectVariable);
                condition = gameObjectVariable.Evaluate(compareOperator, gameObjectData.Value);
            }
            else if (t == typeof(MaterialVariable))
            {
                MaterialVariable materialVariable = (variable as MaterialVariable);
                condition = materialVariable.Evaluate(compareOperator, materialData.Value);
            }
            else if (t == typeof(ObjectVariable))
            {
                ObjectVariable objectVariable = (variable as ObjectVariable);
                condition = objectVariable.Evaluate(compareOperator, objectData.Value);
            }
            else if (t == typeof(Rigidbody2DVariable))
            {
                Rigidbody2DVariable rigidbody2DVariable = (variable as Rigidbody2DVariable);
                condition = rigidbody2DVariable.Evaluate(compareOperator, rigidbody2DData.Value);
            }
            else if (t == typeof(SpriteVariable))
            {
                SpriteVariable spriteVariable = (variable as SpriteVariable);
                condition = spriteVariable.Evaluate(compareOperator, spriteData.Value);
            }
            else if (t == typeof(TextureVariable))
            {
                TextureVariable textureVariable = (variable as TextureVariable);
                condition = textureVariable.Evaluate(compareOperator, textureData.Value);
            }
            else if (t == typeof(TransformVariable))
            {
                TransformVariable transformVariable = (variable as TransformVariable);
                condition = transformVariable.Evaluate(compareOperator, transformData.Value);
            }
            else if (t == typeof(Vector2Variable))
            {
                Vector2Variable vector2Variable = (variable as Vector2Variable);
                condition = vector2Variable.Evaluate(compareOperator, vector2Data.Value);
            }
            else if (t == typeof(Vector3Variable))
            {
                Vector3Variable vector3Variable = (variable as Vector3Variable);
                condition = vector3Variable.Evaluate(compareOperator, vector3Data.Value);
            }

            return condition;
        }

        protected override bool HasNeededProperties()
        {
            return (variable != null);
        }

        #region Public members

        public static readonly Dictionary<System.Type, CompareOperator[]> operatorsByVariableType = new Dictionary<System.Type, CompareOperator[]>() {
            { typeof(BooleanVariable), BooleanVariable.compareOperators },
            { typeof(IntegerVariable), IntegerVariable.compareOperators },
            { typeof(FloatVariable), FloatVariable.compareOperators },
            { typeof(StringVariable), StringVariable.compareOperators },
            { typeof(AnimatorVariable), AnimatorVariable.compareOperators },
            { typeof(AudioSourceVariable), AudioSourceVariable.compareOperators },
            { typeof(ColorVariable), ColorVariable.compareOperators },
            { typeof(GameObjectVariable), GameObjectVariable.compareOperators },
            { typeof(MaterialVariable), MaterialVariable.compareOperators },
            { typeof(ObjectVariable), ObjectVariable.compareOperators },
            { typeof(Rigidbody2DVariable), Rigidbody2DVariable.compareOperators },
            { typeof(SpriteVariable), SpriteVariable.compareOperators },
            { typeof(TextureVariable), TextureVariable.compareOperators },
            { typeof(TransformVariable), TransformVariable.compareOperators },
            { typeof(Vector2Variable), Vector2Variable.compareOperators },
            { typeof(Vector3Variable), Vector3Variable.compareOperators }
        };

        /// <summary>
        /// The type of comparison operation to be performed.
        /// </summary>
        public virtual CompareOperator _CompareOperator { get { return compareOperator; } }

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "Error: No variable selected";
            }

            var t = variable.GetType();

            string summary = variable.Key + " ";
            summary += Condition.GetOperatorDescription(compareOperator) + " ";

            if (t == typeof(BooleanVariable))
            {
                summary += booleanData.GetDescription();
            }
            else if (t == typeof(IntegerVariable))
            {
                summary += integerData.GetDescription();
            }
            else if (t == typeof(FloatVariable))
            {
                summary += floatData.GetDescription();
            }
            else if (t == typeof(StringVariable))
            {
                summary += stringData.GetDescription();
            }
            else if (t == typeof(AnimatorVariable))
            {
                summary += animatorData.GetDescription();
            }
            else if (t == typeof(AudioSourceVariable))
            {
                summary += audioSourceData.GetDescription();
            }
            else if (t == typeof(ColorVariable))
            {
                summary += colorData.GetDescription();
            }
            else if (t == typeof(GameObjectVariable))
            {
                summary += gameObjectData.GetDescription();
            }
            else if (t == typeof(MaterialVariable))
            {
                summary += materialData.GetDescription();
            }
            else if (t == typeof(ObjectVariable))
            {
                summary += objectData.GetDescription();
            }
            else if (t == typeof(Rigidbody2DVariable))
            {
                summary += rigidbody2DData.GetDescription();
            }
            else if (t == typeof(SpriteVariable))
            {
                summary += spriteData.GetDescription();
            }
            else if (t == typeof(TextureVariable))
            {
                summary += textureData.GetDescription();
            }
            else if (t == typeof(TransformVariable))
            {
                summary += transformData.GetDescription();
            }
            else if (t == typeof(Vector2Variable))
            {
                summary += vector2Data.GetDescription();
            }
            else if (t == typeof(Vector3Variable))
            {
                summary += vector3Data.GetDescription();
            }

            return summary;
        }

        public override bool HasReference(Variable variable)
        {
            bool retval = (variable == this.variable) || base.HasReference(variable);

            var t = variable.GetType();

            //this is a nightmare
            if (t == typeof(BooleanVariable))
            {
                retval |= booleanData.booleanRef == variable;
            }
            else if (t == typeof(IntegerVariable))
            {
                retval |= integerData.integerRef == variable;
            }
            else if (t == typeof(FloatVariable))
            {
                retval |= floatData.floatRef == variable;
            }
            else if (t == typeof(StringVariable))
            {
                retval |= stringData.stringRef == variable;
            }
            else if (t == typeof(AnimatorVariable))
            {
                retval |= animatorData.animatorRef == variable;
            }
            else if (t == typeof(AudioSourceVariable))
            {
                retval |= audioSourceData.audioSourceRef == variable;
            }
            else if (t == typeof(ColorVariable))
            {
                retval |= colorData.colorRef == variable;
            }
            else if (t == typeof(GameObjectVariable))
            {
                retval |= gameObjectData.gameObjectRef == variable;
            }
            else if (t == typeof(MaterialVariable))
            {
                retval |= materialData.materialRef == variable;
            }
            else if (t == typeof(ObjectVariable))
            {
                retval |= objectData.objectRef == variable;
            }
            else if (t == typeof(Rigidbody2DVariable))
            {
                retval |= rigidbody2DData.rigidbody2DRef == variable;
            }
            else if (t == typeof(SpriteVariable))
            {
                retval |= spriteData.spriteRef == variable;
            }
            else if (t == typeof(TextureVariable))
            {
                retval |= textureData.textureRef == variable;
            }
            else if (t == typeof(TransformVariable))
            {
                retval |= transformData.transformRef == variable;
            }
            else if (t == typeof(Vector2Variable))
            {
                retval |= vector2Data.vector2Ref == variable;
            }
            else if (t == typeof(Vector3Variable))
            {
                retval |= vector3Data.vector3Ref == variable;
            }

            return retval;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}
