/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;
using MoonSharp.RemoteDebugger;

namespace Fungus
{
    public class LuaEnvironment : MonoBehaviour 
    {
		/// <summary>
		/// Helper class used to extend the initialization behavior of LuaEnvironment.
		/// </summary>
		public abstract class Initializer : MonoBehaviour
		{
			/// <summary>
			/// Called when the LuaEnvironment is initializing.
			/// </summary>
			public abstract void Initialize();

            /// <summary>
            /// Applies transformations to the input script prior to execution.
            /// </summary>
            public abstract string PreprocessScript(string input);
		}

        /// <summary>
        /// Custom file loader for MoonSharp that loads in all Lua scripts in the project.
        /// Scripts must be placed in a Resources/Lua directory.
        /// </summary>
        protected class LuaScriptLoader : ScriptLoaderBase
        {
            // Give the script loader access to the list of accessible Lua Modules.
            private IEnumerable<TextAsset> luaScripts;
            public LuaScriptLoader(IEnumerable<TextAsset> luaScripts)
            {
                this.luaScripts = luaScripts;
            }

            /// <summary>
            // Bypasses the standard path resolution logic for require.
            /// </summary>
            protected override string ResolveModuleName(string modname, string[] paths)
            {
                return modname;
            }

            public override object LoadFile(string file, Table globalContext)
            {
                foreach (TextAsset luaScript in luaScripts)
                {
                    // Case insensitive string compare to allow standard Lua naming conventions in code
                    if (String.Compare(luaScript.name, file, true) == 0)
                    {
                        return luaScript.text;
                    }
                }
                return "";
            }

            public override bool ScriptFileExists(string name)
            {
                foreach (TextAsset luaScript in luaScripts)
                {
                    // Case insensitive string compare to allow standard Lua naming conventions in code
                    if (String.Compare(luaScript.name, name, true) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

		/// <summary>
		/// Returns the first Lua Environment found in the scene, or creates one if none exists.
		/// This is a slow operation, call it once at startup and cache the returned value.
		/// </summary>
		public static LuaEnvironment GetLua()
		{
			LuaEnvironment luaEnvironment = GameObject.FindObjectOfType<LuaEnvironment>();
			if (luaEnvironment == null)
			{
				GameObject prefab = Resources.Load<GameObject>("Prefabs/LuaEnvironment");
				if (prefab != null)
				{
					GameObject go = Instantiate(prefab) as GameObject;
					go.name = "LuaEnvironment";
					luaEnvironment = go.GetComponent<LuaEnvironment>();
				}
			}
			return luaEnvironment;
		}

        protected Script interpreter;

        /// <summary>
        /// Returns the MoonSharp interpreter instance used to run Lua code.
        /// </summary>
        public Script Interpreter { get { return interpreter; } }

        /// <summary>
        /// Launches the remote Lua debugger in your browser and breaks execution at the first executed Lua command.
        /// </summary>
        [Tooltip("Launches the remote Lua debugger in your browser and breaks execution at the first executed Lua command. Standalone platform only.")]
        public bool remoteDebugger = false;

        /// <summary>
        /// Instance of remote debugging service when debugging option is enabled.
        /// </summary>
        protected RemoteDebuggerService remoteDebuggerService;

        /// <summary>
        /// Flag used to avoid startup dependency issues.
        /// </summary>
        protected bool initialised = false;

		protected virtual void Start() 
        {
            InitEnvironment();
        }

        /// <summary>
        /// Initialise the Lua interpreter so we can start running Lua code.
        /// </summary>
        public virtual void InitEnvironment()
        {   
            if (initialised)
            {
                return;
            }

            Script.DefaultOptions.DebugPrint = (s) => { UnityEngine.Debug.Log(s); };

            // In some use cases (e.g. downloadable Lua files) some Lua modules can pose a potential security risk.
            // You can restrict which core lua modules are available here if needed. See the MoonSharp documentation for details.
            interpreter = new Script(CoreModules.Preset_Complete);

			// Load all Lua scripts in the project
            InitLuaScriptFiles();

			// Initialize any attached initializer components (e.g. LuaUtils)
			Initializer[] initializers = GetComponents<Initializer>();
			foreach (Initializer initializer in initializers)
			{
				initializer.Initialize();
			}

            if (remoteDebugger)
            {
                ActivateRemoteDebugger(interpreter);
            }

            initialised = true;
        }

        /// <summary>
        /// Register all Lua files in the project so they can be accessed at runtime.
        /// </summary>
        protected virtual void InitLuaScriptFiles()
        {
            object[] result = Resources.LoadAll("Lua", typeof(TextAsset));
            interpreter.Options.ScriptLoader = new LuaScriptLoader(result.OfType<TextAsset>());
        }

		/// <summary>
		/// Register a type given it's assembly qualified name.
		/// </summary>
        public static void RegisterType(string typeName, bool extensionType = false)
		{
            System.Type t = System.Type.GetType(typeName);
			if (t == null)
			{
				UnityEngine.Debug.LogWarning("Type not found: " + typeName);
				return;
			}

			// Registering System.Object breaks MoonSharp's automated conversion of Lists and Dictionaries to Lua tables.
			if (t == typeof(System.Object))
			{
				return;
			}

			if (!UserData.IsTypeRegistered(t))
			{
                try
                {
    				if (extensionType)
    				{
    					UserData.RegisterExtensionType(t);
    				}
    				else
    				{
    					UserData.RegisterType(t);
    				}
                }
                catch (ArgumentException ex)
                {
                    UnityEngine.Debug.LogWarning(ex.Message);
                }
			}
		}

        /// <summary>
        /// Loads and compiles a string containing Lua script, returning a closure (Lua function) which can be executed later.
        /// <param name="luaString">The Lua code to be run.</param>
        /// <param name="friendlyName">A descriptive name to be used in error reports.</param>
        /// </summary>
        public virtual Closure LoadLuaString(string luaString, string friendlyName)
        {
            InitEnvironment();

            string processedString;
            Initializer initializer = GetComponent<Initializer>();
            if (initializer != null)
            {
                processedString = initializer.PreprocessScript(luaString);
            }
            else
            {
                processedString = luaString;
            }

            // Load the Lua script
            DynValue res = null;
            try
            {
                res = interpreter.LoadString(processedString, null, friendlyName);
            }
            catch (InterpreterException ex)
            {
                LogException(ex.DecoratedMessage, luaString);
            }

            if (res == null ||
                res.Type != DataType.Function)
            {
                UnityEngine.Debug.LogError("Failed to create Lua function from Lua string");
                return null;
            }

            return res.Function;
        }
            
        /// <summary>
        /// Load and run a previously compiled Lua script. May be run as a coroutine.
        /// <param name="fn">A previously compiled Lua function.</param>
        /// <param name="runAsCoroutine">Run the Lua code as a coroutine to support asynchronous operations.</param>
        /// <param name="onComplete">Method to callback when the Lua code finishes exection. Supports return parameters.</param>
        /// </summary>
        public virtual void RunLuaFunction(Closure fn, bool runAsCoroutine, Action<DynValue> onComplete = null)
        {            
            if (fn == null)
            {
                if (onComplete != null)
                {
                    onComplete(null);
                }

                return;
            }

            // Execute the Lua script
            if (runAsCoroutine)
            {
                StartCoroutine(RunLuaCoroutine(fn, onComplete));
            }
            else
            {
                DynValue returnValue = null;
                try
                {                
                    returnValue = fn.Call();                
                }
                catch (InterpreterException ex)
                {
					LogException(ex.DecoratedMessage, GetSourceCode());
                }

                if (onComplete != null)
                {
                    onComplete(returnValue);
                }
            }
        }

        /// <summary>
        /// Load and run a string containing Lua script. May be run as a coroutine.
        /// <param name="luaString">The Lua code to be run.</param>
        /// <param name="friendlyName">A descriptive name to be used in error reports.</param>
        /// <param name="runAsCoroutine">Run the Lua code as a coroutine to support asynchronous operations.</param>
        /// <param name="onComplete">Method to callback when the Lua code finishes exection. Supports return parameters.</param>
        /// </summary>
        public virtual void DoLuaString(string luaString, string friendlyName, bool runAsCoroutine, Action<DynValue> onComplete = null)
        {
            Closure fn = LoadLuaString(luaString, friendlyName);
            RunLuaFunction(fn, runAsCoroutine, onComplete);
        }
            
        /// <summary>
        /// A Unity coroutine method which updates a Lua coroutine each frame.
        /// <param name="closure">A MoonSharp closure object representing a function.</param>
        /// <param name="onComplete">A delegate method that is called when the coroutine completes. Includes return parameter.</param>
        /// </summary>
        protected virtual IEnumerator RunLuaCoroutine(Closure closure, Action<DynValue> onComplete = null)
        {
            DynValue co = interpreter.CreateCoroutine(closure);

            DynValue returnValue = null;
            while (co.Coroutine.State != CoroutineState.Dead)
            {
                try
                {                
                    returnValue = co.Coroutine.Resume();
                }
                catch (InterpreterException ex)
                {
					LogException(ex.DecoratedMessage, GetSourceCode());
                }

                yield return null;
            }

            if (onComplete != null)
            {
                onComplete(returnValue);
            }
        }

		protected virtual string GetSourceCode()
		{
			// Get most recently executed source code
			string sourceCode = "";
			if (interpreter.SourceCodeCount > 0)
			{
				MoonSharp.Interpreter.Debugging.SourceCode sc = interpreter.GetSourceCode(interpreter.SourceCodeCount - 1);
				if (sc != null)
				{
					sourceCode = sc.Code;
				}
			}
			return sourceCode;
		}

        /// <summary>
        /// Start a Unity coroutine from a Lua call.
        /// </summary>
        public virtual Task RunUnityCoroutine(IEnumerator coroutine)
        {
            if (coroutine == null)
            {
                return null;
            }

            // We use the Task class so we can poll the coroutine to check if it has finished.
            // Standard Unity coroutines don't support this check.
            return new Task(RunUnityCoroutineImpl(coroutine));
        }

        /// <summary>
        /// Starts a standard Unity coroutine.
        /// The coroutine is managed by the LuaEnvironment monobehavior, so you can call StopAllCoroutines to
        /// stop all active coroutines later.
        /// </summary>
        protected virtual IEnumerator RunUnityCoroutineImpl(IEnumerator coroutine)
        {
            if (coroutine == null)
            {
                UnityEngine.Debug.LogWarning("Coroutine must not be null");
                yield break;
            }

            yield return StartCoroutine(coroutine);
        }

        protected virtual void ActivateRemoteDebugger(Script script)
        {
            #if UNITY_STANDALONE
            if (remoteDebuggerService == null)
            {
                remoteDebuggerService = new RemoteDebuggerService();

                // the last boolean is to specify if the script is free to run 
                // after attachment, defaults to false
                remoteDebuggerService.Attach(script, gameObject.name, false);
            }

            // start the web-browser at the correct url. Replace this or just
            // pass the url to the user in some way.
            Process.Start(remoteDebuggerService.HttpUrlStringLocalHost);
            #endif
        }

		/// <summary>
		/// Writes a MoonSharp exception to the debug log in a helpful format.
		/// </summary>
		/// <param name="decoratedMessage">Decorated message from a MoonSharp exception</param>
		/// <param name="debugInfo">Debug info, usually the Lua script that was running.</param>
		public static void LogException(string decoratedMessage, string debugInfo)
		{
			string output = decoratedMessage + "\n";

			char[] separators = { '\r', '\n' };
			string[] lines = debugInfo.Split(separators, StringSplitOptions.None);

			// Show line numbers for script listing
			int count = 1;
			foreach (string line in lines)
			{
				output += count.ToString() + ": " + line + "\n";
				count++;
			}
				
			UnityEngine.Debug.LogError(output);
		}
    }

}