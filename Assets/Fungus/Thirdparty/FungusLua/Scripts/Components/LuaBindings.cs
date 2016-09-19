// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Fungus
{
    /// <summary>
    /// Base class for a component which registers Lua Bindings.
    /// When the Lua Environment initialises, it finds all components in the scene that inherit
    /// from LuaBindingsBase and calls them to add their bindings.
    /// </summary>
    public abstract class LuaBindingsBase : MonoBehaviour, ILuaBindings
    {
        public abstract void AddBindings(ILuaEnvironment luaEnv);

        public abstract List<BoundObject> BoundObjects { get; }
    }

    /// <summary>
    /// Component which manages a list of bound objects to be accessed in Lua scripts.
    /// </summary>
    [ExecuteInEditMode]
    public class LuaBindings : LuaBindingsBase 
    {
        [Tooltip("Add bindings to every Lua Environment in the scene. If false, only add bindings to a specific Lua Environment.")]
        [SerializeField] protected bool allEnvironments = true;

        [Tooltip("The specific LuaEnvironment to register the bindings in.")]
        [SerializeField] protected LuaEnvironment luaEnvironment;

        [Tooltip("Name of global table variable to store bindings in. If left blank then each binding will be added as a global variable.")]
        [SerializeField] protected string tableName = "";

        [Tooltip("Register all CLR types used by the bound objects so that they can be accessed from Lua. If you don't use this option you will need to register these types yourself.")]
        [SerializeField] protected bool registerTypes = true;

        [HideInInspector]
        [SerializeField] protected List<string> boundTypes = new List<string>();

        [Tooltip("The list of Unity objects to be bound to make them accessible in Lua script.")]
        [SerializeField] protected List<BoundObject> boundObjects = new List<BoundObject>();

        [Tooltip("Show inherited public members.")]
        [SerializeField] protected bool showInherited;

        protected virtual void Update() 
        {
            // Add in a single empty line at start
            if (boundObjects.Count == 0)
            {
                boundObjects.Add(null);
            }
        }

        #region ILuaBindings implementation

        public override void AddBindings(ILuaEnvironment luaEnv)
        {
            if (!allEnvironments && 
                (luaEnvironment != null && !luaEnvironment.Equals(luaEnv)))
            {
                // Don't add bindings to this environment
                return;
            }

            MoonSharp.Interpreter.Script interpreter = luaEnv.Interpreter;
            Table globals = interpreter.Globals;

            Table bindingsTable = null;
            if (tableName == "")
            {
                // Add directly to globals table
                bindingsTable = globals;
            }
            else
            {
                DynValue res = globals.Get(tableName);
                if (res.Type == DataType.Table)
                {
                    // Add to an existing table
                    bindingsTable = res.Table;
                }
                else
                {
                    // Create a new table
                    bindingsTable = new Table(globals.OwnerScript);
                    globals[tableName] = bindingsTable; 
                }
            }

            if (bindingsTable == null)
            {
                Debug.LogError("Bindings table must not be null");
            }

            // Register types of bound objects with MoonSharp
            if (registerTypes)
            {
                foreach (string typeName in boundTypes)
                {
                    LuaEnvironment.RegisterType(typeName);
                }
            }

            for (int i = 0; i < boundObjects.Count; ++i)
            {
                // Ignore empty keys
                string key = boundObjects[i].key;
                if (key == null ||
                    key == "")
                {
                    continue;
                }

                // Check for keys used multiple times
                if (bindingsTable.Get(key).Type != DataType.Nil)
                {
                    Debug.LogWarning("An item already exists with the same key as binding '" + key + "'. This binding will be ignored.");
                    continue;
                }

                // Ignore bindings with no object set
                GameObject go = boundObjects[i].obj as GameObject;
                if (go != null)
                {
                    // Register as gameobject / components
                    Component component = boundObjects[i].component;
                    if (component == null)
                    {
                        // Bind the key to the gameobject
                        bindingsTable[key] = go;
                    }
                    else
                    {
                        // Bind the key to the component
                        bindingsTable[key] = component;
                    }
                }
                else
                {
                    // Register as other UnityEngine.Object type
                    bindingsTable[key] = boundObjects[i].obj;
                }
            }
        }

        public override List<BoundObject> BoundObjects { get { return boundObjects; } }

        #endregion
    }
}