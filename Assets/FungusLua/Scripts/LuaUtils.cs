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

	public class LuaUtils : LuaEnvironment.Initializer, StringSubstituter.ISubstitutionHandler
    {
		public enum FungusModuleOptions
		{
			UseGlobalVariables,	// Fungus helper items will be available as global variables.
			UseFungusVariable,	// Fungus helper items will be available in the 'fungus' global variable.
			NoFungusModule		// The fungus helper module will not be loaded.
		}

		/// <summary>
		/// Controls if the fungus utilities are accessed from globals (e.g. say) or via a fungus variable (e.g. fungus.say)"
		/// You can also choose to disable loading the fungus module if it's not required by your script.
		/// </summary>
		[Tooltip("Controls if the fungus utilities are accessed from globals (e.g. say) or via a fungus variable (e.g. fungus.say)")]
		public FungusModuleOptions fungusModule = FungusModuleOptions.UseGlobalVariables;

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
        /// This allows pausing of time based operations by setting timescale to 0.
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
        /// Flag used to avoid startup dependency issues.
        /// </summary>
        protected bool initialised = false;

        /// <summary>
        /// Cached reference to the string table (if loaded).
        /// </summary>
        protected Table stringTableCached;

		/// <summary>
		/// Cached reference to the Lua Environment component.
		/// </summary>
		protected LuaEnvironment luaEnvironment;

		protected StringSubstituter stringSubstituter;

		/// <summary>
		/// Called by LuaEnvironment when initializing.
		/// </summary>
		public override void Initialize()
        {   
			luaEnvironment = GetComponent<LuaEnvironment>();
			if (luaEnvironment == null)
			{
				UnityEngine.Debug.LogError("No Lua Environment found");
				return;
			}

			if (luaEnvironment.Interpreter == null)
			{
				UnityEngine.Debug.LogError("No Lua interpreter found");
				return;
			}

			InitTypes();
            InitFungusModule();
			InitBindings();
        }

		/// <summary>
		/// Registers all listed c# types for interop with Lua.
		/// You can also register types directly in the Awake method of any 
		/// monobehavior in your scene using UserData.RegisterType().
		/// </summary>
		protected virtual void InitTypes()
		{
			bool isFungusInstalled = (Type.GetType("Fungus.Flowchart") != null);

			foreach (TextAsset textFile in registerTypes)
			{
				if (textFile == null)
				{
					continue;
				}

				char[] separators = { '\r', '\n' };
				foreach (string typeName in textFile.text.Split(separators, StringSplitOptions.RemoveEmptyEntries))
				{
					// Skip comments and empty lines
					if (typeName.StartsWith("#") || typeName.Trim() == "")
					{
						continue;
					}

					// Don't register fungus types if the Fungus library is not present
					if (!isFungusInstalled &&
						typeName.StartsWith("Fungus."))
					{
						continue;
					}
						
					LuaEnvironment.RegisterType(typeName);
				}
			}
		}

        /// <summary>
        /// Binds all gameobjects and components defined in scene LuaBindings to the global table.
        /// </summary>
        protected virtual void InitBindings()
        {
			MoonSharp.Interpreter.Script interpreter = luaEnvironment.Interpreter;

			LuaBindingsBase[] bindings = GameObject.FindObjectsOfType<LuaBindingsBase>();
			foreach (LuaBindingsBase binding in bindings)
            {
                binding.AddBindings(interpreter.Globals);
            }
        }

		/// <summary>
		/// Register some commonly used Unity classes and objects for Lua interop.
		/// To register more class objects externally to this class, register them in the Awake method of any 
		/// monobehavior in your scene.
		/// </summary>
		protected virtual void InitFungusModule()
		{
			if (fungusModule == FungusModuleOptions.NoFungusModule)
			{
				return;
			}

			MoonSharp.Interpreter.Script interpreter = luaEnvironment.Interpreter;

			// Require the Fungus module and assign it to the global 'fungus'
			Table fungusTable = null;
			MoonSharp.Interpreter.DynValue value = interpreter.RequireModule("fungus");
			if (value != null &&
				value.Type == DataType.Function)
			{
				fungusTable = value.Function.Call().Table;
			}
			if (fungusTable == null)
			{
				UnityEngine.Debug.LogError("Failed to create Fungus table");
				return;
			}
			interpreter.Globals["fungus"] = fungusTable;

			// Static classes
			fungusTable["time"] = UserData.CreateStatic(typeof(Time));
			fungusTable["prefs"] = UserData.CreateStatic(typeof(FungusPrefs));
			fungusTable["factory"] = UserData.CreateStatic(typeof(PODTypeFactory));

			// Lua Environment and Lua Utils components
			fungusTable["luaenvironment"] = luaEnvironment;
			fungusTable["luautils"] = this;

			// Provide access to the Unity Test Tools (if available).
			Type testType = Type.GetType("IntegrationTest");
			if (testType != null)
			{
				UserData.RegisterType(testType);
				fungusTable["test"] = UserData.CreateStatic(testType);
			}

			// Register the stringtable (if one is set)
			if (stringTable != null)
			{
				try
				{                
					DynValue stringTableRes = interpreter.DoString(stringTable.text);
					if (stringTableRes.Type == DataType.Table)
					{
						stringTableCached = stringTableRes.Table;
						if (fungusTable != null)
						{
							fungusTable["stringtable"] = stringTableCached;
						}
					}
				}
				catch (ScriptRuntimeException ex)
				{
					LuaEnvironment.LogException(ex.DecoratedMessage, stringTable.text);
				}
				catch (InterpreterException ex)
				{
					LuaEnvironment.LogException(ex.DecoratedMessage, stringTable.text);
				}
			}

			stringSubstituter = new StringSubstituter();

			if (fungusModule == FungusModuleOptions.UseGlobalVariables)
			{				
				// Copy all items from the Fungus table to global variables
				foreach (TablePair p in fungusTable.Pairs)
				{
					if (interpreter.Globals.Keys.Contains(p.Key))
					{
						UnityEngine.Debug.LogError("Lua globals already contains a variable " + p.Key);
					}
					else
					{
						interpreter.Globals[p.Key] = p.Value;
					}
				}

				interpreter.Globals["fungus"] = DynValue.Nil;

				// Note: We can't remove the fungus table itself because of dependencies between functions
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
		/// Implementation of StringSubstituter.ISubstitutionHandler
        /// Substitutes specially formatted tokens in the text with global variables and string table values.
        /// The string table value used depends on the currently loaded string table and active language.
        /// </summary>
		[MoonSharpHidden]
		public virtual string SubstituteStrings(string input)
        {
			// This method could be called from the Start of another component, so
			// we need to ensure that the LuaEnvironment has been initialized.
			if (luaEnvironment == null)
			{
				luaEnvironment = GetComponent<LuaEnvironment>();
				if (luaEnvironment != null)
				{
					luaEnvironment.InitEnvironment();
				}
			}
					
			if (luaEnvironment == null)
				{
				UnityEngine.Debug.LogError("No Lua Environment found");
				return input;
			}

			if (luaEnvironment.Interpreter == null)
			{
				UnityEngine.Debug.LogError("No Lua interpreter found");
				return input;
			}
				
			MoonSharp.Interpreter.Script interpreter = luaEnvironment.Interpreter;

            string subbedText = input;

            // Instantiate the regular expression object.
			Regex r = new Regex("\\{\\$.*?\\}");

            // Match the regular expression pattern against a text string.
			var results = r.Matches(input);
            foreach (Match match in results)
            {
                string key = match.Value.Substring(2, match.Value.Length - 3);

				// Match against string table and active language (if specified)
				if (stringTableCached != null)
				{
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
		/// Performs string substitution on the input string, replacing tokens of the form {$VarName} with 
		/// matching variables, localised strings, etc. in the scene.
		/// </summary>
		public virtual string Substitute(string input)
		{
			return stringSubstituter.SubstituteStrings(input);
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
		/// Find a game object by name and returns it.
		/// </summary>
		public virtual GameObject Find(string name)
		{
			return GameObject.Find(name);
		}

		/// <summary>
		/// Returns one active GameObject tagged tag. Returns null if no GameObject was found.
		/// </summary>
		public virtual GameObject FindWithTag(string tag)
		{
			return GameObject.FindGameObjectWithTag(tag);
		}

		/// <summary>
		/// Returns a list of active GameObjects tagged tag. Returns empty array if no GameObject was found.
		/// </summary>
		public virtual GameObject[] FindGameObjectsWithTag(string tag)
		{
			return GameObject.FindGameObjectsWithTag(tag);
		}
			
		/// <summary>
		/// Create a copy of a GameObject.
		/// Can be used to instantiate prefabs.
		/// </summary>
		public virtual GameObject Instantiate(GameObject go)
		{
			return GameObject.Instantiate(go);
		}

		/// <summary>
		/// Destroys an instance of a GameObject.
		/// </summary>
		public virtual void Destroy(GameObject go)
		{
			GameObject.Destroy(go);
		}

		/// <summary>
		/// Spawns an instance of a named prefab resource.
		/// The prefab must exist in a Resources folder in the project.
		/// </summary>
		public virtual GameObject Spawn(string resourceName)
		{
			// Auto spawn a say dialog object from the prefab
			GameObject prefab = Resources.Load<GameObject>(resourceName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab) as GameObject;
				go.name = resourceName;
				return go;
			}
			return null;
		}
   }

}