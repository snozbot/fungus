using UnityEngine;
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

    public class FungusScript : MonoBehaviour 
    {
        /// <summary>
        /// Custom file loader for MoonSharp that loads in all Lua scripts in the project.
        /// Scripts must be placed in a Resources/Lua directory.
        /// </summary>
        protected class FungusScriptLoader : ScriptLoaderBase
        {
            // Give the script loader access to the list of accessible Lua Modules.
            private IEnumerable<TextAsset> luaScripts;
            public FungusScriptLoader(IEnumerable<TextAsset> luaScripts)
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

        protected Script interpreter;

        /// <summary>
        /// Returns the MoonSharp interpreter instance used to run Lua code.
        /// </summary>
        public Script Interpreter { get { return interpreter; } }

        /// <summary>
        /// Launches the remote Lua debugger in your browser and breaks execution at the first executed Lua command.
        /// </summary>
        [Tooltip("Launches the remote Lua debugger in your browser and breaks execution at the first executed Lua command.")]
        public bool remoteDebugger = false;

        /// <summary>
        /// Lua script file which defines the global string table used for localisation.
        /// </summary>
        [Tooltip("Lua script file which defines the global string table used for localisation.")]
        public TextAsset stringTable;

        /// <summary>
        /// The currently selected language in the string table. Affects variable substitution.
        /// </summary>
        [Tooltip("The currently selected language in the string table. Affects variable substitution.")]
        public string activeLanguage = "en";

        /// <summary>
        /// Time scale factor to apply when running Lua scripts.
        /// This allows pausing of a FungusScript by setting timescale to 0.
        /// Use the GetTime() and GetDeltaTime() functions to get scaled time values.
        /// If negative, then GetTime() and GetDeltaTime() return the same values as the standard Time class.
        /// </summary>
        [Tooltip("Time scale factor to apply when running Lua scripts. If negative then uses the same values as the standard Time class.")]
        public float timeScale = -1f;

		/// <summary>
		/// A text file listing the c# types that can be accessed from Lua.
		/// </summary>
		[HideInInspector]
		public List<TextAsset> registerTypes = new List<TextAsset>();

		/// <summary>
		/// A text file listing the c# extension types that can be accessed from Lua.
		/// </summary>
		[HideInInspector]
		public List<TextAsset> registerExtensionTypes = new List<TextAsset>();

        /// <summary>
        /// Instance of remote debugging service when debugging option is enabled.
        /// </summary>
        protected RemoteDebuggerService remoteDebuggerService;

        /// <summary>
        /// Flag used to avoid startup dependency issues.
        /// </summary>
        protected bool initialised = false;

        /// <summary>
        /// Cached referenence to the string table (if loaded).
        /// </summary>
        protected Table stringTableCached;

        protected virtual void Start() 
        {
            InitInterpreter();
        }

        /// <summary>
        /// Initialise the Lua interpreter so we can start running Lua code.
        /// </summary>
        public virtual void InitInterpreter()
        {   
            if (initialised)
            {
                return;
            }

            Script.DefaultOptions.DebugPrint = (s) => { UnityEngine.Debug.Log(s); };

            // In some use cases (e.g. downloadable Lua files) some Lua modules can pose a potential security risk.
            // You can restrict which core lua modules are available here if needed.
            // See the MoonSharp documentation for details.
            interpreter = new Script(CoreModules.Preset_Complete);

            InitLuaScriptFiles();
            InitTypes();
            InitCustomObjects();
            InitBindings();
            InitStringTable();

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
            interpreter.Options.ScriptLoader = new FungusScriptLoader(result.OfType<TextAsset>());
        }

        /// <summary>
        /// Registers all listed c# types for interop with Lua.
        /// You can also register types directly in the Awake method of any 
		/// monobehavior in your scene using UserData.RegisterType() directly.
        /// </summary>
        protected virtual void InitTypes()
        {
			// Register types
			foreach (TextAsset textFile in registerTypes)
			{
				if (textFile == null)
				{
					continue;
				}

				char[] separators = { '\r', '\n' };
				foreach (string typeName in textFile.text.Split(separators, StringSplitOptions.RemoveEmptyEntries))
				{
					RegisterType(typeName.Trim());
				}
			}

			// Register extension types
			foreach (TextAsset textFile in registerExtensionTypes)
			{
				if (textFile == null)
				{
					continue;
				}

				char[] separators = { '\r', '\n' };
				foreach (string typeName in textFile.text.Split(separators, StringSplitOptions.RemoveEmptyEntries))
				{
					RegisterExtensionType(typeName.Trim());
				}
			}
        }

		/// <summary>
		/// Register a type given it's assembly qualified name.
		/// </summary>
		public virtual void RegisterType(string typeName)
		{
			System.Type t = System.Type.GetType(typeName);
			if (t == null)
			{
				UnityEngine.Debug.LogWarning("Type not found: " + typeName);
				return;
			}

			if (!UserData.IsTypeRegistered(t))
			{
				UserData.RegisterType(t);
			}
		}

		/// <summary>
		/// Register an extension type given it's assembly qualified name.
		/// </summary>
		public virtual void RegisterExtensionType(string typeName)
		{
			System.Type t = System.Type.GetType(typeName);
			if (t == null)
			{
				UnityEngine.Debug.LogWarning("Extension type not found: " + typeName);
				return;
			}

			if (!UserData.IsTypeRegistered(t))
			{
				UserData.RegisterExtensionType(t);
			}
		}

        /// <summary>
        /// Register some commonly used Unity classes and objects for Lua interop.
        /// To register more class objects externally to this class, register them in the Awake method of any 
        /// monobehavior in your scene.
        /// </summary>
        protected virtual void InitCustomObjects()
        {
            // Add the CLR class objects to a global unity table
            Table unityTable = new Table(interpreter);
            interpreter.Globals["unity"] = unityTable;

			// Static classes
            unityTable["time"] = UserData.CreateStatic(typeof(Time));
            unityTable["fungusprefs"] = UserData.CreateStatic(typeof(FungusPrefs));

            // This FungusScript object
            unityTable["fungusscript"] = this;

            // Provide access to the Unity Test Tools (if available).
            Type testType = Type.GetType("IntegrationTest");
            if (testType != null)
            {
                UserData.RegisterType(testType);
                unityTable["test"] = UserData.CreateStatic(testType);
            }

            // Example of how to register an enum
            // UserData.RegisterType<MyClass.MyEnum>();
            // interpreter.Globals.Set("MyEnum", UserData.CreateStatic<MyClass.MyEnum>());
        }

        /// <summary>
        /// Binds all gameobjects and components defined in FungusBinding objects to the global scene table.
        /// </summary>
        protected virtual void InitBindings()
        {
            LuaBindings[] bindings = GameObject.FindObjectsOfType<LuaBindings>();
            foreach (LuaBindings binding in bindings)
            {
                binding.AddBindings(interpreter.Globals);
            }
        }

        /// <summary>
        /// Loads the global string table used for localisation.
        /// </summary>
        protected virtual void InitStringTable()
        {
            if (stringTable != null)
            {
                try
                {                
                    DynValue stringTableRes = interpreter.DoString(stringTable.text);
                    if (stringTableRes.Type == DataType.Table)
                    {
                        stringTableCached = stringTableRes.Table;
                        interpreter.Globals["stringtable"] = stringTableCached;
                    }
                }
                catch (ScriptRuntimeException ex)
                {
                    UnityEngine.Debug.LogError("Lua runtime error: " + ex.DecoratedMessage);
                }
                catch (InterpreterException ex)
                {
                    UnityEngine.Debug.LogError(ex.DecoratedMessage);
                }
            }
        }

        /// <summary>
        /// Returns a string from the string table for this key.
        /// The string returned depends on the active language.
        /// </summary>
        public virtual string GetString(string key)
        {
            if (stringTableCached != null)
            {
                // Match against string table and active language
                DynValue stringTableVar = stringTableCached.Get(key);
                if (stringTableVar.Type == DataType.Table)
                {
                    DynValue languageEntry = stringTableVar.Table.Get(activeLanguage);
                    if (languageEntry.Type == DataType.String)
                    {
                        return languageEntry.String;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Substitutes specially formatted tokens in the text with global variables and string table values.
        /// The string table value used depends on the currently loaded string table and active language.
        /// </summary>
        public virtual string Substitute(string text)
        {
            string subbedText = text;

            // Instantiate the regular expression object.
            Regex r = new Regex("\\[\\$.*?\\]");

            // Match the regular expression pattern against a text string.
            var results = r.Matches(text);
            foreach (Match match in results)
            {
                string key = match.Value.Substring(2, match.Value.Length - 3);

                if (stringTableCached != null)
                {
                    // Match against string table and active language
                    DynValue stringTableVar = stringTableCached.Get(key);
                    if (stringTableVar.Type == DataType.Table)
                    {
                        DynValue languageEntry = stringTableVar.Table.Get(activeLanguage);
                        if (languageEntry.Type == DataType.String)
                        {
                            subbedText = subbedText.Replace(match.Value, languageEntry.String);
                        }
                        continue;
                    }
                }

                // Match against global variables
                DynValue globalVar = interpreter.Globals.Get(key);
                if (globalVar.Type != DataType.Nil)
                {
                    subbedText = subbedText.Replace(match.Value, globalVar.ToPrintString());
                    continue;
                }
            }

            return subbedText;
        }

        /// <summary>
        /// Returns the time since level load, multiplied by timeScale.
        /// If timeScale is negative then returns the same as Time.timeSinceLevelLoaded.
        /// </summary>
        public float GetTime()
        {
            if (timeScale < 0f)
            {
                return Time.timeSinceLevelLoad;
            }
            else
            {
                return Time.unscaledTime * timeScale;
            }
        }

        /// <summary>
        /// Returns the delta time this frame, multiplied by timeScale.
        /// If timeScale is negative then returns the same as Time.deltaTime.
        /// </summary>
        public float GetDeltaTime()
        {
            if (timeScale < 0f)
            {
                return Time.deltaTime;
            }
            else
            {
                return Time.deltaTime * timeScale;
            }
        }

        /// <summary>
        /// Load and run a string containing Lua script. May be run as a coroutine.
        /// <param name="luaString">The Lua code to be run.</param>
        /// <param name="friendlyName">A descriptive name to be used in error reports.</param>
        /// <param name="runAsCoroutine">Run the Lua code as a coroutine to support asynchronous operations.</param>
        /// <param name="onComplete">Method to callback when the Lua code finishes exection. Supports return parameters.</param>
        /// </summary>
        public void DoLuaString(string luaString, string friendlyName, bool runAsCoroutine, Action<DynValue> onComplete = null)
        {
            InitInterpreter();

            // Load the Lua script
            DynValue res = null;
            try
            {
                res = interpreter.LoadString(luaString, null, friendlyName);
            }
            catch (InterpreterException ex)
            {
                UnityEngine.Debug.LogError(ex.DecoratedMessage + "\n" + luaString);
            }

            if (res == null)
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
                StartCoroutine(RunLuaCoroutineInternal(res.Function, luaString, onComplete));
            }
            else
            {
                DynValue returnValue = null;
                try
                {                
                    returnValue = res.Function.Call();                
                }
                catch (InterpreterException ex)
                {
                    UnityEngine.Debug.LogError(ex.DecoratedMessage + "\n" + luaString);
                }

                if (onComplete != null)
                {
                    onComplete(returnValue);
                }
            }
        }

        /// <summary>
        /// Starts a Unity coroutine which updates a Lua coroutine each frame.
        /// <param name="closure">A MoonSharp closure object representing a function.</param>
        /// <param name="debugInfo">Debug text to display if an exception occurs (usually the Lua code that is being executed).</param>
        /// <param name="onComplete">A delegate method that is called when the coroutine completes. Includes return parameter.</param>
        /// </summary>
        public void RunLuaCoroutine(Closure closure, string debugInfo, Action<DynValue> onComplete = null)
        {
            StartCoroutine(RunLuaCoroutineInternal(closure, debugInfo, onComplete));
        }
            
        /// <summary>
        /// A Unity coroutine method which updates a Lua coroutine each frame.
        /// <param name="closure">A MoonSharp closure object representing a function.</param>
        /// <param name="debugInfo">Debug text to display if an exception occurs (usually the Lua code that is being executed).</param>
        /// <param name="onComplete">A delegate method that is called when the coroutine completes. Includes return parameter.</param>
        /// </summary>
        protected IEnumerator RunLuaCoroutineInternal(Closure closure, string debugInfo, Action<DynValue> onComplete = null)
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
                    UnityEngine.Debug.LogError(ex.DecoratedMessage + "\n" + debugInfo);
                }

                yield return null;
            }

            if (onComplete != null)
            {
                onComplete(returnValue);
            }
        }

        /// <summary>
        /// Start a Unity coroutine from a Lua call.
        /// </summary>
        public Task RunUnityCoroutine(IEnumerator coroutine)
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
        /// The coroutine is managed by the FungusScript monobehavior, so you can call StopAllCoroutines to
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
        }
    }

}