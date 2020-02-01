// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;

//TODO needs doco update

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes a list of game objects to be saved for each Save Point.
    /// It knows how to encode / decode concrete game classes like Flowchart and FlowchartData.
    /// To extend the save system to handle other data types, just modify or subclass this component.
    /// </summary>
    public class GlobalVariableSaveDataSerializer : SaveDataSerializer
    {
        protected const string GlobalVarKey = "GlobalVarData";

        public override string DataTypeKey => GlobalVarKey;

        public override void Encode(SavePointData data)
        {
            var gvFlowchartData = FlowchartData.Encode(FungusManager.Instance.GlobalVariables.GlobalVariableFlowchart);
            var gvDataItem = SaveDataItem.Create(GlobalVarKey, JsonUtility.ToJson(gvFlowchartData));
            data.SaveDataItems.Add(gvDataItem);
        }

        public override void Decode(SavePointData data)
        {
            FungusManager.Instance.GlobalVariables.ClearVars();
            DecodeMatchingItem(data, ProcessItem);
        }

        protected virtual void ProcessItem(SaveDataItem item)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartData>(item.Data);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode Global Variable Flowchart save data item");
                return;
            }

            FlowchartData.Decode(flowchartData, FungusManager.Instance.GlobalVariables.GlobalVariableFlowchart);
        }
    }
}

#endif