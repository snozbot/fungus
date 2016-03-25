using UnityEngine;
using System.Collections;
using Fungus;
using MoonSharp.Interpreter;

namespace Fungus
{

    [CommandInfo("FungusScript",
                 "Execute Lua",
                 "Executes a Lua code chunk using a FungusScript object.")]
    public class ExecuteLua : Command 
    {
        [Tooltip("FungusScript to use to execute this Lua script")]
        public FungusScript fungusScript;

        [TextArea(10,100)]
        [Tooltip("Lua script to execute. Use {$VarName} to insert a Flowchart variable in the Lua script.")]
        public string luaScript;

        /// <summary>
        /// Require the fungus Lua module at the start of the script. Equivalent to 'local fungus = require('fungus')
        /// </summary>
        [Tooltip("Require the fungus Lua module at the start of the script. Equivalent to 'local fungus = require('fungus')")]
        public bool useFungusModule = true;

        [Tooltip("Execute this Lua script as a Lua coroutine")]
        public bool runAsCoroutine = true;

        [Tooltip("Pause command execution until the Lua script has finished execution")]
        public bool waitUntilFinished = true;

        [Tooltip("A Flowchart variable to store the returned value in.")]
        [VariableProperty()]
        public Variable returnVariable;

        protected string friendlyName = "";

        protected virtual void Start()
        {
            // Cache a descriptive name to use in Lua error messages
            friendlyName = gameObject.name + "." + parentBlock.blockName + "." + "ExecuteLua #" + commandIndex.ToString();
        }

        public override void OnEnter()
        {
            if (fungusScript == null)        
            {
                Debug.LogWarning("No FungusScript object selected");            
                Continue();
                return;
            }
                
            string s = "";
            if (useFungusModule)
            {
                s = "fungus = require('fungus')\n";
            }

            string subbed = s + GetFlowchart().SubstituteVariables(luaScript);

            fungusScript.DoLuaString(subbed, friendlyName, runAsCoroutine, (returnValue) => {
                StoreReturnVariable(returnValue);
                if (waitUntilFinished)
                {
                    Continue();
                }
            });

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        protected virtual void StoreReturnVariable(DynValue returnValue)
        {
            if (returnVariable == null || returnValue == null)
            {
                return;
            }

            // Store the return value in a Fungus Variable
            System.Type variableType = returnVariable.GetType();
            if (variableType == typeof(BooleanVariable) && returnValue.Type == DataType.Boolean)
            {
                (returnVariable as BooleanVariable).value = returnValue.Boolean;
            }
            else if (variableType == typeof(IntegerVariable) && returnValue.Type == DataType.Number)
            {
                (returnVariable as IntegerVariable).value = (int)returnValue.Number;
            }
            else if (variableType == typeof(FloatVariable) && returnValue.Type == DataType.Number)
            {
                (returnVariable as FloatVariable).value = (float)returnValue.Number;
            }
            else if (variableType == typeof(StringVariable) && returnValue.Type == DataType.String)
            {
                (returnVariable as StringVariable).value = returnValue.String;
            }
            else if (variableType == typeof(ColorVariable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as ColorVariable).value = returnValue.CheckUserDataType<Color>("ExecuteLua.StoreReturnVariable");
            }
            else if (variableType == typeof(GameObjectVariable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as GameObjectVariable).value = returnValue.CheckUserDataType<GameObject>("ExecuteLua.StoreReturnVariable");
            }
            else if (variableType == typeof(MaterialVariable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as MaterialVariable).value = returnValue.CheckUserDataType<Material>("ExecuteLua.StoreReturnVariable");
            }
            else if (variableType == typeof(ObjectVariable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as ObjectVariable).value = returnValue.CheckUserDataType<Object>("ExecuteLua.StoreReturnVariable");
            }
            else if (variableType == typeof(SpriteVariable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as SpriteVariable).value = returnValue.CheckUserDataType<Sprite>("ExecuteLua.StoreReturnVariable");
            }
            else if (variableType == typeof(TextureVariable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as TextureVariable).value = returnValue.CheckUserDataType<Texture>("ExecuteLua.StoreReturnVariable");
            }
            else if (variableType == typeof(Vector2Variable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as Vector2Variable).value = returnValue.CheckUserDataType<Vector2>("ExecuteLua.StoreReturnVariable");
            }
            else if (variableType == typeof(Vector3Variable) && returnValue.Type == DataType.UserData)
            {
                (returnVariable as Vector3Variable).value = returnValue.CheckUserDataType<Vector3>("ExecuteLua.StoreReturnVariable");
            }
            else
            {
                Debug.LogError("Failed to convert " + returnValue.Type.ToLuaTypeString() + " return type to " + variableType.ToString());
            }
        }

        public override string GetSummary()
        {
            if (fungusScript == null)
            {
                return "Error: No FungusScript object selected";
            }

            return luaScript;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }

}
