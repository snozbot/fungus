// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//TODO needs doco update

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes a list of game objects to be saved for each Save Point.
    /// It knows how to encode / decode concrete game classes like Flowchart and FlowchartData.
    /// To extend the save system to handle other data types, just modify or subclass this component.
    /// </summary>
    public class FlowchartSaveDataSerializer : SaveDataSerializer
    {
        protected const string FlowchartDataKey = "FlowchartData";

        [Tooltip("A list of Flowchart objects whose variables will be encoded in the save data. Boolean, Integer, Float and String variables are supported.")]
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        public override string DataTypeKey => FlowchartDataKey;

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

        public override void Decode(SavePointData data)
        {
            DecodeMatchingItem(data, ProcessItem);
        }

        protected virtual void ProcessItem(SaveDataItem item)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartData>(item.Data);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode Flowchart save data item");
                return;
            }

            FlowchartData.Decode(flowchartData);
        }

        private void OnValidate()
        {
            foreach (var item in flowcharts)
            {
                if (item == null)
                    continue;

                if(flowcharts.Where(x=>x.GetName() == item.name).Count() > 1)
                {
                    Debug.LogError("FlowchartSaveData found duplicated name flowcharts: " + item.GetName() + " This is not allowed. All flowcharts saved must be unique with unique names.");
                    break;
                }
            }
        }
    }
}

#endif