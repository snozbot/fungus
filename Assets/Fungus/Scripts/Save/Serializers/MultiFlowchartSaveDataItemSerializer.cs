// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    [System.Serializable]
    /// <summary>
    /// This component encodes and decodes the added flowcharts and their blocks and running states.
    /// </summary>
    public class MultiFlowchartSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string FlowchartDataKey = "FlowchartData";
        protected const int FlowchartDataPriority = 1000;

        [Tooltip("A list of Flowchart objects whose variables will be encoded in the save data. Boolean, Integer, Float and String variables are supported.")]
        public List<Flowchart> flowchartsToSave = new List<Flowchart>();

        public string DataTypeKey => FlowchartDataKey;

        public int Order => FlowchartDataPriority;

        public StringPair[] Encode()
        {
            foreach (var item in flowchartsToSave)
            {
                if (item == null)
                    continue;

                if (flowchartsToSave.Where(x => x.GetName() == item.name).Count() > 1)
                {
                    Debug.LogError("FlowchartSaveData found duplicated name flowcharts: " + item.GetName() + " This is not allowed. All flowcharts saved must be unique with unique names.");
                    return null;
                }
            }

            var retval = new StringPair[flowchartsToSave.Count];

            for (int i = 0; i < flowchartsToSave.Count; i++)
            {
                var flowchart = flowchartsToSave[i];
                if (flowchart == null)
                    continue;

                var flowchartData = FlowchartSaveDataItem.Encode(flowchart);

                var saveDataItem = new StringPair()
                {
                    key = DataTypeKey,
                    val = JsonUtility.ToJson(flowchartData)
                };
                retval[i] = saveDataItem;
            }

            return retval;
        }

        public bool Decode(StringPair sdi)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartSaveDataItem>(sdi.val);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode save data item");
                return false;
            }

            var flowchart = flowchartsToSave.FirstOrDefault(x => x.name == flowchartData.flowchartName);

            if (flowchart == null)
            {
                Debug.LogWarning("Could not find matching flowchart in set, none matching name " + flowchartData.flowchartName
                    + ".\nSkipping data block.");

                flowchart = FlowchartSaveDataItem.FindFlowchartByName(flowchartData.flowchartName);
            }

            if (flowchart == null)
                return false;

            //we want to grab all executions and cache for later
            flowchartData.Decode(flowchart, cachedBlockExecutions);

            return true;
        }

        protected List<FlowchartSaveDataItem.CachedBlockExecution> cachedBlockExecutions = new List<FlowchartSaveDataItem.CachedBlockExecution>();

        public void PreDecode()
        {
            cachedBlockExecutions.Clear();
        }

        public void PostDecode()
        {
            FlowchartSaveDataItem.ProcessCachedExecutions(cachedBlockExecutions);
        }
    }
}
