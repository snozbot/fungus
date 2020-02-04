// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the flowchart.block.command that kicked off
    /// a menu that is presently active.
    /// </summary>
    public class MenuSaveDataSerializer : SaveDataSerializer
    {
        protected const string MenuKey = "MenuData";
        protected const int MenuDataPriority = 2000;

        public override string DataTypeKey => MenuKey;
        public override int Order => MenuDataPriority;

        protected List<FlowchartData.CachedBlockExecution> cachedBlockExecutions = new List<FlowchartData.CachedBlockExecution>();

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
            cachedBlockExecutions.Clear();
            DecodeMatchingDataTypeItems(data);
        }

        protected override void ProcessItem(SaveDataItem item)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartData>(item.Data);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode Menu Data save data item");
                return;
            }

            //we want to grab all executions and cache for later
            flowchartData.Decode(null, true);
            cachedBlockExecutions.AddRange(flowchartData.CachedBlockExecutions);
        }

        public override void PostDecode()
        {
            base.PostDecode();
            FlowchartData.ProcessCachedExecutions(cachedBlockExecutions);
        }
    }
}
