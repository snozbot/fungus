// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

namespace Fungus
{
    /// <summary>
    /// Wrapper for a MoonSharp Lua Script instance.
    /// A debug server is started automatically when running in the Unity Editor. Use VS Code to debug Lua scripts.
    /// </summary>
    public class LuaEnvironment : MonoBehaviour
    {
        [Tooltip("Start a Lua debug server on scene start.")]
        [SerializeField] protected bool startDebugServer = true;

        [Tooltip("Port to use for the Lua debug server.")]
        [SerializeField] protected int debugServerPort = 41912;

        /// <summary>
        /// The MoonSharp interpreter instance.
        /// </summary>
        protected Script interpreter;

        /// <summary>
        /// Flag used to avoid startup dependency issues.
        /// </summary>
        protected bool initialised = false;

        protected virtual void Start() 
        {
            InitEnvironment();
        }

        /// <summary>
        /// Detach the MoonSharp script from the debugger.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (DebugServer != null)
            {
                DebugServer.Detach(interpreter);
            }            
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

        /// <summary>
        /// Writes a MoonSharp exception to the debug log in a helpful format.
        /// </summary>
        /// <param name="decoratedMessage">Decorated message from a MoonSharp exception</param>
        /// <param name="debugInfo">Debug info, usually the Lua script that was running.</param>
        protected static void LogException(string decoratedMessage, string debugInfo)
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

        #region Public members

        /// <summary>
        /// Instance of VS Code debug server when debugging option is enabled.
        /// </summary>
        public static MoonSharpVsCodeDebugServer DebugServer { get; private set; }

        /// <summary>
        /// Returns the first Lua Environment found in the scene, or creates one if none exists.
        /// This is a slow operation, call it once at startup and cache the returned value.
        /// </summary>
        public static LuaEnvironment GetLua()
        {
            var luaEnv = GameObject.FindObjectOfType<LuaEnvironment>();
            if (luaEnv == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/LuaEnvironment");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.name = "LuaEnvironment";
                    luaEnv = go.GetComponent<LuaEnvironment>();
                }
            }
            return luaEnv;
        }

        /// <summary>
        /// Register a type given it's assembly qualified name.
        /// </summary>
        public static void RegisterType(string typeName, bool extensionType = false)
        {
            System.Type t = null;
            try
            {
                t = System.Type.GetType(typeName);
            }
            catch
            {}

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
            LuaEnvironmentInitializer[] initializers = GetComponents<LuaEnvironmentInitializer>();
            foreach (LuaEnvironmentInitializer initializer in initializers)
            {
                initializer.Initialize();
            }

            //
            // Change this to #if UNITY_STANDALONE if you want to debug a standalone build.
            //
            #if UNITY_EDITOR
            if (startDebugServer &&
                DebugServer == null)
            {
                // Create the debugger server
                DebugServer = new MoonSharpVsCodeDebugServer(debugServerPort);

                // Start the debugger server
                DebugServer.Start();

                // Attach the MoonSharp script to the debugger
                DebugServer.AttachToScript(interpreter, gameObject.name);
            }

            #endif

            initialised = true;
        }

        /// <summary>
        /// The MoonSharp interpreter instance used to run Lua code.
        /// </summary>
        public virtual Script Interpreter { get { return interpreter; } }

        /// <summary>
        /// Loads and compiles a string containing Lua script, returning a closure (Lua function) which can be executed later.
        /// <param name="luaString">The Lua code to be run.</param>
        /// <param name="friendlyName">A descriptive name to be used in error reports.</param>
        /// </summary>
        public virtual Closure LoadLuaFunction(string luaString, string friendlyName)
        {
            InitEnvironment();

            string processedString;
            var initializer = GetComponent<LuaEnvironmentInitializer>();
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
            Closure fn = LoadLuaFunction(luaString, friendlyName);
            RunLuaFunction(fn, runAsCoroutine, onComplete);
        }

        #endregion
    }
}