#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using Fungus;

public class SequenceController : MonoBehaviour 
{
	public float stepTime;

	public Sequence activeSequence;

	public void ExecuteSequence(Sequence sequence)
	{
		if (activeSequence == null)
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
