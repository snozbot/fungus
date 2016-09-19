// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using MoonSharp.Interpreter;

namespace Fungus
{
    /// <summary>
    /// Wrapper for a MoonSharp Lua Script instance.
    /// </summary>    
    public interface ILuaEnvironment
    {
        /// <summary>
        /// Initialise the Lua interpreter so we can start running Lua code.
        /// </summary>
        void InitEnvironment();

        /// <summary>
        /// The MoonSharp interpreter instance used to run Lua code.
        /// </summary>
        Script Interpreter { get; }

        /// <summary>
        /// Loads and compiles a string containing Lua script, returning a closure (Lua function) which can be executed later.
        /// <param name="luaString">The Lua code to be run.</param>
        /// <param name="friendlyName">A descriptive name to be used in error reports.</param>
        /// </summary>
        Closure LoadLuaFunction(string luaString, string friendlyName);

        /// <summary>
        /// Load and run a previously compiled Lua script. May be run as a coroutine.
        /// <param name="fn">A previously compiled Lua function.</param>
        /// <param name="runAsCoroutine">Run the Lua code as a coroutine to support asynchronous operations.</param>
        /// <param name="onComplete">Method to callback when the Lua code finishes exection. Supports return parameters.</param>
        /// </summary>
        void RunLuaFunction(Closure fn, bool runAsCoroutine, System.Action<DynValue> onComplete = null);

        /// <summary>
        /// Load and run a string containing Lua script. May be run as a coroutine.
        /// <param name="luaString">The Lua code to be run.</param>
        /// <param name="friendlyName">A descriptive name to be used in error reports.</param>
        /// <param name="runAsCoroutine">Run the Lua code as a coroutine to support asynchronous operations.</param>
        /// <param name="onComplete">Method to callback when the Lua code finishes exection. Supports return parameters.</param>
        /// </summary>
        void DoLuaString(string luaString, string friendlyName, bool runAsCoroutine, System.Action<DynValue> onComplete = null);
    }
}