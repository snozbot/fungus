#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	
	public class CommandInfoAttribute : Attribute
	{
		public CommandInfoAttribute(string category, string commandName, string helpText, byte red, byte green, byte blue)
		{
			this.Category = category;
			this.CommandName = commandName;
			this.HelpText = helpText;
			this.ButtonColor = new Color32(red, green, blue, 255);
		}

		public string Category { get; set; }
		public string CommandName { get; set; }
		public string HelpText { get; set; }
		public Color ButtonColor { get; set; }
	}
	
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
			parentFungusScript = GetFungusScript();
		}

		public FungusScript GetFungusScript()
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

		public virtual void Continue()
		{
			OnExit();
			parentSequence.ExecuteNextCommand(this);
		}

		public virtual void Stop()
		{
			OnExit();
			parentSequence.Stop();
		}

		public virtual void ExecuteSequence(Sequence s)
		{
			OnExit();
			parentSequence.Stop();
			parentFungusScript.ExecuteSequence(s);
		}

		public virtual void OnEnter()
		{}

		public virtual void OnExit()
		{}

		public virtual void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{}

		public virtual bool HasReference(FungusVariable variable)
		{
			return false;
		}

		public virtual string GetSummary()
		{
			return "";
		}

		public virtual string GetHelpText()
		{
			return "";
		}
	}

}