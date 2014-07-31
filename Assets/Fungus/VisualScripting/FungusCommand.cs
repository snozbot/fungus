#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[RequireComponent(typeof(Sequence))]
	public class FungusCommand : MonoBehaviour 
	{
		[HideInInspector]
		public string errorMessage = "";

		[HideInInspector]
		public FungusScript parentFungusScript;

		[HideInInspector]
		public Sequence parentSequence;

		public virtual void Start()
		{
			parentSequence = GetComponent<Sequence>();
			parentFungusScript = GetParentFungusScript();
		}

		public FungusScript GetParentFungusScript()
		{
			FungusScript sc = null;

			Transform parent = transform.parent;		
			while (parent != null)
			{
				sc = parent.gameObject.GetComponent<FungusScript>();
				if (sc != null)
				{
					break;
				}
				parent = parent.transform.parent;
			}
			return sc;
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
		}

		public virtual void Finish()
		{
			OnExit();
			parentSequence.ExecuteNextCommand(this);
		}

		public virtual void ExecuteSequence(Sequence s)
		{
			OnExit();
			parentSequence.Finish();
			parentFungusScript.ExecuteSequence(s);
		}

		public virtual void OnEnter()
		{}

		public virtual void OnExit()
		{}

		public virtual void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{}
	}

}