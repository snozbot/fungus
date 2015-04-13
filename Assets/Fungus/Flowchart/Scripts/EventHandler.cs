using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	
	public class EventHandlerInfoAttribute : Attribute
	{
		public EventHandlerInfoAttribute(string category, string eventHandlerName, string helpText)
		{
			this.Category = category;
			this.EventHandlerName = eventHandlerName;
			this.HelpText = helpText;
		}
		
		public string Category { get; set; }
		public string EventHandlerName { get; set; }
		public string HelpText { get; set; }
	}

	/**
	 * A Block may have an associated Event Handler which starts executing commands when
	 * a specific event occurs. 
	 * To create a custom Event Handler, simply subclass EventHandler and call the ExecuteBlock() method
	 * when the event occurs. 
	 * Add an EventHandlerInfo attibute and your new EventHandler class will automatically appear in the
	 * 'Execute On Event' dropdown menu when a block is selected.
	 */
	[RequireComponent(typeof(Block))]
	[RequireComponent(typeof(Flowchart))]
	[AddComponentMenu("")]
	public class EventHandler : MonoBehaviour
	{	
		[HideInInspector]
		[FormerlySerializedAs("parentSequence")]
		public Block parentBlock;

		/**
		 * The Event Handler should call this method when the event is detected.
		 */
		public virtual bool ExecuteBlock()
		{
			if (parentBlock == null)
			{
				return false;
			}

			if (parentBlock.eventHandler != this)
			{
				return false;
			}

			Flowchart flowchart = parentBlock.GetFlowchart();

			// Auto-follow the executing block if none is currently selected
			if (flowchart.selectedBlock == null)
			{
				flowchart.selectedBlock = parentBlock;
			}

			return flowchart.ExecuteBlock(parentBlock);
		}

		/**
		 * Returns a custom summary for the event handler.
		 */
		public virtual string GetSummary()
		{
			return "";
		}
	}
}
