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

namespace Fungus
{
    /// <summary>
    /// Visual scripting controller for the Flowchart programming language.
    /// Flowchart objects may be edited visually using the Flowchart editor window.
    /// </summary>
    [ExecuteInEditMode]
    public class Flowchart : MonoBehaviour, ISubstitutionHandler
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

        protected static List<Flowchart> cachedFlowcharts = new List<Flowchart>();

        protected static bool eventSystemPresent;

        protected StringSubstituter stringSubstituer;

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
            var blocks = GetComponents<Block>();
            foreach (var block in blocks)
            {
                if (block.ItemId == -1 ||
                    usedIds.Contains(block.ItemId))
                {
                    block.ItemId = NextItemId();
                }
                usedIds.Add(block.ItemId);
            }
            
            var commands = GetComponents<Command>();
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
            
            var blocks = GetComponents<Block>();

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

        #region Public members

        /// <summary>
        /// Cached list of flowchart objects in the scene for fast lookup.
        /// </summary>
        public static List<Flowchart> CachedFlowcharts { get { return cachedFlowcharts; } }

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

        /// <summary>
        /// Scroll position of Flowchart editor window.
        /// </summary>
        public virtual Vector2 ScrollPos { get { return scrollPos; } set { scrollPos = value; } }

        /// <summary>
        /// Scroll position of Flowchart variables window.
        /// </summary>
        public virtual Vector2 VariablesScrollPos { get { return variablesScrollPos; } set { variablesScrollPos = value; } }

        /// <summary>
        /// Show the variables pane.
        /// </summary>
        public virtual bool VariablesExpanded { get { return variablesExpanded; } set { variablesExpanded = value; } }

        /// <summary>
        /// Height of command block view in inspector.
        /// </summary>
        public virtual float BlockViewHeight { get { return blockViewHeight; } set { blockViewHeight = value; } }

        /// <summary>
        /// Zoom level of Flowchart editor window.
        /// </summary>
        public virtual float Zoom { get { return zoom; } set { zoom = value; } }

        /// <summary>
        /// Scrollable area for Flowchart editor window.
        /// </summary>
        public virtual Rect ScrollViewRect { get { return scrollViewRect; } set { scrollViewRect = value; } }

        /// <summary>
        /// Currently selected block in the Flowchart editor.
        /// </summary>
        public virtual Block SelectedBlock { get { return selectedBlock; } set { selectedBlock = value; } }

        /// <summary>
        /// Currently selected command in the Flowchart editor.
        /// </summary>
        public virtual List<Command> SelectedCommands { get { return selectedCommands; } }

        /// <summary>
        /// The list of variables that can be accessed by the Flowchart.
        /// </summary>
        public virtual List<Variable> Variables { get { return variables; } }

        /// <summary>
        /// Description text displayed in the Flowchart editor window
        /// </summary>
        public virtual string Description { get { return description; } }

        /// <summary>
        /// Slow down execution in the editor to make it easier to visualise program flow.
        /// </summary>
        public virtual float StepPause { get { return stepPause; } }

        /// <summary>
        /// Use command color when displaying the command list in the inspector.
        /// </summary>
        public virtual bool ColorCommands { get { return colorCommands; } }

        /// <summary>
        /// Saves the selected block and commands when saving the scene. Helps avoid version control conflicts if you've only changed the active selection.
        /// </summary>
        public virtual bool SaveSelection { get { return saveSelection; } }

        /// <summary>
        /// Unique identifier for identifying this flowchart in localized string keys.
        /// </summary>
        public virtual string LocalizationId { get { return localizationId; } }

        /// <summary>
        /// Display line numbers in the command list in the Block inspector.
        /// </summary>
        public virtual bool ShowLineNumbers { get { return showLineNumbers; } }

        /// <summary>
        /// Lua Environment to be used by default for all Execute Lua commands in this Flowchart.
        /// </summary>
        public virtual LuaEnvironment LuaEnv { get { return luaEnvironment; } }

        /// <summary>
        /// The ExecuteLua command adds a global Lua variable with this name bound to the flowchart prior to executing.
        /// </summary>
        public virtual string LuaBindingName { get { return luaBindingName; } }

        /// <summary>
        /// Position in the center of all blocks in the flowchart.
        /// </summary>
        public virtual Vector2 CenterPosition { set; get; }

        /// <summary>
        /// Variable to track flowchart's version so components can update to new versions.
        /// </summary>
        public int Version { set { version = value; } }

        /// <summary>
        /// Returns true if the Flowchart gameobject is active.
        /// </summary>
        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        /// <summary>
        /// Returns the Flowchart gameobject name.
        /// </summary>
        public string GetName()
        {
            return gameObject.name;
        }

        /// <summary>
        /// Returns the next id to assign to a new flowchart item.
        /// Item ids increase monotically so they are guaranteed to
        /// be unique within a Flowchart.
        /// </summary>
        public int NextItemId()
        {
            int maxId = -1;
            var blocks = GetComponents<Block>();
            foreach (var block in blocks)
            {
                maxId = Math.Max(maxId, block.ItemId);
            }

            var commands = GetComponents<Command>();
            foreach (var command in commands)
            {
                maxId = Math.Max(maxId, command.ItemId);
            }
            return maxId + 1;
        }

        /// <summary>
        /// Create a new block node which you can then add commands to.
        /// </summary>
        public virtual Block CreateBlock(Vector2 position)
        {
            Block b = CreateBlockComponent(gameObject);
            b._NodeRect = new Rect(position.x, position.y, 0, 0);
            b.BlockName = GetUniqueBlockKey(b.BlockName, b);
            b.ItemId = NextItemId();

            return b;
        }

        /// <summary>
        /// Returns the named Block in the flowchart, or null if not found.
        /// </summary>
        public virtual Block FindBlock(string blockName)
        {
            var blocks = GetComponents<Block>();
            foreach (var block in blocks)
            {
                if (block.BlockName == blockName)
                {
                    return block;
                }
            }

            return null;
        }

        /// <summary>
        /// Execute a child block in the Flowchart.
        /// You can use this method in a UI event. e.g. to handle a button click.
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

        /// <summary>
        /// Execute a child block in the flowchart.
        /// The block must be in an idle state to be executed.
        /// This version provides extra options to control how the block is executed.
        /// Returns true if the Block started execution.            
        /// </summary>
        public virtual bool ExecuteBlock(Block block, int commandIndex = 0, Action onComplete = null)
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

        /// <summary>
        /// Stop all executing Blocks in this Flowchart.
        /// </summary>
        public virtual void StopAllBlocks()
        {
            var blocks = GetComponents<Block>();
            foreach (Block block in blocks)
            {
                if (block.IsExecuting())
                {
                    block.Stop();
                }
            }
        }

        /// <summary>
        /// Sends a message to this Flowchart only.
        /// Any block with a matching MessageReceived event handler will start executing.
        /// </summary>
        public virtual void SendFungusMessage(string messageName)
        {
            MessageReceived[] eventHandlers = GetComponents<MessageReceived>();
            foreach (MessageReceived eventHandler in eventHandlers)
            {
                eventHandler.OnSendFungusMessage(messageName);
            }
        }

        /// <summary>
        /// Returns a new variable key that is guaranteed not to clash with any existing variable in the list.
        /// </summary>
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

        /// <summary>
        /// Returns a new Block key that is guaranteed not to clash with any existing Block in the Flowchart.
        /// </summary>
        public virtual string GetUniqueBlockKey(string originalKey, Block ignoreBlock = null)
        {
            int suffix = 0;
            string baseKey = originalKey.Trim();

            // No empty keys allowed
            if (baseKey.Length == 0)
            {
                baseKey = FungusConstants.DefaultBlockName;
            }

            var blocks = GetComponents<Block>();

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

        /// <summary>
        /// Returns a new Label key that is guaranteed not to clash with any existing Label in the Block.
        /// </summary>
        public virtual string GetUniqueLabelKey(string originalKey, Label ignoreLabel)
        {
            int suffix = 0;
            string baseKey = originalKey.Trim();

            // No empty keys allowed
            if (baseKey.Length == 0)
            {
                baseKey = "New Label";
            }

            var block = ignoreLabel.ParentBlock;

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

        /// <summary>
        /// Returns the variable with the specified key, or null if the key is not found.
        /// You will need to cast the returned variable to the correct sub-type.
        /// You can then access the variable's value using the Value property. e.g.
        /// BooleanVariable boolVar = flowchart.GetVariable("MyBool") as BooleanVariable;
        /// boolVar.Value = false;
        /// </summary>
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

        /// <summary>
        /// Returns the variable with the specified key, or null if the key is not found.
        /// You can then access the variable's value using the Value property. e.g.
        /// BooleanVariable boolVar = flowchart.GetVariable<BooleanVariable>("MyBool");
        /// boolVar.Value = false;
        /// </summary>
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

        /// <summary>
        /// Register a new variable with the Flowchart at runtime. 
        /// The variable should be added as a component on the Flowchart game object.
        /// </summary>
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

        /// <summary>
        /// Gets a list of all variables with public scope in this Flowchart.
        /// </summary>
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

        /// <summary>
        /// Gets the value of a boolean variable.
        /// Returns false if the variable key does not exist.
        /// </summary>
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

        /// <summary>
        /// Sets the value of a boolean variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        public virtual void SetBooleanVariable(string key, bool value)
        {
            BooleanVariable variable = GetVariable<BooleanVariable>(key);
            if(variable != null)
            {
                variable.Value = value;
            }
        }

        /// <summary>
        /// Gets the value of an integer variable.
        /// Returns 0 if the variable key does not exist.
        /// </summary>
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

        /// <summary>
        /// Sets the value of an integer variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        public virtual void SetIntegerVariable(string key, int value)
        {
            IntegerVariable variable = GetVariable<IntegerVariable>(key);
            if (variable != null)
            {
                variable.Value = value;
            }
        }

        /// <summary>
        /// Gets the value of a float variable.
        /// Returns 0 if the variable key does not exist.
        /// </summary>
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

        /// <summary>
        /// Sets the value of a float variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        public virtual void SetFloatVariable(string key, float value)
        {
            FloatVariable variable = GetVariable<FloatVariable>(key);
            if (variable != null)
            {
                variable.Value = value;
            }
        }

        /// <summary>
        /// Gets the value of a string variable.
        /// Returns the empty string if the variable key does not exist.
        /// </summary>
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

        /// <summary>
        /// Sets the value of a string variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        public virtual void SetStringVariable(string key, string value)
        {
            StringVariable variable = GetVariable<StringVariable>(key);
            if (variable != null)
            {
                variable.Value = value;
            }
        }

        /// <summary>
        /// Set the block objects to be hidden or visible depending on the hideComponents property.
        /// </summary>
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

        /// <summary>
        /// Clears the list of selected commands.
        /// </summary>
        public virtual void ClearSelectedCommands()
        {
            selectedCommands.Clear();
        }

        /// <summary>
        /// Adds a command to the list of selected commands.
        /// </summary>
        public virtual void AddSelectedCommand(Command command)
        {
            if (!selectedCommands.Contains(command))
            {
                selectedCommands.Add(command);
            }
        }

        /// <summary>
        /// Reset the commands and variables in the Flowchart.
        /// </summary>
        public virtual void Reset(bool resetCommands, bool resetVariables)
        {
            if (resetCommands)
            {
                var commands = GetComponents<Command>();
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

        /// <summary>
        /// Override this in a Flowchart subclass to filter which commands are shown in the Add Command list.
        /// </summary>
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

        /// <summary>
        /// Returns true if there are any executing blocks in this Flowchart.
        /// </summary>
        public virtual bool HasExecutingBlocks()
        {
            var blocks = GetComponents<Block>();
            foreach (var block in blocks)
            {
                if (block.IsExecuting())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a list of all executing blocks in this Flowchart.
        /// </summary>
        public virtual List<Block> GetExecutingBlocks()
        {
            var executingBlocks = new List<Block>();
            var blocks = GetComponents<Block>();
            foreach (var block in blocks)
            {
                if (block.IsExecuting())
                {
                    executingBlocks.Add(block);
                }
            }

            return executingBlocks;
        }

        /// <summary>
        /// Substitute variables in the input text with the format {$VarName}
        /// This will first match with private variables in this Flowchart, and then
        /// with public variables in all Flowcharts in the scene (and any component
        /// in the scene that implements StringSubstituter.ISubstitutionHandler).
        /// </summary>
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