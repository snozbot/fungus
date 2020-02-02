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
    public class MenuSaveDataSerializer : SaveDataSerializer
    {
        protected const string MenuKey = "MenuData";

        public override string DataTypeKey => MenuKey;

        public override void Encode(SavePointData data)
        {
            var activeMenu = MenuDialog.ActiveMenuDialog;
            if (activeMenu != null && activeMenu.isActiveAndEnabled && activeMenu.FirstTouchedByCommand != null)
            {
                var cmd = activeMenu.FirstTouchedByCommand;
                var menuFlowchartCommandData = new FlowchartData()
                {
                    FlowchartName = cmd.GetFlowchart().GetName(),
                };

                menuFlowchartCommandData.AddBlockData(new FlowchartData.BlockData()
                {
                    blockName = cmd.ParentBlock.BlockName,
                    commandIndex = cmd.CommandIndex,
                    executionState = ExecutionState.Executing,
                });

                var menuLogitem = SaveDataItem.Create(MenuKey, JsonUtility.ToJson(menuFlowchartCommandData));
                data.SaveDataItems.Add(menuLogitem);
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
                Debug.LogError("Failed to decode Menu Data save data item");
                return;
            }

            FlowchartData.Decode(flowchartData);
        }
    }
}

#endif
