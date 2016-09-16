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
    public class EventHandler : MonoBehaviour, IEventHandler
    {   
        [HideInInspector]
        [FormerlySerializedAs("parentSequence")]
        [SerializeField] protected Block parentBlock;

        #region IEventHandler

        public virtual IBlock ParentBlock { get { return parentBlock; } set { parentBlock = (Block)value; } }

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

            var flowchart = parentBlock.GetFlowchart();

            // Auto-follow the executing block if none is currently selected
            if (flowchart.SelectedBlock == null)
            {
                flowchart.SelectedBlock = parentBlock;
            }

            return flowchart.ExecuteBlock(parentBlock);
        }

        public virtual string GetSummary()
        {
            return "";
        }

        #endregion
    }
}
