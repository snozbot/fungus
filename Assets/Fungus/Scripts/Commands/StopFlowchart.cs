// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Stops execution of all Blocks in a Flowchart.
    /// </summary>
    [CommandInfo("Flow", 
                 "Stop Flowchart", 
                 "Stops execution of all Blocks in a Flowchart")]
    [AddComponentMenu("")]
    public class StopFlowchart : Command
    {       
        [Tooltip("Stop all executing Blocks in the Flowchart that contains this command")]
        [SerializeField] protected bool stopParentFlowchart;

        [Tooltip("Stop all executing Blocks in a list of target Flowcharts")]
        [SerializeField] protected List<Flowchart> targetFlowcharts = new List<Flowchart>();

        #region Public members

        public override void OnEnter()
        {
            var flowchart = GetFlowchart();

            if (stopParentFlowchart)
            {
                flowchart.StopAllBlocks();
            }

            for (int i = 0; i < targetFlowcharts.Count; i++)
            {
                var f = targetFlowcharts[i];
                if (f == flowchart)
                {
                    // Flowchart has already been stopped
                    continue;
                }
                f.StopAllBlocks();
            }
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetFlowcharts")
            {
                return true;
            }

            return false;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}