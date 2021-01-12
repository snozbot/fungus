// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System;

namespace Fungus
{
    /// <summary>
    /// Attribute class for Fungus event handlers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
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
        [HideInInspector]
        [FormerlySerializedAs("parentSequence")]
        [SerializeField] protected Block parentBlock;

        [Tooltip("If true, the flowchart window will not auto select the Block when the Event Handler fires. Affects Editor only.")]
        [SerializeField] protected bool suppressBlockAutoSelect = false;

        #region Public members

        /// <summary>
        /// The parent Block which owns this Event Handler.
        /// </summary>
        public virtual Block ParentBlock { get { return parentBlock; } set { parentBlock = value; } }

        /// <summary>
        /// The Event Handler should call this method when the event is detected to start executing the Block.
        /// </summary>
        public virtual bool ExecuteBlock()
        {
            if (ParentBlock == null)
            {
                return false;
            }

            if (ParentBlock._EventHandler != this)
            {
                return false;
            }

            var flowchart = ParentBlock.GetFlowchart();

            //if somehow the flowchart is invalid or has been disabled we don't want to continue
            if(flowchart == null || !flowchart.isActiveAndEnabled)
            {
                return false;
            }

            if (suppressBlockAutoSelect)
            {
                ParentBlock.SuppressNextAutoSelection = true;
            }

            return flowchart.ExecuteBlock(ParentBlock,0);
        }

        /// <summary>
        /// Returns custom summary text for the event handler.
        /// </summary>
        public virtual string GetSummary()
        {
            return "";
        }

        /// <summary>
        /// Optional method for child classes to add additional data to a save via flowchartData.AddToVisitorPairs.
        /// To be used if the child command has mutable state that it needs restored, in VisitDecode.
        /// </summary>
        /// <param name="flowchartData"></param>
        public virtual void VisitEncode(FlowchartSaveDataItem flowchartData)
        {
        }

        /// <summary>
        /// Optional method for child classes to restore previously saved data from a save that is being loaded, via
        /// flowchartData.TryGetVisitorValueByKey restoring internal state from previously saved values.
        /// </summary>
        /// <param name="flowchartData"></param>
        public virtual void VisitDecode(FlowchartSaveDataItem flowchartData)
        {
        }
        #endregion
    }
}
