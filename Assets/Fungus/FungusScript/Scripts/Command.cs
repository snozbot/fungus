#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	
	public class CommandInfoAttribute : Attribute
	{
		public CommandInfoAttribute(string category, string commandName, string helpText)
		{
			this.Category = category;
			this.CommandName = commandName;
			this.HelpText = helpText;
		}

		public string Category { get; set; }
		public string CommandName { get; set; }
		public string HelpText { get; set; }
	}
	
	[RequireComponent(typeof(Sequence))]
	public class Command : MonoBehaviour 
	{
		[HideInInspector]
		public string errorMessage = "";

		[HideInInspector]
		public FungusScript parentFungusScript;

		[HideInInspector]
		public Sequence parentSequence;

		[HideInInspector]
		public int indentLevel;

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
			Continue(this);
		}

		public virtual void Continue(Command currentCommand)
		{
			OnExit();
			parentSequence.ExecuteNextCommand(currentCommand);
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

		public virtual bool HasReference(Variable variable)
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

		/**
		 * Indent offset for this command.
		 */
		public virtual int GetPreIndent()
		{
			return 0;
		}

		/**
		 * Indent offset for subsequent commands.
		 */
		public virtual int GetPostIndent()
		{
			return 0;
		}

		public virtual Color GetButtonColor()
		{
			return Color.white;
		}
	}

}