#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class FungusScript : MonoBehaviour 
{
	public float stepTime;

	public Sequence startSequence;

	[System.NonSerialized]
	public Sequence activeSequence;
	
	public List<Variable> variables = new List<Variable>();

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

#if UNITY_EDITOR
		Selection.activeGameObject = sequence.gameObject;
#endif

		activeSequence = sequence;
		sequence.ExecuteNextCommand();
	}

	public Variable GetVariable(string key)
	{
		foreach (Variable v in variables)
		{
			if (v.key == key)
			{
				return v;
			}
		}
		return null;
	}	
}
