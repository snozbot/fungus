#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using Fungus;

public class SequenceController : MonoBehaviour 
{
	public float stepTime;

	public Sequence startSequence;

	[System.NonSerialized]
	public Sequence activeSequence;

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
}
