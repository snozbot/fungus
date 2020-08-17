// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Common save data serialisation actions for Flowchart related data serialisers.
    ///
    /// At present this means Menu and MultiFlowchart.
    /// </summary>
    public abstract class BaseFlowchartSaveDataSerializer : SaveDataSerializer
    {
        protected List<FlowchartData.CachedBlockExecution> cachedBlockExecutions = new List<FlowchartData.CachedBlockExecution>();

        public override void PreDecode()
        {
            base.PreDecode();
            cachedBlockExecutions.Clear();
        }

        //this is the naive, surely there is a matching flowchart somwhere method
        protected override void ProcessItem(SaveDataItem item)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartData>(item.Data);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode save data item");
                return;
            }

            //we want to grab all executions and cache for later
            flowchartData.Decode(null, cachedBlockExecutions);
        }

        public override void PostDecode()
        {
            base.PostDecode();
            FlowchartData.ProcessCachedExecutions(cachedBlockExecutions);
        }
    }
}