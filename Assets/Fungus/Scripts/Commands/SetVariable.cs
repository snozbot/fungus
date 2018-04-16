// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Sets a Boolean, Integer, Float or String variable to a new value using a simple arithmetic operation. The value can be a constant or reference another variable of the same type.
    /// </summary>
    [CommandInfo("Variable",
                 "Set Variable",
                 "Sets a Boolean, Integer, Float or String variable to a new value using a simple arithmetic operation. The value can be a constant or reference another variable of the same type.")]
    [AddComponentMenu("")]
    public class SetVariable : Command
    {
        [Tooltip("The variable whos value will be set")]
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

        [Tooltip("The type of math operation to be performed")]
        [SerializeField] protected SetOperator setOperator;

        [Tooltip("Boolean value to set with")]
        [SerializeField] protected BooleanData booleanData;

        [Tooltip("Integer value to set with")]
        [SerializeField] protected IntegerData integerData;

        [Tooltip("Float value to set with")]
        [SerializeField] protected FloatData floatData;

        [Tooltip("String value to set with")]
        [SerializeField] protected StringDataMulti stringData;

        [Tooltip("Animator value to set with")]
        [SerializeField] protected AnimatorData animatorData;

        [Tooltip("AudioSource value to set with")]
        [SerializeField] protected AudioSourceData audioSourceData;

        [Tooltip("Color value to set with")]
        [SerializeField] protected ColorData colorData;

        [Tooltip("GameObject value to set with")]
        [SerializeField] protected GameObjectData gameObjectData;

        [Tooltip("Material value to set with")]
        [SerializeField] protected MaterialData materialData;

        [Tooltip("Object value to set with")]
        [SerializeField] protected ObjectData objectData;

        [Tooltip("Rigidbody2D value to set with")]
        [SerializeField] protected Rigidbody2DData rigidbody2DData;

        [Tooltip("Sprite value to set with")]
        [SerializeField] protected SpriteData spriteData;

        [Tooltip("Texture value to set with")]
        [SerializeField] protected TextureData textureData;

        [Tooltip("Transform value to set with")]
        [SerializeField] protected TransformData transformData;

        [Tooltip("Vector2 value to set with")]
        [SerializeField] protected Vector2Data vector2Data;

        [Tooltip("Vector3 value to set with")]
        [SerializeField] protected Vector3Data vector3Data;

        protected virtual void DoSetOperation()
        {
            if (variable == null)
            {
                return;
            }

            if (variable.GetType() == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = (variable as BooleanVariable);
                booleanVariable.Apply(setOperator, booleanData.Value);
            }
            else if (variable.GetType() == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = (variable as IntegerVariable);
                integerVariable.Apply(setOperator, integerData.Value);
            }
            else if (variable.GetType() == typeof(FloatVariable))
            {
                FloatVariable floatVariable = (variable as FloatVariable);
                floatVariable.Apply(setOperator, floatData.Value);
            }
            else if (variable.GetType() == typeof(StringVariable))
            {
                StringVariable stringVariable = (variable as StringVariable);
                var flowchart = GetFlowchart();
                stringVariable.Apply(setOperator, flowchart.SubstituteVariables(stringData.Value));
            }
            else if (variable.GetType() == typeof(AnimatorVariable))
            {
                AnimatorVariable animatorVariable = (variable as AnimatorVariable);
                animatorVariable.Apply(setOperator, animatorData.Value);
            }
            else if (variable.GetType() == typeof(AudioSourceVariable))
            {
                AudioSourceVariable audioSourceVariable = (variable as AudioSourceVariable);
                audioSourceVariable.Apply(setOperator, audioSourceData.Value);
            }
            else if (variable.GetType() == typeof(ColorVariable))
            {
                ColorVariable colorVariable = (variable as ColorVariable);
                colorVariable.Apply(setOperator, colorData.Value);
            }
            else if (variable.GetType() == typeof(GameObjectVariable))
            {
                GameObjectVariable gameObjectVariable = (variable as GameObjectVariable);
                gameObjectVariable.Apply(setOperator, gameObjectData.Value);
            }
            else if (variable.GetType() == typeof(MaterialVariable))
            {
                MaterialVariable materialVariable = (variable as MaterialVariable);
                materialVariable.Apply(setOperator, materialData.Value);
            }
            else if (variable.GetType() == typeof(ObjectVariable))
            {
                ObjectVariable objectVariable = (variable as ObjectVariable);
                objectVariable.Apply(setOperator, objectData.Value);
            }
            else if (variable.GetType() == typeof(Rigidbody2DVariable))
            {
                Rigidbody2DVariable rigidbody2DVariable = (variable as Rigidbody2DVariable);
                rigidbody2DVariable.Apply(setOperator, rigidbody2DData.Value);
            }
            else if (variable.GetType() == typeof(SpriteVariable))
            {
                SpriteVariable spriteVariable = (variable as SpriteVariable);
                spriteVariable.Apply(setOperator, spriteData.Value);
            }
            else if (variable.GetType() == typeof(TextureVariable))
            {
                TextureVariable textureVariable = (variable as TextureVariable);
                textureVariable.Apply(setOperator, textureData.Value);
            }
            else if (variable.GetType() == typeof(TransformVariable))
            {
                TransformVariable transformVariable = (variable as TransformVariable);
                transformVariable.Apply(setOperator, transformData.Value);
            }
            else if (variable.GetType() == typeof(Vector2Variable))
            {
                Vector2Variable vector2Variable = (variable as Vector2Variable);
                vector2Variable.Apply(setOperator, vector2Data.Value);
            }
            else if (variable.GetType() == typeof(Vector3Variable))
            {
                Vector3Variable vector3Variable = (variable as Vector3Variable);
                vector3Variable.Apply(setOperator, vector3Data.Value);
            }
        }

        #region Public members

        public static readonly Dictionary<System.Type, SetOperator[]> operatorsByVariableType = new Dictionary<System.Type, SetOperator[]>() {
            { typeof(BooleanVariable), BooleanVariable.setOperators },
            { typeof(IntegerVariable), IntegerVariable.setOperators },
            { typeof(FloatVariable), FloatVariable.setOperators },
            { typeof(StringVariable), StringVariable.setOperators },
            { typeof(AnimatorVariable), AnimatorVariable.setOperators },
            { typeof(AudioSourceVariable), AudioSourceVariable.setOperators },
            { typeof(ColorVariable), ColorVariable.setOperators },
            { typeof(GameObjectVariable), GameObjectVariable.setOperators },
            { typeof(MaterialVariable), MaterialVariable.setOperators },
            { typeof(ObjectVariable), ObjectVariable.setOperators },
            { typeof(Rigidbody2DVariable), Rigidbody2DVariable.setOperators },
            { typeof(SpriteVariable), SpriteVariable.setOperators },
            { typeof(TextureVariable), TextureVariable.setOperators },
            { typeof(TransformVariable), TransformVariable.setOperators },
            { typeof(Vector2Variable), Vector2Variable.setOperators },
            { typeof(Vector3Variable), Vector3Variable.setOperators }
        };

        /// <summary>
        /// The type of math operation to be performed.
        /// </summary>
        public virtual SetOperator _SetOperator { get { return setOperator; } }

        public override void OnEnter()
        {
            DoSetOperation();

            Continue();
        }

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "Error: Variable not selected";
            }

            string description = variable.Key;

            switch (setOperator)
            {
            default:
            case SetOperator.Assign:
                description += " = ";
                break;
            case SetOperator.Negate:
                description += " =! ";
                break;
            case SetOperator.Add:
                description += " += ";
                break;
            case SetOperator.Subtract:
                description += " -= ";
                break;
            case SetOperator.Multiply:
                description += " *= ";
                break;
            case SetOperator.Divide:
                description += " /= ";
                break;
            }

            if (variable.GetType() == typeof(BooleanVariable))
            {
                description += booleanData.GetDescription();
            }
            else if (variable.GetType() == typeof(IntegerVariable))
            {
                description += integerData.GetDescription();
            }
            else if (variable.GetType() == typeof(FloatVariable))
            {
                description += floatData.GetDescription();
            }
            else if (variable.GetType() == typeof(StringVariable))
            {
                description += stringData.GetDescription();
            }
            else if (variable.GetType() == typeof(AnimatorVariable))
            {
                description += animatorData.GetDescription();
            }
            else if (variable.GetType() == typeof(AudioSourceVariable))
            {
                description += audioSourceData.GetDescription();
            }
            else if (variable.GetType() == typeof(ColorVariable))
            {
                description += colorData.GetDescription();
            }
            else if (variable.GetType() == typeof(GameObjectVariable))
            {
                description += gameObjectData.GetDescription();
            }
            else if (variable.GetType() == typeof(MaterialVariable))
            {
                description += materialData.GetDescription();
            }
            else if (variable.GetType() == typeof(ObjectVariable))
            {
                description += objectData.GetDescription();
            }
            else if (variable.GetType() == typeof(Rigidbody2DVariable))
            {
                description += rigidbody2DData.GetDescription();
            }
            else if (variable.GetType() == typeof(SpriteVariable))
            {
                description += spriteData.GetDescription();
            }
            else if (variable.GetType() == typeof(TextureVariable))
            {
                description += textureData.GetDescription();
            }
            else if (variable.GetType() == typeof(TransformVariable))
            {
                description += transformData.GetDescription();
            }
            else if (variable.GetType() == typeof(Vector2Variable))
            {
                description += vector2Data.GetDescription();
            }
            else if (variable.GetType() == typeof(Vector3Variable))
            {
                description += vector3Data.GetDescription();
            }

            return description;
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == this.variable);
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}
