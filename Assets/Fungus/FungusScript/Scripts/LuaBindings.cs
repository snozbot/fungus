using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Fungus
{

	/// <summary>
	/// Base class for a component which registers Lua Bindings.
	/// When FungusScript initialises, it finds all components in the scene that inherit
	/// from LuaBindingsBase and calls them to add their bindings.
	/// </summary>
	public abstract class LuaBindingsBase : MonoBehaviour
	{
		/// <summary>
		/// Add all declared bindings to the globals table.
		/// </summary>
		public abstract void AddBindings(Table globals);
	}

    /// <summary>
    /// Component which manages a list of bound objects to be accessed in Lua scripts.
    /// </summary>
    [ExecuteInEditMode]
	public class LuaBindings : LuaBindingsBase 
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
        /// Name of global table variable to store bindings in. If left blank then each binding will be added as a global variable.
        /// </summary>
        [Tooltip("Name of global table variable to store bindings in. If left blank then each binding will be added as a global variable.")]
        public string tableName = "";

        /// <summary>
        /// The list of Unity objects to be bound for access in Lua.
        /// </summary>
        [Tooltip("The list of Unity objects to be bound to make them accessible in Lua script.")]
        public List<BoundObject> boundObjects = new List<BoundObject>();

		public List<string> boundTypes = new List<string>();

        /// <summary>
        /// Always ensure there is at least one row in the bound objects list.
        /// </summary>
        protected virtual void Update() 
        {
            // Add in a single empty line at start
            if (boundObjects.Count == 0)
            {
                boundObjects.Add(null);
            }
        }

        /// <summary>
        /// Add all declared bindings to the globals table.
        /// </summary>
        public override void AddBindings(Table globals)
        {
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

			// Register types of bound object with MoonSharp
			foreach (string typeName in boundTypes)
			{
				FungusScript.RegisterType(typeName);
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
    }

}