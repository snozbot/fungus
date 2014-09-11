using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class FungusScript : MonoBehaviour 
	{
		[System.NonSerialized]
		public Sequence executingSequence;

		[System.NonSerialized]
		public Command copyCommand;

		[HideInInspector]
		public int selectedAddCommandIndex;

		[HideInInspector]
		public System.Type selectedAddCommandType;

		[HideInInspector]
		public int selectedCommandCategoryIndex;

		[HideInInspector]
		public Vector2 scriptScrollPos;

		[HideInInspector]
		public Vector2 commandScrollPos;

		[HideInInspector]
		public float commandViewWidth = 300;

		public float stepTime;
		
		public Sequence startSequence;

		public Sequence selectedSequence;

		public Command selectedCommand;

		public bool startAutomatically = true;

		public bool colorCommands = true;

		public bool showSequenceObjects = false;

		public List<Variable> variables = new List<Variable>();

		void Start()
		{
			if (startAutomatically)
			{
				Execute();
			}
		}

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

		public void Execute()
		{
			if (startSequence == null)
			{
				return;
			}

			ExecuteSequence(startSequence);
		}

		public void ExecuteSequence(Sequence sequence)
		{
			if (sequence == null)
			{
				return;
			}

			executingSequence = sequence;
			selectedSequence = sequence;
			sequence.ExecuteNextCommand();
		}

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