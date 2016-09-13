using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Binds objects to identifiers in a Lua Environment.
    /// </summary>
    public interface ILuaBindings
    {
        /// <summary>
        /// Add all declared bindings to the globals table.
        /// </summary>
        void AddBindings(ILuaEnvironment luaEnv);
    }
}