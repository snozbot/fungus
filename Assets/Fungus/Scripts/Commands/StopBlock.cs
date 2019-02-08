// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stops executing the named Block.
    /// </summary>
    [CommandInfo("Flow", 
                 "Stop Block", 
                 "Stops executing the named Block")]
    public class StopBlock : Command 
    {
        [Tooltip("Flowchart containing the Block. If none is specified, the parent Flowchart is used.")]
        [SerializeField] protected Flowchart flowchart;

        [Tooltip("Name of the Block to stop")]
        [SerializeField] protected StringData blockName = new StringData("");

        #region Public members

        public override void OnEnter()
        {
            if (blockName.Value == "")
            {
                Continue();
            }

            if (flowchart == null)
            {
                flowchart = (Flowchart)GetFlowchart();
            }

            var block = flowchart.FindBlock(blockName.Value);
            if (block == null ||
                !block.IsExecuting())
            {
                Continue();
            }

            block.Stop();

            Continue();
        }

        public override string GetSummary()
        {
            return blockName;
        }
            
        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return blockName.stringRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}