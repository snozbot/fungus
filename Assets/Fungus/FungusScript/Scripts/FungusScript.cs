using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	/**
	 * Visual scripting controller for the Fungus Script programming language.
	 * FungusScript objects may be edited visually using the Fungus Script editor window.
	 */
	public class FungusScript : MonoBehaviour 
	{
		/**
		 * Currently executing sequence.
		 */
		[System.NonSerialized]
		public Sequence executingSequence;

		/**
		 * Scroll position of Fungus Script editor window (map view).
		 */
		[HideInInspector]
		public Vector2 scriptScrollPos;

		/**
		 * Scroll position of Fungus Script editor window (command view).
		 */
		[HideInInspector]
		public Vector2 commandScrollPos;

		/**
		 * Current width of command view
		 */
		[HideInInspector]
		public float commandViewWidth = 350;

		/**
		 * Currently selected sequence in the Fungus Script editor.
		 */
		[HideInInspector]
		public Sequence selectedSequence;
		
		/**
		 * Currently selected command in the Fungus Script editor.
		 */
		[HideInInspector]
		public Command selectedCommand;

		/**
		 * The list of variables that can be accessed by the Fungus Script.
		 */
		[HideInInspector]
		public List<Variable> variables = new List<Variable>();

		/**
		 * First sequence to execute when the Fungus Script executes.
		 */
		[Tooltip("First sequence to execute when the Fungus Script executes")]
		public Sequence startSequence;

		/**
		 * Execute this Fungus Script when the scene starts.
		 */
		[Tooltip("Execute this Fungus Script when the scene starts playing")]
		public bool executeOnStart = true;

		[System.Serializable]
		public class Settings
		{
			/**
			 * Slow down execution when playing in the editor to make it easier to visualise program flow.
			 */
			[Tooltip("Slow down execution in the editor to make it easier to visualise program flow")]
			public bool runSlowInEditor = true;

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
			 * Hides the child sequence game objects in the Hierarchy view.
			 * Deselect to inspect the child gameobjects and components that make up the Fungus Script.
			 */
			[Tooltip("Hides the child sequence game objects in the Hierarchy view")]
			public bool hideSequenceObjects = true;
		}

		/**
		 * Advanced configuration options for the Fungus Script.
		 */
		[Tooltip("Advanced configuration options for the Fungus Script")]
		public Settings settings;

		protected virtual void Start()
		{
			if (executeOnStart)
			{
				Execute();
			}
		}

		/**
		 * Create a new sequence node which you can then add commands to.
		 */
		public virtual Sequence CreateSequence(Vector2 position)
		{
			GameObject go = new GameObject("Sequence");
			go.transform.parent = transform;
			go.transform.hideFlags = HideFlags.HideInHierarchy;
			Sequence s = go.AddComponent<Sequence>();
			s.nodeRect.x = position.x;
			s.nodeRect.y = position.y;
			return s;
		}

		/**
		 * Start running the Fungus Script by executing the startSequence.
		 */
		public virtual void Execute()
		{
			if (startSequence == null)
			{
				return;
			}

			ExecuteSequence(startSequence);
		}

		/**
		 * Start running the Fungus Script by executing a specific child sequence.
		 */
		public virtual void ExecuteSequence(Sequence sequence)
		{
			// Sequence must be a child of the parent Fungus Script
			if (sequence == null ||
			    sequence.transform.parent != transform) 
			{
				return;
			}

			executingSequence = sequence;
			selectedSequence = sequence;
			sequence.ExecuteNextCommand();
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
		 * Returns false if the variable key does not exist.
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
		 * Returns false if the variable key does not exist.
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
		 * Returns false if the variable key does not exist.
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
			Sequence[] sequences = GetComponentsInChildren<Sequence>();
			foreach (Sequence sequence in sequences)
			{
				sequence.gameObject.hideFlags = settings.hideSequenceObjects ? HideFlags.HideInHierarchy : HideFlags.None;
			}
		}
	}

}