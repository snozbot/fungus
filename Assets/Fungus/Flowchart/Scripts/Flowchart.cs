/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

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
	/**
	 * Interface for Flowchart components which can be updated when the 
	 * scene loads in the editor. This is used to maintain backwards 
	 * compatibility with earlier versions of Fungus.
	 */
	interface IUpdateable
	{
		void UpdateToVersion(int oldVersion, int newVersion);		
	}

	/**
	 * Visual scripting controller for the Flowchart programming language.
	 * Flowchart objects may be edited visually using the Flowchart editor window.
	 */
	[ExecuteInEditMode]
	public class Flowchart : MonoBehaviour, StringSubstituter.ISubstitutionHandler 
	{
		/**
        * The current version of the Flowchart. Used for updating components.
        */
		public const int CURRENT_VERSION = 1;

        /**
        * The name of the initial block in a new flowchart.
        */
        public const string DEFAULT_BLOCK_NAME = "New Block";

		/**
		 * Variable to track flowchart's version so components can update to new versions.
		 */
		[HideInInspector]
		public int version = 0; // Default to 0 to always trigger an update for older versions of Fungus.

		/**
		 * Scroll position of Flowchart editor window.
		 */
		[HideInInspector]
		public Vector2 scrollPos;

		/**
		 * Scroll position of Flowchart variables window.
		 */
		[HideInInspector]
		public Vector2 variablesScrollPos;

		/**
		 * Show the variables pane.
		 */
		[HideInInspector]
		public bool variablesExpanded = true;

		/**
		 * Height of command block view in inspector.
		 */
		[HideInInspector]
		public float blockViewHeight = 400;

		/**
		 * Zoom level of Flowchart editor window
		 */
		[HideInInspector]
		public float zoom = 1f;

		/**
		 * Scrollable area for Flowchart editor window.
		 */
		[HideInInspector]
		public Rect scrollViewRect;

		/**
		 * Currently selected block in the Flowchart editor.
		 */
		[HideInInspector]
		[FormerlySerializedAs("selectedSequence")]
		public Block selectedBlock;
		
		/**
		 * Currently selected command in the Flowchart editor.
		 */
		[HideInInspector]
		public List<Command> selectedCommands = new List<Command>();

		/**
		 * The list of variables that can be accessed by the Flowchart.
		 */
		[HideInInspector]
		public List<Variable> variables = new List<Variable>();

		[TextArea(3, 5)]
		[Tooltip("Description text displayed in the Flowchart editor window")]
		public string description = "";

		/**
	 	 * Slow down execution in the editor to make it easier to visualise program flow.
	 	 */
		[Range(0f, 5f)]
		[Tooltip("Adds a pause after each execution step to make it easier to visualise program flow. Editor only, has no effect in platform builds.")]
		public float stepPause = 0f;

		/**
		 * Use command color when displaying the command list in the inspector.
		 */
		[Tooltip("Use command color when displaying the command list in the Fungus Editor window")]
		public bool colorCommands = true;
		
		/**
		 * Hides the Flowchart block and command components in the inspector.
		 * Deselect to inspect the block and command components that make up the Flowchart.
		 */
		[Tooltip("Hides the Flowchart block and command components in the inspector")]
		public bool hideComponents = true;

		/**
		 * Saves the selected block and commands when saving the scene.
		 * Helps avoid version control conflicts if you've only changed the active selection.
		 */
		[Tooltip("Saves the selected block and commands when saving the scene.")]
		public bool saveSelection = true;

		/**
		 * Unique identifier for identifying this flowchart in localized string keys.
		 */
		[Tooltip("Unique identifier for this flowchart in localized string keys. If no id is specified then the name of the Flowchart object will be used.")]
		public string localizationId = "";

		/**
		 * Display line numbers in the command list in the Block inspector.
		 */ 
		[Tooltip("Display line numbers in the command list in the Block inspector.")]
		public bool showLineNumbers = false;

		/**
		 * List of commands to hide in the Add Command menu. Use this to restrict the set of commands available when editing a Flowchart.
		 */
		[Tooltip("List of commands to hide in the Add Command menu. Use this to restrict the set of commands available when editing a Flowchart.")]
		public List<string> hideCommands = new List<string>();

        [Tooltip("Lua Environment to be used by default for all Execute Lua commands in this Flowchart")]
        public LuaEnvironment luaEnvironment;

        /**
         * The ExecuteLua command adds a global Lua variable with this name bound to the flowchart prior to executing.
         */
        [Tooltip("The ExecuteLua command adds a global Lua variable with this name bound to the flowchart prior to executing.")]
        public string luaBindingName = "flowchart";

		/**
		 * Position in the center of all blocks in the flowchart.
		 */
		[NonSerialized]
		public Vector2 centerPosition = Vector2.zero;

		/**
		 * Cached list of flowchart objects in the scene for fast lookup
		 */
		public static List<Flowchart> cachedFlowcharts = new List<Flowchart>();

		protected static bool eventSystemPresent;

		protected StringSubstituter stringSubstituer;

		/**
		 * Returns the next id to assign to a new flowchart item.
		 * Item ids increase monotically so they are guaranteed to
		 * be unique within a Flowchart.
		 */
		public int NextItemId()
		{
			int maxId = -1;
			Block[] blocks = GetComponents<Block>();
			foreach (Block block in blocks)
			{
				maxId = Math.Max(maxId, block.itemId);
			}
			
			Command[] commands = GetComponents<Command>();
			foreach (Command command in commands)
			{
				maxId = Math.Max(maxId, command.itemId);
			}
			return maxId + 1;
		}

		#if UNITY_5_4_OR_NEWER
		protected virtual void Awake()
		{
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (A, B) => {
				LevelWasLoaded();
			};
		}
		#else
		public virtual void OnLevelWasLoaded(int level) 
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
				GameObject prefab = Resources.Load<GameObject>("EventSystem");
				if (prefab != null)
				{
					GameObject go = Instantiate(prefab) as GameObject;
					go.name = "EventSystem";
				}
			}
			
			eventSystemPresent = true;
		}

		public virtual void OnEnable()
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
			if (version == CURRENT_VERSION)
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
					u.UpdateToVersion(version, CURRENT_VERSION);
				}
			}

			version = CURRENT_VERSION;
		}

		public virtual void OnDisable()
		{
			cachedFlowcharts.Remove(this);
		}

		protected virtual void CheckItemIds()
		{
			// Make sure item ids are unique and monotonically increasing.
			// This should always be the case, but some legacy Flowcharts may have issues.
			List<int> usedIds = new List<int>();
			Block[] blocks = GetComponents<Block>();
			foreach (Block block in blocks)
			{
				if (block.itemId == -1 ||
				    usedIds.Contains(block.itemId))
				{
					block.itemId = NextItemId();
				}
				usedIds.Add(block.itemId);
			}
			
			Command[] commands = GetComponents<Command>();
			foreach (Command command in commands)
			{
				if (command.itemId == -1 ||
				    usedIds.Contains(command.itemId))
				{
					command.itemId = NextItemId();
				}
				usedIds.Add(command.itemId);
			}
		}

		protected virtual void CleanupComponents()
		{
			// Delete any unreferenced components which shouldn't exist any more
			// Unreferenced components don't have any effect on the flowchart behavior, but
			// they waste memory so should be cleared out periodically.

			Block[] blocks = GetComponents<Block>();

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
			
			foreach (Command command in GetComponents<Command>())
			{
				bool found = false;
				foreach (Block block in blocks)
				{
					if (block.commandList.Contains(command))
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
				foreach (Block block in blocks)
				{
					if (block.eventHandler == eventHandler)
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

		/**
		 * Create a new block node which you can then add commands to.
		 */
		public virtual Block CreateBlock(Vector2 position)
		{
			Block b = CreateBlockComponent(gameObject);
			b.nodeRect.x = position.x;
			b.nodeRect.y = position.y;
			b.blockName = GetUniqueBlockKey(b.blockName, b);
			b.itemId = NextItemId();

			return b;
		}

		/**
		 * Returns the named Block in the flowchart, or null if not found.
		 */
		public virtual Block FindBlock(string blockName)
		{
			Block [] blocks = GetComponents<Block>();
			foreach (Block block in blocks)
			{
				if (block.blockName == blockName)
				{
					return block;
				}
			}
			
			return null;
		}

		/**
		 * Execute a child block in the Flowchart.
		 * You can use this method in a UI event. e.g. to handle a button click.
		 * Returns true if the Block started execution.
		 */
        public virtual void ExecuteBlock(string blockName)
		{
            Block block = null;
            foreach (Block b in GetComponents<Block>())
            {
                if (b.blockName == blockName)
                {
                    block = b;
                    break;
                }
            }

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

		/**
		 * Execute a child block in the flowchart.
		 * The block must be in an idle state to be executed.
		 * This version provides extra options to control how the block is executed.
		 * Returns true if the Block started execution.
		 */
        public virtual bool ExecuteBlock(Block block, int commandIndex = 0, Action onComplete = null)
		{
            if (block == null)
            {
                Debug.LogError("Block must not be null");
                return false;
            }

            if (block.gameObject != gameObject)
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

		/**
		 * Stop all executing Blocks in this Flowchart.
		 */
		public virtual void StopAllBlocks()
		{
			Block [] blocks = GetComponents<Block>();
			foreach (Block block in blocks)
			{
				if (block.IsExecuting())
				{
					block.Stop();
				}
			}
		}

        /**
         * Sends a message to this Flowchart only.
         * Any block with a matching MessageReceived event handler will start executing.
         */
        public virtual void SendFungusMessage(string messageName)
        {
            MessageReceived[] eventHandlers = GetComponents<MessageReceived>();
            foreach (MessageReceived eventHandler in eventHandlers)
            {
                eventHandler.OnSendFungusMessage(messageName);
            }
        }

        /**
         * Sends a message to all Flowchart objects in the current scene.
         * Any block with a matching MessageReceived event handler will start executing.
         */
        public static void BroadcastFungusMessage(string messageName)
        {
            MessageReceived[] eventHandlers = UnityEngine.Object.FindObjectsOfType<MessageReceived>();
            foreach (MessageReceived eventHandler in eventHandlers)
            {
                eventHandler.OnSendFungusMessage(messageName);
            }
        }

		/**
		 * Returns a new variable key that is guaranteed not to clash with any existing variable in the list.
		 */
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
					    variable.key == null)
					{
						continue;
					}

					if (variable.key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
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

		/**
		 * Returns a new Block key that is guaranteed not to clash with any existing Block in the Flowchart.
		 */
		public virtual string GetUniqueBlockKey(string originalKey, Block ignoreBlock = null)
		{
			int suffix = 0;
			string baseKey = originalKey.Trim();
			
			// No empty keys allowed
			if (baseKey.Length == 0)
			{
				baseKey = "New Block";
			}

			Block[] blocks = GetComponents<Block>();

			string key = baseKey;
			while (true)
			{
				bool collision = false;
				foreach(Block block in blocks)
				{
					if (block == ignoreBlock ||
					    block.blockName == null)
					{
						continue;
					}
					
					if (block.blockName.Equals(key, StringComparison.CurrentCultureIgnoreCase))
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

		/**
		 * Returns a new Label key that is guaranteed not to clash with any existing Label in the Block.
		 */
		public virtual string GetUniqueLabelKey(string originalKey, Label ignoreLabel)
		{
			int suffix = 0;
			string baseKey = originalKey.Trim();
			
			// No empty keys allowed
			if (baseKey.Length == 0)
			{
				baseKey = "New Label";
			}
			
			Block block = ignoreLabel.parentBlock;
			
			string key = baseKey;
			while (true)
			{
				bool collision = false;
				foreach(Command command in block.commandList)
				{
					Label label = command as Label;
					if (label == null ||
						label == ignoreLabel)
					{
						continue;
					}
					
					if (label.key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
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

        /**
         * Returns the variable with the specified key, or null if the key is not found.
         * You will need to cast the returned variable to the correct sub-type.
         * You can then access the variable's value using the Value property. e.g.
         *  BooleanVariable boolVar = flowchart.GetVariable("MyBool") as BooleanVariable;
         *  boolVar.Value = false;
         */
        public Variable GetVariable(string key)
        {
            foreach (Variable variable in variables)
            {
                if (variable != null && variable.key == key)
                {
                    return variable;
                }
            }

            return null;
        }

		/**
		 * Returns the variable with the specified key, or null if the key is not found.
		 * You can then access the variable's value using the Value property. e.g.
		 * 	BooleanVariable boolVar = flowchart.GetVariable<BooleanVariable>("MyBool");
		 * 	boolVar.Value = false;
		 */
		public T GetVariable<T>(string key) where T : Variable
		{
			foreach (Variable variable in variables)
			{
				if (variable != null && variable.key == key)
				{
					return variable as T;
				}
			}

            Debug.LogWarning("Variable " + key + " not found.");
            return null;
		}

		/**
		 * Register a new variable with the Flowchart at runtime. 
		 * The variable should be added as a component on the Flowchart game object.
		 */
        public void SetVariable<T>(string key, T newvariable) where T : Variable
        {
            foreach (Variable v in variables)
            {
                if (v != null && v.key == key)
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

        /**
		 * Gets a list of all variables with public scope in this Flowchart.
		 */
        public virtual List<Variable> GetPublicVariables()
		{
			List<Variable> publicVariables = new List<Variable>();
			foreach (Variable v in variables)
			{
				if (v != null && v.scope == VariableScope.Public)
				{
					publicVariables.Add(v);
				}
			}

			return publicVariables;
		}

        /**
		 * Gets the value of a boolean variable.
		 * Returns false if the variable key does not exist.
		 */
        public virtual bool GetBooleanVariable(string key)
        {
            BooleanVariable variable = GetVariable<BooleanVariable>(key);

            if(variable != null)
            {
                return GetVariable<BooleanVariable>(key).value;
            }
            else
            {
                return false;
            }
        }

        /**
		 * Sets the value of a boolean variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
        public virtual void SetBooleanVariable(string key, bool value)
        {
            BooleanVariable variable = GetVariable<BooleanVariable>(key);
            if(variable != null)
            {
                variable.value = value;
            }
        }

        /**
		 * Gets the value of an integer variable.
		 * Returns 0 if the variable key does not exist.
		 */
        public virtual int GetIntegerVariable(string key)
        {
            IntegerVariable variable = GetVariable<IntegerVariable>(key);

            if (variable != null)
            {
                return GetVariable<IntegerVariable>(key).value;
            }
            else
            {
                return 0;
            }
        }

        /**
		 * Sets the value of an integer variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
        public virtual void SetIntegerVariable(string key, int value)
        {
            IntegerVariable variable = GetVariable<IntegerVariable>(key);
            if (variable != null)
            {
                variable.value = value;
            }
        }

        /**
		 * Gets the value of a float variable.
		 * Returns 0 if the variable key does not exist.
		 */
        public virtual float GetFloatVariable(string key)
        {
            FloatVariable variable = GetVariable<FloatVariable>(key);

            if (variable != null)
            {
                return GetVariable<FloatVariable>(key).value;
            }
            else
            {
                return 0f;
            }
        }

        /**
		 * Sets the value of a float variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
        public virtual void SetFloatVariable(string key, float value)
        {
            FloatVariable variable = GetVariable<FloatVariable>(key);
            if (variable != null)
            {
                variable.value = value;
            }
        }

        /**
		 * Gets the value of a string variable.
		 * Returns the empty string if the variable key does not exist.
		 */
        public virtual string GetStringVariable(string key)
        {
            StringVariable variable = GetVariable<StringVariable>(key);

            if (variable != null)
            {
                return GetVariable<StringVariable>(key).value;
            }
            else
            {
                return "";
            }
        }

        /**
		 * Sets the value of a string variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
        public virtual void SetStringVariable(string key, string value)
        {
            StringVariable variable = GetVariable<StringVariable>(key);
            if (variable != null)
            {
                variable.value = value;
            }
        }

        /**
		 * Set the block objects to be hidden or visible depending on the hideComponents property.
		 */
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
						block.gameObject.hideFlags = HideFlags.HideInHierarchy;
					}
				}

				Command[] commands = GetComponents<Command>();
				foreach (Command command in commands)
				{
					command.hideFlags = HideFlags.HideInInspector;
				}

				EventHandler[] eventHandlers = GetComponents<EventHandler>();
				foreach (EventHandler eventHandler in eventHandlers)
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
				Command[] commands = GetComponents<Command>();
				foreach (Command command in commands)
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

		/**
		 * Override this in a Flowchart subclass to filter which commands are shown in the Add Command list.
		 */
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

		/**
		 * Returns true if there are any executing blocks in this Flowchart.
		 */
		public virtual bool HasExecutingBlocks()
		{
			Block[] blocks = GetComponents<Block>();
			foreach (Block block in blocks)
			{
				if (block.IsExecuting())
				{
					return true;
				}
			}
			return false;
		}

		/**
		 * Returns a list of all executing blocks in this Flowchart.
		 */
		public virtual List<Block> GetExecutingBlocks()
		{
			List<Block> executingBlocks = new List<Block>();

			Block[] blocks = GetComponents<Block>();
			foreach (Block block in blocks)
			{
				if (block.IsExecuting())
				{
					executingBlocks.Add(block);
				}
			}

			return executingBlocks;
		}

		/**
		 * Implementation of StringSubstituter.ISubstitutionHandler which matches any public variable in the Flowchart.
		 * To perform full variable substitution with all substitution handlers in the scene, you should
		 * use the SubstituteVariables() method instead.
		 */
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

					if (variable.scope == VariableScope.Public &&
						variable.key == key)
					{	
						string value = variable.ToString();
						input.Replace(match.Value, value);

                        modified = true;
					}
				}
			}

            return modified;
		}

		/**
		 * Substitute variables in the input text with the format {$VarName}
		 * This will first match with private variables in this Flowchart, and then
		 * with public variables in all Flowcharts in the scene (and any component
		 * in the scene that implements StringSubstituter.ISubstitutionHandler).
		 */
		public virtual string SubstituteVariables(string input)
		{
			if (stringSubstituer == null)
			{
				stringSubstituer = new StringSubstituter();
			}

            // Use the string builder from StringSubstituter for efficiency.
            StringBuilder sb = stringSubstituer.stringBuilder;
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

					if (variable.scope == VariableScope.Private &&
						variable.key == key)
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
	}

}