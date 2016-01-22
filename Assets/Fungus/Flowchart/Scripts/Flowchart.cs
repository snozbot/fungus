using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	/**
	 * Visual scripting controller for the Flowchart programming language.
	 * Flowchart objects may be edited visually using the Flowchart editor window.
	 */
	[ExecuteInEditMode]
	public class Flowchart : MonoBehaviour 
	{
        /**
		 * Current version used to compare with the previous version so older versions can be custom-updated from previous versions.
		 */
        public const string CURRENT_VERSION = "1.0";

        /**
        * The name of the initial block in a new flowchart.
        */
        public const string DEFAULT_BLOCK_NAME = "New Block";

		/**
		 * Variable to track flowchart's version and if initial set up has completed.
		 */
		[HideInInspector]
		public string version;

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
		 * List of commands to hide in the Add Command menu. Use this to restrict the set of commands available when editing a Flowchart.
		 */
		[Tooltip("List of commands to hide in the Add Command menu. Use this to restrict the set of commands available when editing a Flowchart.")]
		public List<string> hideCommands = new List<string>();

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

		/**
		 * Returns the next id to assign to a new flowchart item.
		 * Item ids increase monotically so they are guaranteed to
		 * be unique within a Flowchart.
		 */
		public int NextItemId()
		{
			int maxId = -1;
			Block[] blocks = GetComponentsInChildren<Block>();
			foreach (Block block in blocks)
			{
				maxId = Math.Max(maxId, block.itemId);
			}
			
			Command[] commands = GetComponentsInChildren<Command>();
			foreach (Command command in commands)
			{
				maxId = Math.Max(maxId, command.itemId);
			}
			return maxId + 1;
		}

		protected virtual void OnLevelWasLoaded(int level) 
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

		public virtual void OnDisable()
		{
			cachedFlowcharts.Remove(this);
		}

		protected virtual void CheckItemIds()
		{
			// Make sure item ids are unique and monotonically increasing.
			// This should always be the case, but some legacy Flowcharts may have issues.
			List<int> usedIds = new List<int>();
			Block[] blocks = GetComponentsInChildren<Block>();
			foreach (Block block in blocks)
			{
				if (block.itemId == -1 ||
				    usedIds.Contains(block.itemId))
				{
					block.itemId = NextItemId();
				}
				usedIds.Add(block.itemId);
			}
			
			Command[] commands = GetComponentsInChildren<Command>();
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

			Block[] blocks = GetComponentsInChildren<Block>();

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

        private void UpdateVersion()
        {
            // If versions match, then we are already using the latest.
            if (version == CURRENT_VERSION) return;

            switch (version)
            {
                // Version never set, so we are initializing on first creation or this flowchart is pre-versioning.
                case null:
                case "":
                    Initialize();
                    break;
            }

            version = CURRENT_VERSION;
        }

	    protected virtual void Initialize()
	    {}

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
			Block [] blocks = GetComponentsInChildren<Block>();
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
		 * Start running another Flowchart by executing a specific child block.
		 * The block must be in an idle state to be executed.
		 * You can use this method in a UI event. e.g. to handle a button click.
		 */
		public virtual void ExecuteBlock(string blockName)
		{
			Block [] blocks = GetComponentsInChildren<Block>();
			foreach (Block block in blocks)
			{
				if (block.blockName == blockName)
				{
					ExecuteBlock(block);
				}
			}
		}

		/**
		 * Sends a message to this Flowchart only.
		 * Any block with a matching MessageReceived event handler will start executing.
		 */
		public virtual void SendFungusMessage(string messageName)
		{
			MessageReceived[] eventHandlers = GetComponentsInChildren<MessageReceived>();
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
			MessageReceived[] eventHandlers = GameObject.FindObjectsOfType<MessageReceived>();
			foreach (MessageReceived eventHandler in eventHandlers)
			{
				eventHandler.OnSendFungusMessage(messageName);
			}
		}

		/**
		 * Start executing a specific child block in the flowchart.
		 * The block must be in an idle state to be executed.
		 * Returns true if the Block started execution.
		 */
		public virtual bool ExecuteBlock(Block block, Action onComplete = null)
		{
			// Block must be a component of the Flowchart game object
			if (block == null ||
			    block.gameObject != gameObject) 
			{
				return false;
			}

			// Can't restart a running block, have to wait until it's idle again
			if (block.IsExecuting())
			{
				return false;
			}

			// Execute the first command in the command list
			block.Execute(onComplete);

			return true;
		}

		/**
		 * Stop all executing Blocks in this Flowchart.
		 */
		public virtual void StopAllBlocks()
		{
			Block [] blocks = GetComponentsInChildren<Block>();
			foreach (Block block in blocks)
			{
				if (block.IsExecuting())
				{
					block.Stop();
				}
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

			Block[] blocks = GetComponentsInChildren<Block>();

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

			return null;
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
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					BooleanVariable variable = v as BooleanVariable;
					if (variable != null)
					{
						return variable.value;
					}
				}
			}
			Debug.LogWarning("Boolean variable " + key + " not found.");
			return false;
		}
					
		/**
		 * Sets the value of a boolean variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
		public virtual void SetBooleanVariable(string key, bool value)
		{
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					BooleanVariable variable = v as BooleanVariable;
					if (variable != null)
					{
						variable.value = value;
						return;
					}
				}
			}
			Debug.LogWarning("Boolean variable " + key + " not found.");
		}

		/**
		 * Gets the value of an integer variable.
		 * Returns 0 if the variable key does not exist.
		 */
		public virtual int GetIntegerVariable(string key)
		{
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					IntegerVariable variable = v as IntegerVariable;
					if (variable != null)
					{
						return variable.value;
					}
				}
			}
			Debug.LogWarning("Integer variable " + key + " not found.");
			return 0;
		}

		/**
		 * Sets the value of an integer variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
		public virtual void SetIntegerVariable(string key, int value)
		{
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					IntegerVariable variable = v as IntegerVariable;
					if (variable != null)
					{
						variable.value = value;
						return;
					}
				}
			}
			Debug.LogWarning("Integer variable " + key + " not found.");
		}

		/**
		 * Gets the value of a float variable.
		 * Returns 0 if the variable key does not exist.
		 */
		public virtual float GetFloatVariable(string key)
		{
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					FloatVariable variable = v as FloatVariable;
					if (variable != null)
					{
						return variable.value;
					}
				}
			}
			Debug.LogWarning("Float variable " + key + " not found.");
			return 0f;
		}
				
		/**
		 * Sets the value of a float variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
		public virtual void SetFloatVariable(string key, float value)
		{
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					FloatVariable variable = v as FloatVariable;
					if (variable != null)
					{
						variable.value = value;
						return;
					}
				}
			}
			Debug.LogWarning("Float variable " + key + " not found.");
		}

		/**
		 * Gets the value of a string variable.
		 * Returns the empty string if the variable key does not exist.
		 */
		public virtual string GetStringVariable(string key)
		{
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					StringVariable variable = v as StringVariable;
					if (variable != null)
					{
						return variable.value;
					}
				}
			}
			Debug.LogWarning("String variable " + key + " not found.");
			return "";
		}

		/**
		 * Sets the value of a string variable.
		 * The variable must already be added to the list of variables for this Flowchart.
		 */
		public virtual void SetStringVariable(string key, string value)
		{
			foreach (Variable v in variables)
			{
				if (v != null && v.key == key)
				{
					StringVariable variable = v as StringVariable;
					if (variable != null)
					{
						variable.value = value;
						return;
					}
				}
			}
			Debug.LogWarning("String variable " + key + " not found.");
		}

		/**
		 * Set the block objects to be hidden or visible depending on the hideComponents property.
		 */
		public virtual void UpdateHideFlags()
		{
			if (hideComponents)
			{
				Block[] blocks = GetComponentsInChildren<Block>();
				foreach (Block block in blocks)
				{
					block.hideFlags = HideFlags.HideInInspector;
					if (block.gameObject != gameObject)
					{
						block.gameObject.hideFlags = HideFlags.HideInHierarchy;
					}
				}

				Command[] commands = GetComponentsInChildren<Command>();
				foreach (Command command in commands)
				{
					command.hideFlags = HideFlags.HideInInspector;
				}

				EventHandler[] eventHandlers = GetComponentsInChildren<EventHandler>();
				foreach (EventHandler eventHandler in eventHandlers)
				{
					eventHandler.hideFlags = HideFlags.HideInInspector;
				}
			}
			else
			{
				MonoBehaviour[] monoBehaviours = GetComponentsInChildren<MonoBehaviour>();
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
				Command[] commands = GetComponentsInChildren<Command>();
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
			Block[] blocks = GetComponentsInChildren<Block>();
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

			Block[] blocks = GetComponentsInChildren<Block>();
			foreach (Block block in blocks)
			{
				if (block.IsExecuting())
				{
					executingBlocks.Add(block);
				}
			}

			return executingBlocks;
		}

		public virtual string SubstituteVariables(string text)
		{
			string subbedText = text;
			
			// Instantiate the regular expression object.
			Regex r = new Regex("{\\$.*?}");
			
			// Match the regular expression pattern against a text string.
			var results = r.Matches(text);
			foreach (Match match in results)
			{
				string key = match.Value.Substring(2, match.Value.Length - 3);

				// Look for any matching variables in this Flowchart first (public or private)
				foreach (Variable variable in variables)
				{
					if (variable == null)
						continue;

					if (variable.key == key)
					{	
						string value = variable.ToString();
						subbedText = subbedText.Replace(match.Value, value);
					}
				}

				// Now search all public variables in all scene Flowcharts in the scene
				foreach (Flowchart flowchart in cachedFlowcharts)
				{
					if (flowchart == this)
					{
						// We've already searched this flowchart
						continue;
					}

					foreach (Variable variable in flowchart.variables)
					{
						if (variable == null)
							continue;
						
						if (variable.scope == VariableScope.Public &&
							variable.key == key)
						{	
							string value = variable.ToString();
							subbedText = subbedText.Replace(match.Value, value);
						}
					}
				}

				// Next look for matching localized string
				string localizedString = Localization.GetLocalizedString(key);
				if (localizedString != null)
				{
					subbedText = subbedText.Replace(match.Value, localizedString);
				}
			}
			
			return subbedText;
		}
	}

}