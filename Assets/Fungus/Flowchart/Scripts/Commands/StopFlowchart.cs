/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    [CommandInfo("Flow", 
                 "Stop Flowchart", 
                 "Stops execution of all Blocks in a Flowchart")]
    [AddComponentMenu("")]
    public class StopFlowchart : Command
    {       
        [Tooltip("Stop all executing Blocks in the Flowchart that contains this command")]
        public bool stopParentFlowchart;

        [Tooltip("Stop all executing Blocks in a list of target Flowcharts")]
        public List<Flowchart> targetFlowcharts = new List<Flowchart>();

        public override void OnEnter()
        {
            Flowchart flowchart = GetFlowchart();

            if (stopParentFlowchart)
            {
                flowchart.StopAllBlocks();
            }

            foreach (Flowchart f in targetFlowcharts)
            {
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
    }

}