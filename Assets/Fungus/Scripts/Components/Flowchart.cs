// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Fungus.Commands;
using Fungus.EventHandlers;
using Fungus.Utils;
using Fungus.Variables;

namespace Fungus
{
    /// <summary>
    /// Visual scripting controller for the Flowchart programming language.
    /// Flowchart objects may be edited visually using the Flowchart editor window.
    /// </summary>
    [ExecuteInEditMode]
    public class Flowchart : MonoBehaviour, IFlowchart, ISubstitutionHandler
    {
        [HideInInspector]
        [SerializeField] protected int version = 0; // Default to 0 to always trigger an update for older versions of Fungus.

        [HideInInspector]
        [SerializeField] protected Vector2 scrollPos;

        [HideInInspector]
        [SerializeField] protected Vector2 variablesScrollPos;

        [HideInInspector]
        [SerializeField] protected bool variablesExpanded = true;

        [HideInInspector]
        [SerializeField] protected float blockViewHeight = 400;

        [HideInInspector]
        [SerializeField] protected float zoom = 1f;

        [HideInInspector]
        [SerializeField] protected Rect scrollViewRect;

        [HideInInspector]
        [FormerlySerializedAs("selectedSequence")]
        [SerializeField] protected Block selectedBlock;

        [HideInInspector]
        [SerializeField] protected List<Command> selectedCommands = new List<Command>();

        [HideInInspector]
        [SerializeField] protected List<Variable> variables = new List<Variable>();

        [TextArea(3, 5)]
        [Tooltip("Description text displayed in the Flowchart editor window")]
        [SerializeField] protected string description = "";

        [Range(0f, 5f)]
        [Tooltip("Adds a pause after each execution step to make it easier to visualise program flow. Editor only, has no effect in platform builds.")]
        [SerializeField] protected float stepPause = 0f;

        [Tooltip("Use command color when displaying the command list in the Fungus Editor window")]
        [SerializeField] protected bool colorCommands = true;

        [Tooltip("Hides the Flowchart block and command components in the inspector. Deselect to inspect the block and command components that make up the Flowchart.")]
        [SerializeField] protected bool hideComponents = true;

        [Tooltip("Saves the selected block and commands when saving the scene. Helps avoid version control conflicts if you've only changed the active selection.")]
        [SerializeField] protected bool saveSelection = true;

        [Tooltip("Unique identifier for this flowchart in localized string keys. If no id is specified then the name of the Flowchart object will be used.")]
        [SerializeField] protected string localizationId = "";

        [Tooltip("Display line numbers in the command list in the Block inspector.")]
        [SerializeField] protected bool showLineNumbers = false;

        [Tooltip("List of commands to hide in the Add Command menu. Use this to restrict the set of commands available when editing a Flowchart.")]
        [SerializeField] protected List<string> hideCommands = new List<string>();

        [Tooltip("Lua Environment to be used by default for all Execute Lua commands in this Flowchart")]
        [SerializeField] protected LuaEnvironment luaEnvironment;

        [Tooltip("The ExecuteLua command adds a global Lua variable with this name bound to the flowchart prior to executing.")]
        [SerializeField] protected string luaBindingName = "flowchart";

        protected static bool eventSystemPresent;

        protected StringSubstituter stringSubstituer;

        /// <summary>
        /// Cached list of flowchart objects in the scene for fast lookup.
        /// </summary>
        public static List<Flowchart> cachedFlowcharts = new List<Flowchart>();

        /// <summary>
        /// Sends a message to all Flowchart objects in the current scene.
        /// Any block with a matching MessageReceived event handler will start executing.
        /// </summary>
        public static void BroadcastFungusMessage(string messageName)
        {
            MessageReceived[] eventHandlers = UnityEngine.Object.FindObjectsOfType<MessageReceived>();
            foreach (MessageReceived eventHandler in eventHandlers)
            {
                eventHandler.OnSendFungusMessage(messageName);
            }
        }

        #if UNITY_5_4_OR_NEWER
        protected virtual void Awake()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (A, B) => {
                LevelWasLoaded();
            };
        }
        #else
        protected virtual void OnLevelWasLoaded(int level) 
        {
            LevelWasLoaded();
        }
        #endif

        protected virtual void LevelWasLoaded()
        {
            // Reset the flag for checking for an event system as there may not be one in the newly loaded scene.
            eventSystemPresent = false;
        }
            
        protected virtual void Start()
        {
            CheckEventSystem();
        }

        // There must be an Event System in the scene for Say and Menu input to work.
        // This method will automatically instantiate one if none exists.
        protected virtual void CheckEventSystem()
        {
            if (eventSystemPresent)
            {
                return;
            }
            
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                // Auto spawn an Event System from the prefab
                GameObject prefab = Resources.Load<GameObject>("Prefabs/EventSystem");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.name = "EventSystem";
                }
            }
            
            eventSystemPresent = true;
        }

        protected virtual void OnEnable()
        {
            if (!cachedFlowcharts.Contains(this))
            {
                cachedFlowcharts.Add(this);
            }

            CheckItemIds();
            CleanupComponents();
            UpdateVersion();
        }

        protected virtual void UpdateVersion()
        {
            if (version == FungusConstants.CurrentVersion)
            {
                // No need to update
                return;
            }

            // Tell all components that implement IUpdateable to update to the new version
            foreach (Component component in GetComponents<Component>())
            {
                IUpdateable u = component as IUpdateable;
                if (u != null)
                {
                    u.UpdateToVersion(version, FungusConstants.CurrentVersion);
                }
            }

            version = FungusConstants.CurrentVersion;
        }

        protected virtual void OnDisable()
        {
            cachedFlowcharts.Remove(this);
        }

        protected virtual void CheckItemIds()
        {
            // Make sure item ids are unique and monotonically increasing.
            // This should always be the case, but some legacy Flowcharts may have issues.
            List<int> usedIds = new List<int>();
            var blocks = GetComponents<IBlock>();
            foreach (var block in blocks)
            {
                if (block.ItemId == -1 ||
                    usedIds.Contains(block.ItemId))
                {
                    block.ItemId = NextItemId();
                }
                usedIds.Add(block.ItemId);
            }
            
            var commands = GetComponents<ICommand>();
            foreach (var command in commands)
            {
                if (command.ItemId == -1 ||
                    usedIds.Contains(command.ItemId))
                {
                    command.ItemId = NextItemId();
                }
                usedIds.Add(command.ItemId);
            }
        }

        protected virtual void CleanupComponents()
        {
            // Delete any unreferenced components which shouldn't exist any more
            // Unreferenced components don't have any effect on the flowchart behavior, but
            // they waste memory so should be cleared out periodically.

            // Remove any null entries in the variables list
            // It shouldn't happen but it seemed to occur for a user on the forum 
            variables.RemoveAll(item => item == null);

            foreach (Variable variable in GetComponents<Variable>())
            {
                if (!variables.Contains(variable))
                {
                    DestroyImmediate(variable);
                }
            }
            
            var blocks = GetComponents<IBlock>();

            foreach (var command in GetComponents<Command>())
            {
                bool found = false;
                foreach (var block in blocks)
                {
                    if (block.CommandList.Contains(command))
                    {
                        found = true;
                        break;
                    }
                }
                
                if (!found)
                {
                    DestroyImmediate(command);
                }
            }
            
            foreach (EventHandler eventHandler in GetComponents<EventHandler>())
            {
                bool found = false;
                foreach (var block in blocks)
                {
                    if (block._EventHandler == eventHandler)
                    {
                        found = true;
                        break;
                    }
                }
                
                if (!found)
                {
                    DestroyImmediate(eventHandler);
                }
            }
        }

        protected virtual Block CreateBlockComponent(GameObject parent)
        {
            Block block = parent.AddComponent<Block>();
            return block;
        }

        #region IFlowchart implementation

        public virtual Vector2 ScrollPos { get { return scrollPos; } set { scrollPos = value; } }

        public virtual Vector2 VariablesScrollPos { get { return variablesScrollPos; } set { variablesScrollPos = value; } }

        public virtual bool VariablesExpanded { get { return variablesExpanded; } set { variablesExpanded = value; } }

        public virtual float BlockViewHeight { get { return blockViewHeight; } set { blockViewHeight = value; } }

        public virtual float Zoom { get { return zoom; } set { zoom = value; } }

        public virtual Rect ScrollViewRect { get { return scrollViewRect; } set { scrollViewRect = value; } }

        public virtual Block SelectedBlock { get { return selectedBlock; } set { selectedBlock = value; } }

        public virtual List<Command> SelectedCommands { get { return selectedCommands; } }

        public virtual List<Variable> Variables { get { return variables; } }

        public virtual string Description { get { return description; } }

        public virtual float StepPause { get { return stepPause; } }

        public virtual bool ColorCommands { get { return colorCommands; } }

        public virtual bool SaveSelection { get { return saveSelection; } }

        public virtual string LocalizationId { get { return localizationId; } }

        public virtual bool ShowLineNumbers { get { return showLineNumbers; } }

        public virtual ILuaEnvironment LuaEnv { get { return luaEnvironment; } }

        public virtual string LuaBindingName { get { return luaBindingName; } }

        public virtual Vector2 CenterPosition { set; get; }

        public int Version { set { version = value; } }

        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public int NextItemId()
        {
            int maxId = -1;
            var blocks = GetComponents<IBlock>();
            foreach (var block in blocks)
            {
                maxId = Math.Max(maxId, block.ItemId);
            }

            var commands = GetComponents<ICommand>();
            foreach (var command in commands)
            {
                maxId = Math.Max(maxId, command.ItemId);
            }
            return maxId + 1;
        }

        public virtual Block CreateBlock(Vector2 position)
        {
            Block b = CreateBlockComponent(gameObject);
            b._NodeRect = new Rect(position.x, position.y, 0, 0);
            b.BlockName = GetUniqueBlockKey(b.BlockName, b);
            b.ItemId = NextItemId();

            return b;
        }

        public virtual IBlock FindBlock(string blockName)
        {
            var blocks = GetComponents<IBlock>();
            foreach (var block in blocks)
            {
                if (block.BlockName == blockName)
                {
                    return block;
                }
            }

            return null;
        }

        public virtual void ExecuteBlock(string blockName)
        {
            var block = FindBlock(blockName);

            if (block == null)
            {
                Debug.LogError("Block " + blockName  + "does not exist");
                return;
            }

            if (!ExecuteBlock(block))
            {
                Debug.LogWarning("Block " + blockName  + "failed to execute");
            }
        }

        public virtual bool ExecuteBlock(IBlock block, int commandIndex = 0, Action onComplete = null)
        {
            if (block == null)
            {
                Debug.LogError("Block must not be null");
                return false;
            }

            if (((Block)block).gameObject != gameObject)
            {
                Debug.LogError("Block must belong to the same gameobject as this Flowchart");
                return false;                
            }

            // Can't restart a running block, have to wait until it's idle again
            if (block.IsExecuting())
            {
                return false;
            }

            // Start executing the Block as a new coroutine
            StartCoroutine(block.Execute(commandIndex, onComplete));

            return true;
        }

        public virtual void StopAllBlocks()
        {
            var blocks = GetComponents<IBlock>();
            foreach (IBlock block in blocks)
            {
                if (block.IsExecuting())
                {
                    block.Stop();
                }
            }
        }

        public virtual void SendFungusMessage(string messageName)
        {
            MessageReceived[] eventHandlers = GetComponents<MessageReceived>();
            foreach (MessageReceived eventHandler in eventHandlers)
            {
                eventHandler.OnSendFungusMessage(messageName);
            }
        }

        public virtual string GetUniqueVariableKey(string originalKey, Variable ignoreVariable = null)
        {
            int suffix = 0;
            string baseKey = originalKey;

            // Only letters and digits allowed
            char[] arr = baseKey.Where(c => (char.IsLetterOrDigit(c) || c == '_')).ToArray(); 
            baseKey = new string(arr);

            // No leading digits allowed
            baseKey = baseKey.TrimStart('0','1','2','3','4','5','6','7','8','9');

            // No empty keys allowed
            if (baseKey.Length == 0)
            {
                baseKey = "Var";
            }

            string key = baseKey;
            while (true)
            {
                bool collision = false;
                foreach(Variable variable in variables)
                {
                    if (variable == null ||
                        variable == ignoreVariable ||
                        variable.Key == null)
                    {
                        continue;
                    }

                    if (variable.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        collision = true;
                        suffix++;
                        key = baseKey + suffix;
                    }
                }

                if (!collision)
                {
                    return key;
                }
            }
        }

        public virtual string GetUniqueBlockKey(string originalKey, IBlock ignoreBlock = null)
        {
            int suffix = 0;
            string baseKey = originalKey.Trim();

            // No empty keys allowed
            if (baseKey.Length == 0)
            {
                baseKey = FungusConstants.DefaultBlockName;
            }

            var blocks = GetComponents<IBlock>();

            string key = baseKey;
            while (true)
            {
                bool collision = false;
                foreach (var block in blocks)
                {
                    if (block == ignoreBlock ||
                        block.BlockName == null)
                    {
                        continue;
                    }

                    if (block.BlockName.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        collision = true;
                        suffix++;
                        key = baseKey + suffix;
                    }
                }

                if (!collision)
                {
                    return key;
                }
            }
        }

        public virtual string GetUniqueLabelKey(string originalKey, Label ignoreLabel)
        {
            int suffix = 0;
            string baseKey = originalKey.Trim();

            // No empty keys allowed
            if (baseKey.Length == 0)
            {
                baseKey = "New Label";
            }

            IBlock block = ignoreLabel.ParentBlock;

            string key = baseKey;
            while (true)
            {
                bool collision = false;
                foreach (var command in block.CommandList)
                {
                    Label label = command as Label;
                    if (label == null ||
                        label == ignoreLabel)
                    {
                        continue;
                    }

                    if (label.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        collision = true;
                        suffix++;
                        key = baseKey + suffix;
                    }
                }

                if (!collision)
                {
                    return key;
                }
            }
        }

        public Variable GetVariable(string key)
        {
            foreach (Variable variable in variables)
            {
                if (variable != null && variable.Key == key)
                {
                    return variable;
                }
            }

            return null;
        }

        public T GetVariable<T>(string key) where T : Variable
        {
            foreach (Variable variable in variables)
            {
                if (variable != null && variable.Key == key)
                {
                    return variable as T;
                }
            }

            Debug.LogWarning("Variable " + key + " not found.");
            return null;
        }

        public void SetVariable<T>(string key, T newvariable) where T : Variable
        {
            foreach (Variable v in variables)
            {
                if (v != null && v.Key == key)
                {
                    T variable = v as T;
                    if (variable != null)
                    {
                        variable = newvariable;
                        return;
                    }
                }
            }

            Debug.LogWarning("Variable " + key + " not found.");
        }

        public virtual List<Variable> GetPublicVariables()
        {
            List<Variable> publicVariables = new List<Variable>();
            foreach (Variable v in variables)
            {
                if (v != null && v.Scope == VariableScope.Public)
                {
                    publicVariables.Add(v);
                }
            }

            return publicVariables;
        }

        public virtual bool GetBooleanVariable(string key)
        {
            BooleanVariable variable = GetVariable<BooleanVariable>(key);

            if(variable != null)
            {
                return GetVariable<BooleanVariable>(key).Value;
            }
            else
            {
                return false;
            }
        }

        public virtual void SetBooleanVariable(string key, bool value)
        {
            BooleanVariable variable = GetVariable<BooleanVariable>(key);
            if(variable != null)
            {
                variable.Value = value;
            }
        }

        public virtual int GetIntegerVariable(string key)
        {
            IntegerVariable variable = GetVariable<IntegerVariable>(key);

            if (variable != null)
            {
                return GetVariable<IntegerVariable>(key).Value;
            }
            else
            {
                return 0;
            }
        }

        public virtual void SetIntegerVariable(string key, int value)
        {
            IntegerVariable variable = GetVariable<IntegerVariable>(key);
            if (variable != null)
            {
                variable.Value = value;
            }
        }

        public virtual float GetFloatVariable(string key)
        {
            FloatVariable variable = GetVariable<FloatVariable>(key);

            if (variable != null)
            {
                return GetVariable<FloatVariable>(key).Value;
            }
            else
            {
                return 0f;
            }
        }

        public virtual void SetFloatVariable(string key, float value)
        {
            FloatVariable variable = GetVariable<FloatVariable>(key);
            if (variable != null)
            {
                variable.Value = value;
            }
        }

        public virtual string GetStringVariable(string key)
        {
            StringVariable variable = GetVariable<StringVariable>(key);

            if (variable != null)
            {
                return GetVariable<StringVariable>(key).Value;
            }
            else
            {
                return "";
            }
        }

        public virtual void SetStringVariable(string key, string value)
        {
            StringVariable variable = GetVariable<StringVariable>(key);
            if (variable != null)
            {
                variable.Value = value;
            }
        }

        public virtual void UpdateHideFlags()
        {
            if (hideComponents)
            {
                Block[] blocks = GetComponents<Block>();
                foreach (Block block in blocks)
                {
                    block.hideFlags = HideFlags.HideInInspector;
                    if (block.gameObject != gameObject)
                    {
                        block.hideFlags = HideFlags.HideInHierarchy;
                    }
                }

                Command[] commands = GetComponents<Command>();
                foreach (var command in commands)
                {
                    command.hideFlags = HideFlags.HideInInspector;
                }

                EventHandler[] eventHandlers = GetComponents<EventHandler>();
                foreach (var eventHandler in eventHandlers)
                {
                    eventHandler.hideFlags = HideFlags.HideInInspector;
                }
            }
            else
            {
                MonoBehaviour[] monoBehaviours = GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    if (monoBehaviour == null)
                    {
                        continue;
                    }

                    monoBehaviour.hideFlags = HideFlags.None;
                    monoBehaviour.gameObject.hideFlags = HideFlags.None;
                }
            }

        }

        public virtual void ClearSelectedCommands()
        {
            selectedCommands.Clear();
        }

        public virtual void AddSelectedCommand(Command command)
        {
            if (!selectedCommands.Contains(command))
            {
                selectedCommands.Add(command);
            }
        }

        public virtual void Reset(bool resetCommands, bool resetVariables)
        {
            if (resetCommands)
            {
                ICommand[] commands = GetComponents<ICommand>();
                foreach (var command in commands)
                {
                    command.OnReset();
                }
            }

            if (resetVariables)
            {
                foreach (Variable variable in variables)
                {
                    variable.OnReset();
                }
            }
        }

        public virtual bool IsCommandSupported(CommandInfoAttribute commandInfo)
        {
            foreach (string key in hideCommands)
            {
                // Match on category or command name (case insensitive)
                if (String.Compare(commandInfo.Category, key, StringComparison.OrdinalIgnoreCase) == 0 ||
                    String.Compare(commandInfo.CommandName, key, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool HasExecutingBlocks()
        {
            var blocks = GetComponents<IBlock>();
            foreach (IBlock block in blocks)
            {
                if (block.IsExecuting())
                {
                    return true;
                }
            }
            return false;
        }

        public virtual List<IBlock> GetExecutingBlocks()
        {
            var executingBlocks = new List<IBlock>();
            var blocks = GetComponents<IBlock>();
            foreach (var block in blocks)
            {
                if (block.IsExecuting())
                {
                    executingBlocks.Add(block);
                }
            }

            return executingBlocks;
        }

        public virtual string SubstituteVariables(string input)
        {
            if (stringSubstituer == null)
            {
                stringSubstituer = new StringSubstituter();
                stringSubstituer.CacheSubstitutionHandlers();
            }

            // Use the string builder from StringSubstituter for efficiency.
            StringBuilder sb = stringSubstituer._StringBuilder;
            sb.Length = 0;
            sb.Append(input);

            // Instantiate the regular expression object.
            Regex r = new Regex("{\\$.*?}");

            bool changed = false;

            // Match the regular expression pattern against a text string.
            var results = r.Matches(input);
            foreach (Match match in results)
            {
                string key = match.Value.Substring(2, match.Value.Length - 3);

                // Look for any matching private variables in this Flowchart first
                foreach (Variable variable in variables)
                {
                    if (variable == null)
                        continue;

                    if (variable.Scope == VariableScope.Private &&
                        variable.Key == key)
                    {   
                        string value = variable.ToString();
                        sb.Replace(match.Value, value);
                        changed = true;
                    }
                }
            }

            // Now do all other substitutions in the scene
            changed |= stringSubstituer.SubstituteStrings(sb);

            if (changed)
            {
                return sb.ToString();
            }
            else
            {
                return input;
            }
        }

        #endregion

        #region IStringSubstituter implementation

        /// <summary>
        /// Implementation of StringSubstituter.ISubstitutionHandler which matches any public variable in the Flowchart.
        /// To perform full variable substitution with all substitution handlers in the scene, you should
        /// use the SubstituteVariables() method instead.
        /// </summary>
        [MoonSharp.Interpreter.MoonSharpHidden]
        public virtual bool SubstituteStrings(StringBuilder input)
        {
            // Instantiate the regular expression object.
            Regex r = new Regex("{\\$.*?}");

            bool modified = false;

            // Match the regular expression pattern against a text string.
            var results = r.Matches(input.ToString());
            foreach (Match match in results)
            {
                string key = match.Value.Substring(2, match.Value.Length - 3);

                // Look for any matching public variables in this Flowchart
                foreach (Variable variable in variables)
                {
                    if (variable == null)
                        continue;

                    if (variable.Scope == VariableScope.Public &&
                        variable.Key == key)
                    {   
                        string value = variable.ToString();
                        input.Replace(match.Value, value);

                        modified = true;
                    }
                }
            }

            return modified;
        }

        #endregion
    }
}