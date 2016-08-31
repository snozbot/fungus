// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System;

namespace Fungus
{
    /// <summary>
    /// Attribute class for Fungus event handlers.
    /// </summary>
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

    /// <summary>
    /// A Block may have an associated Event Handler which starts executing commands when
    /// a specific event occurs. 
    /// To create a custom Event Handler, simply subclass EventHandler and call the ExecuteBlock() method
    /// when the event occurs. 
    /// Add an EventHandlerInfo attibute and your new EventHandler class will automatically appear in the
    /// 'Execute On Event' dropdown menu when a block is selected.
    /// </summary>
    [RequireComponent(typeof(Block))]
    [RequireComponent(typeof(Flowchart))]
    [AddComponentMenu("")]
    public class EventHandler : MonoBehaviour
    {   
        /// <summary>
        /// Returns the parent Block which owns this Event Handler.
        /// </summary>
        /// <value>The parent block.</value>
        [HideInInspector]
        [FormerlySerializedAs("parentSequence")]
        [SerializeField] protected Block parentBlock;
        public virtual Block ParentBlock { get { return parentBlock; } set { parentBlock = value; } }

        /// <summary>
        /// The Event Handler should call this method when the event is detected.
        /// </summary>
        public virtual bool ExecuteBlock()
        {
            if (parentBlock == null)
            {
                return false;
            }

            if (parentBlock._EventHandler != this)
            {
                return false;
            }

            Flowchart flowchart = parentBlock.GetFlowchart();

            // Auto-follow the executing block if none is currently selected
            if (flowchart.SelectedBlock == null)
            {
                flowchart.SelectedBlock = parentBlock;
            }

            return flowchart.ExecuteBlock(parentBlock);
        }

        /// <summary>
        /// Returns custom summary text for the event handler.
        /// </summary>
        public virtual string GetSummary()
        {
            return "";
        }
    }
}
