// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the added flowcharts and their blocks and running states.
    /// </summary>
    public class MultiFlowchartSaveDataSerializer : BaseFlowchartSaveDataSerializer
    {
        protected const string FlowchartDataKey = "FlowchartData";
        protected const int FlowchartDataPriority = 1000;

        [Tooltip("A list of Flowchart objects whose variables will be encoded in the save data. Boolean, Integer, Float and String variables are supported.")]
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        public override string DataTypeKey => FlowchartDataKey;

        public override int Order => FlowchartDataPriority;

        public override void Encode(SavePointData data)
        {
            for (int i = 0; i < flowcharts.Count; i++)
            {
                var flowchart = flowcharts[i];
                if (flowchart == null)
                    continue;

                var flowchartData = FlowchartData.Encode(flowchart);

                var saveDataItem = SaveDataItem.Create(FlowchartDataKey, JsonUtility.ToJson(flowchartData));
                data.SaveDataItems.Add(saveDataItem);
            }
        }

        protected override void ProcessItem(SaveDataItem item)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartData>(item.Data);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode save data item");
                return;
            }

            var flowchart = flowcharts.FirstOrDefault(x => x.name == flowchartData.FlowchartName);

            if(flowchart == null)
            {
                Debug.LogError("Could not find matching flowchart in set, none matching name " + flowchartData.FlowchartName 
                    + ".\nSkipping data block.");
                return;
            }

            //we want to grab all executions and cache for later
            flowchartData.Decode(flowchart, cachedBlockExecutions);
        }

        private void OnValidate()
        {
            foreach (var item in flowcharts)
            {
                if (item == null)
                    continue;

                if (flowcharts.Where(x => x.GetName() == item.name).Count() > 1)
                {
                    Debug.LogError("FlowchartSaveData found duplicated name flowcharts: " + item.GetName() + " This is not allowed. All flowcharts saved must be unique with unique names.");
                    break;
                }
            }
        }
    }
}