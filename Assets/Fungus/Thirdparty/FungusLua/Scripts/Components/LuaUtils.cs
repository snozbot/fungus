// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using Fungus.Utils;

namespace Fungus
{
    /// <summary>
    /// A collection of utilites to use in Lua for common Unity / Fungus tasks.
    /// </summary>
    public class LuaUtils : LuaEnvironmentInitializer, ISubstitutionHandler, ILuaUtils
    {
        [Tooltip("Controls if the fungus utilities are accessed from globals (e.g. say) or via a fungus variable (e.g. fungus.say). You can also choose to disable loading the fungus module if it's not required by your script.")]
        [SerializeField] protected FungusModuleOptions fungusModule = FungusModuleOptions.UseGlobalVariables;

        [Tooltip("The currently selected language in the string table. Affects variable substitution.")]
        [SerializeField] protected string activeLanguage = "en";

        /// <summary>
        /// Lua script file which defines the global string table used for localisation.
        /// </summary>
        [HideInInspector]
        [Tooltip("List of JSON text files which contain localized strings. These strings are added to the 'stringTable' table in the Lua environment at startup.")]
        [SerializeField] protected List<TextAsset> stringTables = new List<TextAsset>();

        /// <summary>
        /// JSON text files listing the c# types that can be accessed from Lua.
        /// </summary>
        [HideInInspector]
        [Tooltip("JSON text files listing the c# types that can be accessed from Lua.")]
        [SerializeField] protected List<TextAsset> registerTypes = new List<TextAsset>();

        /// <summary>
        /// Flag used to avoid startup dependency issues.
        /// </summary>
        protected bool initialised = false;

        /// <summary>
        /// Cached reference to the string table (if loaded).
        /// </summary>
        protected Table stringTable;

        /// <summary>
        /// Cached reference to the Lua Environment component.
        /// </summary>
        protected ILuaEnvironment LuaEnv { get; set; }

        protected StringSubstituter stringSubstituter;

        protected ConversationManager conversationManager;
        
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

                // Parse JSON file
                JSONObject jsonObject = new JSONObject(textFile.text);
                if (jsonObject == null ||
                    jsonObject.type != JSONObject.Type.OBJECT)
                {
                    UnityEngine.Debug.LogError("Error parsing JSON file " + textFile.name);
                    continue;
                }

                // Register types with MoonSharp
                JSONObject registerTypesArray = jsonObject.GetField("registerTypes");
                if (registerTypesArray != null &&
                    registerTypesArray.type == JSONObject.Type.ARRAY)
                {
                    foreach (JSONObject entry in registerTypesArray.list)
                    {
                        if (entry != null &&
                            entry.type == JSONObject.Type.STRING)
                        {
                            string typeName = entry.str.Trim();

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

                // Register extension types with MoonSharp
                JSONObject extensionTypesArray = jsonObject.GetField("extensionTypes");
                if (extensionTypesArray != null &&
                    extensionTypesArray.type == JSONObject.Type.ARRAY)
                {
                    foreach (JSONObject entry in extensionTypesArray.list)
                    {
                        if (entry != null &&
                            entry.type == JSONObject.Type.STRING)
                        {
                            string typeName = entry.str.Trim();

                            // Don't register fungus types if the Fungus library is not present
                            if (!isFungusInstalled &&
                                typeName.StartsWith("Fungus."))
                            {
                                continue;
                            }

                            LuaEnvironment.RegisterType(typeName, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Binds all gameobjects and components defined in LuaBindings components to LuaEnvironments.
        /// </summary>
        protected virtual void InitBindings()
        {
            LuaBindingsBase[] bindings = GameObject.FindObjectsOfType<LuaBindingsBase>();
            foreach (LuaBindingsBase binding in bindings)
            {
                binding.AddBindings(LuaEnv);
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

            MoonSharp.Interpreter.Script interpreter = LuaEnv.Interpreter;

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
            fungusTable["playerprefs"] = UserData.CreateStatic(typeof(PlayerPrefs));
            fungusTable["prefs"] = UserData.CreateStatic(typeof(FungusPrefs));
            fungusTable["factory"] = UserData.CreateStatic(typeof(PODTypeFactory));

            // Lua Environment and Lua Utils components
            fungusTable["luaenvironment"] = LuaEnv;
            fungusTable["luautils"] = this;

            // Provide access to the Unity Test Tools (if available).
            Type testType = Type.GetType("IntegrationTest");
            if (testType != null)
            {
                UserData.RegisterType(testType);
                fungusTable["test"] = UserData.CreateStatic(testType);
            }

            // Populate the string table by parsing the string table JSON files
            stringTable = new Table(interpreter); 
            fungusTable["stringtable"] = stringTable;
            foreach (TextAsset stringFile in stringTables)
            {
                if (stringFile.text == "")
                {
                    continue;
                }

                JSONObject stringsJSON = new JSONObject(stringFile.text);
                if (stringsJSON == null ||
                    stringsJSON.type != JSONObject.Type.OBJECT)
                {
                    UnityEngine.Debug.LogError("String table JSON format is not correct " + stringFile.name);
                    continue;
                }

                foreach (string stringKey in stringsJSON.keys)
                {
                    if (stringKey == "")
                    {
                        UnityEngine.Debug.LogError("String table JSON format is not correct " + stringFile.name);
                        continue;
                    }

                    Table entriesTable = new Table(interpreter);
                    stringTable[stringKey] = entriesTable;

                    JSONObject entries = stringsJSON.GetField(stringKey);
                    if (entries.type != JSONObject.Type.OBJECT)
                    {
                        UnityEngine.Debug.LogError("String table JSON format is not correct " + stringFile.name);
                        continue;
                    }

                    foreach (string language in entries.keys)
                    {
                        string translation = entries.GetField(language).str;
                        entriesTable[language] = translation;
                    }
                }
            }

            stringSubstituter = new StringSubstituter();
            stringSubstituter.CacheSubstitutionHandlers();

            conversationManager = new ConversationManager();
            conversationManager.PopulateCharacterCache();

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

        #region LuaEnvironment.Initializer implementation

        public override void Initialize()
        {   
            LuaEnv = GetComponent<ILuaEnvironment>();
            if (LuaEnv == null)
            {
                Debug.LogError("No Lua Environment found");
                return;
            }

            if (LuaEnv.Interpreter == null)
            {
                Debug.LogError("No Lua interpreter found");
                return;
            }

            InitTypes();
            InitFungusModule();
            InitBindings();
        }

        public override string PreprocessScript(string input)
        {
            return input;
        }

        #endregion

        #region ILuaUtils implementation

        public virtual string ActiveLanguage { get { return activeLanguage; } set { activeLanguage = value; } }

        public virtual string GetString(string key)
        {
            if (stringTable != null)
            {
                // Match against string table and active language
                DynValue stringTableVar = stringTable.Get(key);
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

        [MoonSharpHidden]
        public virtual bool SubstituteStrings(StringBuilder input)
        {
            // This method could be called from the Start of another component, so
            // we need to ensure that the LuaEnvironment has been initialized.
            if (LuaEnv == null)
            {
                LuaEnv = GetComponent<ILuaEnvironment>();
                if (LuaEnv != null)
                {
                    LuaEnv.InitEnvironment();
                }
            }
                    
            if (LuaEnv == null)
            {
                UnityEngine.Debug.LogError("No Lua Environment found");
                return false;
            }

            if (LuaEnv.Interpreter == null)
            {
                UnityEngine.Debug.LogError("No Lua interpreter found");
                return false;
            }
                
            MoonSharp.Interpreter.Script interpreter = LuaEnv.Interpreter;

            // Instantiate the regular expression object.
            Regex r = new Regex("\\{\\$.*?\\}");

            bool modified = false;

            // Match the regular expression pattern against a text string.
            var results = r.Matches(input.ToString());
            foreach (Match match in results)
            {
                string key = match.Value.Substring(2, match.Value.Length - 3);

                // Match against string table and active language (if specified)
                if (stringTable != null)
                {
                    DynValue stringTableVar = stringTable.Get(key);
                    if (stringTableVar.Type == DataType.Table)
                    {
                        DynValue languageEntry = stringTableVar.Table.Get(activeLanguage);
                        if (languageEntry.Type == DataType.String)
                        {
                            input.Replace(match.Value, languageEntry.String);
                            modified = true;
                        }
                        continue;
                    }
                }

                // Match against global variables
                DynValue globalVar = interpreter.Globals.Get(key);
                if (globalVar.Type != DataType.Nil)
                {
                    input.Replace(match.Value, globalVar.ToPrintString());
                    modified = true;
                    continue;
                }
            }

            return modified;
        }

        public virtual string Substitute(string input)
        {
            return stringSubstituter.SubstituteStrings(input);
        }
            
        public virtual GameObject Find(string name)
        {
            return GameObject.Find(name);
        }

        public virtual GameObject FindWithTag(string tag)
        {
            return GameObject.FindGameObjectWithTag(tag);
        }

        public virtual GameObject[] FindGameObjectsWithTag(string tag)
        {
            return GameObject.FindGameObjectsWithTag(tag);
        }
            
        public virtual GameObject Instantiate(GameObject go)
        {
            return GameObject.Instantiate(go);
        }

        public virtual void Destroy(GameObject go)
        {
            GameObject.Destroy(go);
        }

        public virtual GameObject Spawn(string resourceName)
        {
            // Auto spawn a say dialog object from the prefab
            GameObject prefab = Resources.Load<GameObject>(resourceName);
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab) as GameObject;
                go.name = resourceName.Replace("Prefabs/", "");
                return go;
            }
            return null;
        }

        public virtual IEnumerator DoConversation(string conv)
        {
            return conversationManager.DoConversation(conv);
        }

        public void SetSayDialog(ISayDialog sayDialog)
        {
            SayDialog.activeSayDialog = sayDialog;
        }

        #endregion
   }
}