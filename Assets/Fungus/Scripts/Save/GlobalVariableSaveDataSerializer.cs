// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the current state of the global variables. Achieved
    /// by using flowchart encode/decode on the GlobalVariables flowchart.
    /// </summary>
    public class GlobalVariableSaveDataSerializer : SaveDataSerializer
    {
        protected const string GlobalVarKey = "GlobalVarData";
        protected const int GlobalVarDataPriority = 500;

        public override string DataTypeKey => GlobalVarKey;

        public override int Order => GlobalVarDataPriority;

        public override void Encode(SavePointData data)
        {
            var gvd = GlobalVariableData.Encode();
            var saveDataItem = new SaveDataItem()
            {
                DataType = GlobalVarKey,
                Data = JsonUtility.ToJson(gvd)
            };
            data.SaveDataItems.Add(saveDataItem);
        }

        public override void Decode(SavePointData data)
        {
            FungusManager.Instance.GlobalVariables.ClearVars();
            base.Decode(data);
        }

        protected override void ProcessItem(SaveDataItem item)
        {
            var gvd = JsonUtility.FromJson<GlobalVariableData>(item.Data);
            if (gvd == null)
            {
                Debug.LogError("Failed to decode Global Variable save data item");
                return;
            }

            gvd.Decode();
        }
    }
}