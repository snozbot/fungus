/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fungus
{

    [CommandInfo("Flow", 
                 "Call", 
                 "Execute another block in the same Flowchart as the command, or in a different Flowchart.")]
    [AddComponentMenu("")]
    public class Call : Command
    {   
        [Tooltip("Flowchart which contains the block to execute. If none is specified then the current Flowchart is used.")]
        public Flowchart targetFlowchart;

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("Block to start executing")]
        public Block targetBlock;

        [Tooltip("Command index to start executing")]
        [FormerlySerializedAs("commandIndex")]
        public int startIndex;
    
        public enum CallMode
        {
            Stop,               // Stop executing the current block after calling 
            Continue,           // Continue executing the current block after calling 
            WaitUntilFinished   // Wait until the called block finishes executing, then continue executing current block
        }

        [Tooltip("Select if the calling block should stop or continue executing commands, or wait until the called block finishes.")]
        public CallMode callMode;

        public override void OnEnter()
        {
            Flowchart flowchart = GetFlowchart();

            if (targetBlock != null)
            {
                // Check if calling your own parent block
                if (targetBlock == parentBlock)
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
                        flowchart.selectedBlock = parentBlock;
                        Continue();
                    };
                }

                if (targetFlowchart == null ||
                    targetFlowchart == GetFlowchart())
                {
                    // If the executing block is currently selected then follow the execution 
                    // onto the next block in the inspector.
                    if (flowchart.selectedBlock == parentBlock)
                    {
                        flowchart.selectedBlock = targetBlock;
                    }

                    StartCoroutine(targetBlock.Execute(startIndex, onComplete));
                }
                else
                {
                    // Execute block in another Flowchart
                    targetFlowchart.ExecuteBlock(targetBlock, startIndex, onComplete);
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
                summary = targetBlock.blockName;
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
    }

}