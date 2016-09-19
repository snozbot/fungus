// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Represents a single Unity object (+ optional component) bound to a string key.
    /// </summary>
    [Serializable]
    public class BoundObject
    {
        public string key;
        public UnityEngine.Object obj;
        public Component component;
    }

    /// <summary>
    /// Binds objects to identifiers in a Lua Environment.
    /// </summary>
    public interface ILuaBindings
    {
        /// <summary>
        /// Add all declared bindings to the globals table.
        /// </summary>
        void AddBindings(ILuaEnvironment luaEnv);

        /// <summary>
        /// The list of objects to be bound to Lua.
        /// </summary>
        List<BoundObject> BoundObjects { get; }
    }
}