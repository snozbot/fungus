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
		public int indentLevel;

		public Sequence GetSequence()
		{
			return gameObject.GetComponent<Sequence>();
		}

		public FungusScript GetFungusScript()
		{
			Sequence s = GetSequence();
			if (s == null)
			{
				return null;
			}

			return s.GetFungusScript();
		}

		public bool IsExecuting()
		{
			Sequence sequence = GetSequence();
			if (sequence == null)
			{
				return false;
			}

			return (sequence.activeCommand == this);
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
			Sequence sequence = GetSequence();
			if (sequence != null)
			{
				sequence.ExecuteNextCommand(currentCommand);
			}
		}

		public virtual void Stop()
		{
			OnExit();
			Sequence sequence = GetSequence();
			if (sequence != null)
			{
				sequence.Stop();
			}
		}

		public virtual void ExecuteSequence(Sequence s)
		{
			OnExit();
			Sequence sequence = GetSequence();
			if (sequence != null)
			{
				sequence.Stop();
				FungusScript fungusScript = sequence.GetFungusScript();
				if (fungusScript != null)
				{
					fungusScript.ExecuteSequence(s);
				}
			}
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