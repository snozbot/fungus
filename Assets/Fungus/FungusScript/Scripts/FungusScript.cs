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
		 * Copy and paste buffer for command objects.
		 */
		[System.NonSerialized]
		public Command copyCommand;

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
		 * Execution speed when running the script in the editor. Pauses on each command to help visualise execution order.
		 */
		public float stepTime;

		/**
		 * First sequence to execute when the Fungus Script executes.
		 */
		public Sequence startSequence;

		/**
		 * Currently selected sequence in the Fungus Script editor.
		 */
		public Sequence selectedSequence;

		/**
		 * Currently selected command in the Fungus Script editor.
		 */
		public Command selectedCommand;

		/**
		 * Execute this Fungus Script when the scene starts.
		 */
		public bool executeOnStart = true;

		/**
		 * Use command color when displaying command list in Fungus Editor window.
		 */
		public bool colorCommands = true;

		/**
		 * Show the sequence game objects in the Hierarchy view.
		 * This can be useful if you want to inspect the child gameobjects and components that make up the Fungus Script.
		 */
		public bool showSequenceObjects = false;

		/**
		 * The list of variables that can be accessed by the Fungus Script.
		 */
		public List<Variable> variables = new List<Variable>();

		void Start()
		{
			if (executeOnStart)
			{
				Execute();
			}
		}

		/**
		 * Create a new sequence node which you can then add commands to.
		 */
		public Sequence CreateSequence(Vector2 position)
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
		public void Execute()
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
		public void ExecuteSequence(Sequence sequence)
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
		public string GetUniqueVariableKey(string originalKey, Variable ignoreVariable = null)
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
		 * Sets the value of a boolean variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public void SetBooleanVariable(string key, bool value)
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
		 * Sets the value of an integer variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public void SetIntegerVariable(string key, int value)
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
		 * Sets the value of a float variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public void SetFloatVariable(string key, float value)
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
		 * Sets the value of a string variable.
		 * The variable must already be added to the list of variables for this Fungus Script.
		 */
		public void SetStringVariable(string key, string value)
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
		public void UpdateHideFlags()
		{
			Sequence[] sequences = GetComponentsInChildren<Sequence>();
			foreach (Sequence sequence in sequences)
			{
				sequence.gameObject.hideFlags = showSequenceObjects ? HideFlags.None : HideFlags.HideInHierarchy;
			}
		}
	}

}