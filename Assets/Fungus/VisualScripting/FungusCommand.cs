#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	
	public class FungusCommand : MonoBehaviour 
	{
		[HideInInspector]
		public string errorMessage = "";

		[HideInInspector]
		public SequenceController parentSequenceController;

		[HideInInspector]
		public Sequence parentSequence;

		public virtual void Start()
		{
			parentSequence = GetComponent<Sequence>();

			// Populate sequenceController reference
			Transform parent = transform.parent;		
			while (parent != null)
			{
				parentSequenceController = parent.gameObject.GetComponent<SequenceController>();
				if (parentSequenceController != null)
				{
					break;
				}
			}
		}

		public bool IsExecuting()
		{
			if (parentSequence == null)
			{
				return false;
			}

			return (parentSequence.activeCommand == this);
		}

		public virtual void Execute()
		{
			OnEnter();
			OnExecute();
		}

		public virtual void ExecuteNextCommand()
		{
			OnExit();
			parentSequence.ExecuteNextCommand(this);
		}

		public virtual void ExecuteSequence(Sequence s)
		{
			OnExit();
			parentSequence.Finish();
			parentSequenceController.ExecuteSequence(s);
		}

		public virtual void OnEnter()
		{}

		public virtual void OnExit()
		{}

		public virtual void OnExecute()
		{}

		public virtual void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{}
	}

}