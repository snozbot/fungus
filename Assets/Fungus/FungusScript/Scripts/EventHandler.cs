using UnityEngine;
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
	 * A Sequence may have an associated Event Handler which starts executing the sequence when
	 * a specific event occurs. 
	 * To create a custom Event Handler, simply subclass EventHandler and call the ExecuteSequence() method
	 * when the event occurs. 
	 * Add an EventHandlerInfo attibute and your new EventHandler class will automatically appear in the
	 * 'Execute On Event' dropdown menu when a sequence is selected.
	 */
	[RequireComponent(typeof(Sequence))]
	[RequireComponent(typeof(FungusScript))]
	public class EventHandler : MonoBehaviour
	{	
		[HideInInspector]
		public Sequence parentSequence;

		/**
		 * Returns the class attribute info for an event handler class.
		 */
		public static EventHandlerInfoAttribute GetEventHandlerInfo(System.Type eventHandlerType)
		{
			object[] attributes = eventHandlerType.GetCustomAttributes(typeof(EventHandlerInfoAttribute), false);
			foreach (object obj in attributes)
			{
				EventHandlerInfoAttribute eventHandlerInfoAttr = obj as EventHandlerInfoAttribute;
				if (eventHandlerInfoAttr != null)
				{
					return eventHandlerInfoAttr;
				}
			}
			
			return null;
		}

		/**
		 * The Event Handler should call this method when the event is detected.
		 */
		public virtual bool ExecuteSequence()
		{
			if (parentSequence == null)
			{
				return false;
			}

			FungusScript fungusScript = parentSequence.GetFungusScript();
			return fungusScript.ExecuteSequence(parentSequence);
		}

		/**
		 * Returns a custom summary for the event handler.
		 * If the string is empty, the editor will use the EventHandlerName property of 
		 * the EventHandlerInfo attribute instead.
		 */
		public virtual string GetSummary()
		{
			EventHandlerInfoAttribute info = GetEventHandlerInfo(this.GetType());
			if (info == null)
			{
				return "";
			}

			return info.EventHandlerName;
		}
	}
}
