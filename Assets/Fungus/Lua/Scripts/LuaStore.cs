using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;

namespace Fungus
{

	/// <summary>
	/// Wrapper for a prime Lua table that persists across scene loads. 
	/// This is useful for transferring values from one scene to another. One one LuaStore component may exist 
	/// in a scene at a time.
	/// </summary>
	public class LuaStore : LuaBindingsBase 
	{
		/// <summary>
		/// A Lua table that can be shared between multiple LuaEnvironments.
		/// </summary>
		public Table primeTable;

		protected bool initialized;

		protected static LuaStore instance;

		public void Start()
		{
			Init();
		}

		/// <summary>
		/// Initialize the LuaStore component.
		/// This component behaves somewhat like a singleton in that only one instance
		/// is permitted in the application which persists until shutdown.
		/// </summary>
		protected virtual bool Init()
		{
			if (initialized)	
			{
				return true;
			}

			if (instance == null)
			{
				// This is the first instance of the LuaStore, so store a static reference to it.
				instance = this;
			}
			else if (instance != this)
			{
				// This is an extra instance of LuaStore. We only need one in the scene, so delete this one.
				Destroy(gameObject);
				return false;
			}

			// We're now guaranteed that this instance of LuaStore is the first and only instance.

			primeTable = DynValue.NewPrimeTable().Table;

			// DontDestroyOnLoad only works for root objects
			transform.parent = null;

			DontDestroyOnLoad(this);

			initialized = true;

			return true;
		}

		/// <summary>
		/// Callback to bind this LuaStore component with the "unity" table in a LuaEnvironment component.
		/// </summary>
		public override void AddBindings(Table globals)
		{
			if (!Init())
			{
				return;
			}

			if (globals == null)
			{
				Debug.LogError("Lua globals table is null");
				return;
			}

			Table fungusTable = globals.Get("fungus").Table;
			if (fungusTable == null)
			{
				Debug.LogError("fungus table not found");
				return;
			}

			fungusTable["store"] = primeTable;

			// If we're using the fungus module in globals mode then add the store to globals as well.
			if (globals.Get("luaenvironment") != DynValue.Nil)
			{
				globals["store"] = primeTable;
			}
		}
	}

}