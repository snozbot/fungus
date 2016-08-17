/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

// Adapted from the Unity Test Tools project (MIT license)
// https://bitbucket.org/Unity-Technologies/unitytesttools/src/a30d562427e9/Assets/UnityTestTools/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Fungus
{

    public class LuaScript : MonoBehaviour
    {
        /// <summary>
        /// The Lua Environment to use when executing Lua script.
        /// </summary>
        [Tooltip("The Lua Environment to use when executing Lua script.")]
        public LuaEnvironment luaEnvironment;

        /// <summary>
        /// Text file containing Lua script to be executed.
        /// </summary>
        [Tooltip("Text file containing Lua script to be executed.")]
        public TextAsset luaFile;

        /// <summary>
        /// Lua script to execute.
        /// </summary>
        [Tooltip("A Lua string to execute, appended to the contents of Lua File (if one is specified).")]
        [TextArea(5, 50)]
        public string luaScript = "";

        /// <summary>
        /// Run the script as a Lua coroutine so execution can be yielded for asynchronous operations.
        /// </summary>
        [Tooltip("Run the script as a Lua coroutine so execution can be yielded for asynchronous operations.")]
        public bool runAsCoroutine = true;

        protected string friendlyName = "";

        // This is public so the editor code can force the component to reinitialise
        [NonSerialized]
        public bool initialised;

        // Stores the compiled Lua code for fast execution later.
        protected Closure luaFunction;

        // Recursively build the full hierarchy path to this game object
        private static string GetPath(Transform current) 
        {
            if (current.parent == null)
            {
                return current.name;
            }
            return GetPath(current.parent) + "." + current.name;
        }

        public void Start()
        {
            InitLuaScript();
        }

        /// <summary>
        /// Initialises the Lua environment and compiles the Lua string for execution later on.
        /// </summary>
        protected virtual void InitLuaScript()
        {
            if (initialised)
            {
                return;
            }

            if (luaEnvironment == null)        
            {
                // Create a Lua Environment if none exists yet
                luaEnvironment = LuaEnvironment.GetLua();
            }

            if (luaEnvironment == null)        
            {
                Debug.LogError("No Lua Environment found");
                return;
            }

            // Ensure the LuaEnvironment is initialized before trying to execute code
            luaEnvironment.InitEnvironment();

            // Cache a descriptive name to use in Lua error messages
            friendlyName = GetPath(transform) + ".LuaScript";

            string s = GetLuaString();
            luaFunction = luaEnvironment.LoadLuaString(s, friendlyName);

            initialised = true;
        }

        /// <summary>
        /// Returns the Lua string to be executed. 
        /// This is the contents of the Lua script appended to the contents of the Lua file.
        /// </summary>
        /// <returns>The lua string.</returns>
        protected virtual string GetLuaString()
        {
            string s = "";
            if (luaFile != null)
            {
                s = luaFile.text;
            }

            if (luaScript.Length > 0)
            {
                s += luaScript;
            }

            return s;
        }

        /// <summary>
        /// Execute the Lua script.
        /// This is the function to call if you want to trigger execution from an external script.
        /// </summary>
        public virtual void OnExecute()
        {
            // Make sure the script and Lua environment are initialised before executing
            InitLuaScript();

            if (luaEnvironment == null)
            {
                Debug.LogWarning("No Lua Environment found");
            }
            else
            {
                luaEnvironment.RunLuaFunction(luaFunction, runAsCoroutine);
            }
        }
    }
}
