/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;
using Fungus;
using MoonSharp.Interpreter;

namespace Fungus
{

    [CommandInfo("Scripting",
                 "Execute Lua",
                 "Executes a Lua code chunk using a Lua Environment.")]
    public class ExecuteLua : Command 
    {
        [Tooltip("Lua Environment to use to execute this Lua script")]
        public LuaEnvironment luaEnvironment;

        [Tooltip("A text file containing Lua script to execute.")]
        public TextAsset luaFile;

        [TextArea(10,100)]
        [Tooltip("Lua script to execute. This text is appended to the contents of Lua file (if one is specified).")]
        public string luaScript;

        [Tooltip("Execute this Lua script as a Lua coroutine")]
        public bool runAsCoroutine = true;

        [Tooltip("Pause command execution until the Lua script has finished execution")]
        public bool waitUntilFinished = true;

        [Tooltip("A Flowchart variable to store the returned value in.")]
        [VariableProperty()]
        public Variable returnVariable;

        protected string friendlyName = "";

        protected bool initialised;

        // Stores the compiled Lua code for fast execution later.
        protected Closure luaFunction;
 
        protected virtual void Start()
        {
            InitExecuteLua();
		}

        /// <summary>
        /// Initialises the Lua environment and compiles the Lua string for execution later on.
        /// </summary>
        protected virtual void InitExecuteLua()
        {
            if (initialised)
            {
                return;
            }

            // Cache a descriptive name to use in Lua error messages
            friendlyName = gameObject.name + "." + parentBlock.blockName + "." + "ExecuteLua #" + commandIndex.ToString();

            Flowchart flowchart = GetFlowchart();

            // See if a Lua Environment has been assigned to this Flowchart
            if (luaEnvironment == null)        
            {
                luaEnvironment = flowchart.luaEnvironment;
            }

            // No Lua Environment specified so just use any available or create one.
            if (luaEnvironment == null)        
            {
                luaEnvironment = LuaEnvironment.GetLua();
            }

            string s = GetLuaString();
            luaFunction = luaEnvironment.LoadLuaString(s, friendlyName);

            // Add a binding to the parent flowchart
            if (flowchart.luaBindingName != "")
            {
                Table globals = luaEnvironment.Interpreter.Globals;
                if (globals != null)
				{
                	globals[flowchart.luaBindingName] = flowchart;
				}
            }

            // Always initialise when playing in the editor.
            // Allows the user to edit the Lua script while the game is playing.
            if ( !(Application.isPlaying && Application.isEditor) )
            {
                initialised = true;
            }

        }

        protected virtual string GetLuaString()
        {
            if (luaFile == null)
            {
                return luaScript;
            }

            return luaFile.text + "\n" + luaScript;
        }

        public override void OnEnter()
        {
            InitExecuteLua();

            if (luaFunction == null)
            {
                Continue();
            }

            luaEnvironment.RunLuaFunction(luaFunction, runAsCoroutine, (returnValue) => {
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
            return luaScript;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }

}
