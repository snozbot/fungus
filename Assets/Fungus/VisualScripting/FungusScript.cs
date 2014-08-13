#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	public class FungusScript : MonoBehaviour 
	{
		public float stepTime;

		public Sequence startSequence;

		[System.NonSerialized]
		public Sequence executingSequence;

		public Sequence selectedSequence;

		public bool startAutomatically = false;

		public List<FungusVariable> variables = new List<FungusVariable>();

		void Start()
		{
			if (startAutomatically)
			{
				Execute();
			}
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

	#if UNITY_EDITOR
			Selection.activeGameObject = sequence.gameObject;
	#endif

			executingSequence = sequence;
			sequence.ExecuteNextCommand();
		}
	}

}