// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using MoonSharp.Interpreter;

namespace Fungus
{
	public class LuaCondition : Condition 
	{
        [Tooltip("Lua Environment to use to execute this Lua script (null for global)")]
        [SerializeField] protected LuaEnvironment luaEnvironment;

		[Tooltip("The lua comparison string to run; implicitly prepends 'return' onto this")]
		[TextArea]
		public string luaCompareString;
        protected bool initialised;
        protected string friendlyName = "";
        protected Closure luaFunction;

		protected override bool EvaluateCondition()
		{
			bool condition = false;
            luaEnvironment.RunLuaFunction(luaFunction, false, (returnValue) => {
				if( returnValue != null )
				{
					condition = returnValue.Boolean;
				}
				else
				{
					Debug.LogWarning("No return value from " + friendlyName);
				}
            });
			return condition;
		}

		protected override bool HasNeededProperties()
		{
			return !string.IsNullOrEmpty(luaCompareString);
		}

        protected virtual void Start()
        {
            InitExecuteLua();
		}

        protected virtual string GetLuaString()
        {
            return "return not not (" + luaCompareString + ")";
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
            friendlyName = GetLocationIdentifier();

            Flowchart flowchart = GetFlowchart();

            // See if a Lua Environment has been assigned to this Flowchart
            if (luaEnvironment == null)        
            {
                luaEnvironment = flowchart.LuaEnv;
            }
            
            // No Lua Environment specified so just use any available or create one.
            if (luaEnvironment == null)        
            {
                luaEnvironment = LuaEnvironment.GetLua();
            }

            string s = GetLuaString();
            luaFunction = luaEnvironment.LoadLuaFunction(s, friendlyName);

            // Add a binding to the parent flowchart
            if (flowchart.LuaBindingName != "")
            {
                Table globals = luaEnvironment.Interpreter.Globals;
                if (globals != null)
				{
                	globals[flowchart.LuaBindingName] = flowchart;
				}
            }

            // Always initialise when playing in the editor.
            // Allows the user to edit the Lua script while the game is playing.
            if ( !(Application.isPlaying && Application.isEditor) )
            {
                initialised = true;
            }

        }

		#region Public members

		public override string GetSummary()
		{
			if (string.IsNullOrEmpty(luaCompareString))
			{
				return "Error: no lua compare string provided";
			}

			return luaCompareString;
		}

		#endregion
	}
}
