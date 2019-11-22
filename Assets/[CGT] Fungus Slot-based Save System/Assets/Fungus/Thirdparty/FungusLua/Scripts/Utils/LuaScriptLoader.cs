// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using MoonSharp.Interpreter.Loaders;
using System.Collections.Generic;
using System;
using MoonSharp.Interpreter;

namespace Fungus
{
    /// <summary>
    /// Custom file loader for MoonSharp that loads in all Lua scripts in the project.
    /// Scripts must be placed in a Resources/Lua directory.
    /// </summary>
    public class LuaScriptLoader : ScriptLoaderBase/// 
    {
        // Give the script loader access to the list of accessible Lua Modules.
        protected IEnumerable<TextAsset> luaScripts;

        /// <summary>
        // Bypasses the standard path resolution logic for require.
        /// </summary>
        protected override string ResolveModuleName(string modname, string[] paths)
        {
            return modname;
        }

        #region Public members

        public LuaScriptLoader(IEnumerable<TextAsset> luaScripts)
        {
            this.luaScripts = luaScripts;
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

        #endregion
    }
}