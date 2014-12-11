using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{

	/**
	 * Visual scripting controller for the Fungus Script programming language.
	 * FungusScript objects may be edited visually using the Fungus Script editor window.
	 */
	[AddComponentMenu("Fungus/Fungus Script")]
	public class FungusScript : MonoBehaviour 
	{
		/**
		 * Scroll position of Fungus Script editor window.
		 */
		[HideInInspector]
		public Vector2 scrollPos;

		/**
		 * Scroll position of Fungus Script variables window.
		 */
		[HideInInspector]
		public Vector2 variablesScrollPos;

		[HideInInspector]
		public bool variablesExpanded = true;

		/**
		 * Zoom level of Fungus Script editor window
		 */
		[HideInInspector]
		public float zoom = 1f;

		/**
		 * Scrollable area for Fungus Script editor window.
		 */
		[HideInInspector]
		public Rect scrollViewRect;

		/**
		 * Currently selected sequence in the Fungus Script editor.
		 */
		[HideInInspector]
		public Sequence selectedSequence;
		
		/**
		 * Currently selected command in the Fungus Script editor.
		 */
		[HideInInspector]
		public List<Command> selectedCommands = new List<Command>();

		/**
		 * The list of variables that can be accessed by the Fungus Script.
		 */
		[HideInInspector]
		public List<Variable> variables = new List<Variable>();

		[TextArea(3, 5)]
		[Tooltip("Description text displayed in the Fungus Script editor window")]
		public string description = "";

		/**
	 	 * Minimum time for each command to execute when runSlowInEditor is enabled.
	 	 */
		[Range(0f, 5f)]
		[Tooltip("Minimum time that each command will take to execute when Run Slow In Editor is enabled")]
		public float runSlowDuration = 0.25f;

		/**
		 * Use command color when displaying the command list in the Fungus Editor window.
		 */
		[Tooltip("Use command color when displaying the command list in the Fungus Editor window")]
		public bool colorCommands = true;
		
		/**
		 * Hides the Fungus Script sequence and command components in the inspector.
		 * Deselect to inspect the sequence and command components that make up the Fungus Script.
		 */
		[Tooltip("Hides the Fungus Script sequence and command components in the inspector")]
		public bool hideComponents = true;

		protected virtual Sequence CreateSequenceComponent(GameObject parent)
		{
			Sequence s = parent.AddComponent<Sequence>();
			return s;
		}

		/**
		 * Create a new sequence node which you can then add commands to.
		 */
		public virtual Sequence CreateSequence(Vector2 position)
		{
			Sequence s = CreateSequenceComponent(gameObject);
			s.nodeRect.x = position.x;
			s.nodeRect.y = position.y;
			s.sequenceName = GetUniqueSequenceKey(s.sequenceName, s);
			return s;
		}

		/**
		 * Start running another Fungus Script by executing a specific child sequence.
		 * The sequence must be in an idle state to be executed.
		 * Returns true if the Sequence started execution.
		 */
		public virtual bool ExecuteSequence(string sequenceName)
		{
			Sequence [] sequences = GetComponentsInChildren<Sequence>();
			foreach (Sequence sequence in sequences)
			{
				if (sequence.sequenceName == sequenceName)
				{
					return ExecuteSequence(sequence);
				}
			}

			return false;
		}

		/**
		 * Sends a message to this Fungus Script only.
		 * Any sequence with a matching MessageReceived event handler will start executing.
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
		 * Sends a message to all Fungus Script objects in the current scene.
		 * Any sequence with a matching MessageReceived event handler will start executing.
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
		 * Start running another Fungus Script by executing a specific child sequence.
		 * The sequence must be in an idle state to be executed.
		 * Returns true if the Sequence started execution.
		 */
		public virtual bool ExecuteSequence(Sequence sequence)
		{
			// Sequence must be a component of the Fungus Script game object
			if (sequence == null ||
			    sequence.gameObject != gameObject) 
			{
				return false;
			}

			// Can't restart a running sequence, have to wait until it's idle again
			if (sequence.IsExecuting())
			{
				return false;
			}

			// Execute the first command in the command list
			sequence.ExecuteNextCommand();

			return true;
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
					if (variable == ignoreVariable ||
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
		 * Returns a new Sequence key that is guaranteed not to clash with any existing Sequence in the Fungus Script.
		 */
		public virtual string GetUniqueSequenceKey(string originalKey, Sequence ignoreSequence = null)
		{
			int suffix = 0;
			string baseKey = originalKey.Trim();
			
			// No empty keys allowed
			if (baseKey.Length == 0)
			{
				baseKey = "Sequence";
			}

			Sequence[] sequences = GetComponentsInChildren<Sequence>();

			string key = baseKey;
			while (true)
			{
				bool collision = false;
				foreach(Sequence sequence in sequences)
				{
					if (sequence == ignoreSequence ||
					    sequence.sequenceName == null)
					{
						continue;
					}
					
					if (sequence.sequenceName.Equals(key, StringComparison.CurrentCultureIgnoreCase))
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
		 * Gets the value of a boolean variable.
		 * Returns false if the variable key does not exist.
		 */
		public virtual bool GetBooleanVariable(string key)
		{
			foreach (Variable v in variables)
			{
				if (v.key == key)
				{
					BooleanVariable variable = v as BooleanVariable;
					if (variable != null)
					{
						return variable.Value;
					}
				}
			}
			Debug.LogWarning("Boolean variable " + key + " not found.");
			return false;
		}
					
		/**
		 * Sets the value of a boolean variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public virtual void SetBooleanVariable(string key, bool value)
		{
			foreach (Variable v in variables)
			{
				if (v.key == key)
				{
					BooleanVariable variable = v as BooleanVariable;
					if (variable != null)
					{
						variable.Value = value;
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
				if (v.key == key)
				{
					IntegerVariable variable = v as IntegerVariable;
					if (variable != null)
					{
						return variable.Value;
					}
				}
			}
			Debug.LogWarning("Integer variable " + key + " not found.");
			return 0;
		}

		/**
		 * Sets the value of an integer variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public virtual void SetIntegerVariable(string key, int value)
		{
			foreach (Variable v in variables)
			{
				if (v.key == key)
				{
					IntegerVariable variable = v as IntegerVariable;
					if (variable != null)
					{
						variable.Value = value;
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
				if (v.key == key)
				{
					FloatVariable variable = v as FloatVariable;
					if (variable != null)
					{
						return variable.Value;
					}
				}
			}
			Debug.LogWarning("Float variable " + key + " not found.");
			return 0f;
		}
				
		/**
		 * Sets the value of a float variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public virtual void SetFloatVariable(string key, float value)
		{
			foreach (Variable v in variables)
			{
				if (v.key == key)
				{
					FloatVariable variable = v as FloatVariable;
					if (variable != null)
					{
						variable.Value = value;
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
				if (v.key == key)
				{
					StringVariable variable = v as StringVariable;
					if (variable != null)
					{
						return variable.Value;
					}
				}
			}
			Debug.LogWarning("String variable " + key + " not found.");
			return "";
		}

		/**
		 * Sets the value of a string variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public virtual void SetStringVariable(string key, string value)
		{
			foreach (Variable v in variables)
			{
				if (v.key == key)
				{
					StringVariable variable = v as StringVariable;
					if (variable != null)
					{
						variable.Value = value;
						return;
					}
				}
			}
			Debug.LogWarning("String variable " + key + " not found.");
		}

		/**
		 * Set the sequence objects to be hidden or visible depending on the showSequenceObjects property.
		 */
		public virtual void UpdateHideFlags()
		{
			if (hideComponents)
			{
				Sequence[] sequences = GetComponentsInChildren<Sequence>();
				foreach (Sequence sequence in sequences)
				{
					sequence.hideFlags = HideFlags.HideInInspector;
					if (sequence.gameObject != gameObject)
					{
						sequence.gameObject.hideFlags = HideFlags.HideInHierarchy;
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

		public virtual void Reset(bool resetCommands, bool resetLocalVariables, bool resetGlobalVariables)
		{
			if (resetCommands)
			{
				Command[] commands = GetComponentsInChildren<Command>();
				foreach (Command command in commands)
				{
					command.OnReset();
				}
			}

			foreach (Variable variable in variables)
			{
				if (resetLocalVariables &&
				    variable.scope == VariableScope.Local)
				{
					variable.OnReset();
				}
				else if (resetGlobalVariables &&
				         variable.scope == VariableScope.Global)
				{
					variable.OnReset();
				}
			}
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
				foreach (Variable variable in variables)
				{
					if (variable.key == key)
					{	
						string value = variable.ToString();
						subbedText = subbedText.Replace(match.Value, value);
						break;
					}
				}
			}
			
			return subbedText;
		}
	}

}