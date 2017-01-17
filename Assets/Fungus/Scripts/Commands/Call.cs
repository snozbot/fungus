// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

namespace Fungus
{
    /// <summary>
    /// Supported modes for calling a block.
    /// </summary>
    public enum CallMode
    {
        /// <summary> Stop executing the current block after calling. </summary>
        Stop,
        /// <summary> Continue executing the current block after calling  </summary>
        Continue,
        /// <summary> Wait until the called block finishes executing, then continue executing current block. </summary>
        WaitUntilFinished
    }

    /// <summary>
    /// Execute another block in the same Flowchart as the command, or in a different Flowchart.
    /// </summary>
    [CommandInfo("Flow", 
                 "Call", 
                 "Execute another block in the same Flowchart as the command, or in a different Flowchart.")]
    [AddComponentMenu("")]
    public class Call : Command
    {
        [Tooltip("Flowchart which contains the block to execute. If none is specified then the current Flowchart is used.")]
        [SerializeField] protected Flowchart targetFlowchart;

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("Block to start executing")]
        [SerializeField] protected Block targetBlock;

        [Tooltip("Label to start execution at. Takes priority over startIndex.")]
        [SerializeField] protected StringData startLabel = new StringData();

        [Tooltip("Command index to start executing")]
        [FormerlySerializedAs("commandIndex")]
        [SerializeField] protected int startIndex;
    
        [Tooltip("Select if the calling block should stop or continue executing commands, or wait until the called block finishes.")]
        [SerializeField] protected CallMode callMode;

        #region Public members

        public override void OnEnter()
        {
            var flowchart = GetFlowchart();

            if (targetBlock != null)
            {
                // Check if calling your own parent block
                if (ParentBlock != null && ParentBlock.Equals(targetBlock))
                {
                    // Just ignore the callmode in this case, and jump to first command in list
                    Continue(0);
                    return;
                }

                // Callback action for Wait Until Finished mode
                Action onComplete = null;
                if (callMode == CallMode.WaitUntilFinished)
                {
                    onComplete = delegate {
                        flowchart.SelectedBlock = ParentBlock;
                        Continue();
                    };
                }

                // Find the command index to start execution at
                int index = startIndex;
                if (startLabel.Value != "")
                {
                    int labelIndex = targetBlock.GetLabelIndex(startLabel.Value);
                    if (labelIndex != -1)
                    {
                        index = labelIndex;
                    }
                }

                if (targetFlowchart == null ||
                    targetFlowchart.Equals(GetFlowchart()))
                {
                    // If the executing block is currently selected then follow the execution 
                    // onto the next block in the inspector.
                    if (flowchart.SelectedBlock == ParentBlock)
                    {
                        flowchart.SelectedBlock = targetBlock;
                    }

                    StartCoroutine(targetBlock.Execute(index, onComplete));
                }
                else
                {
                    // Execute block in another Flowchart
                    targetFlowchart.ExecuteBlock(targetBlock, index, onComplete);
                }
            }

            if (callMode == CallMode.Stop)
            {
                StopParentBlock();
            }
            else if (callMode == CallMode.Continue)
            {
                Continue();
            }
        }

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (targetBlock != null)
            {
                connectedBlocks.Add(targetBlock);
            }       
        }
        
        public override string GetSummary()
        {
            string summary = "";

            if (targetBlock == null)
            {
                summary = "<None>";
            }
            else
            {
                summary = targetBlock.BlockName;
            }

            switch (callMode)
            {
            case CallMode.Stop:
                summary += " : Stop";
                break;
            case CallMode.Continue:
                summary += " : Continue";
                break;
            case CallMode.WaitUntilFinished:
                summary += " : Wait";
                break;
            }

            return summary;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}