// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the flowchart.block.command that kicked off a menu that is presently active.
    /// </summary>
    public class MenuSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string MenuKey = "MenuData";
        protected const int MenuDataPriority = 2000;

        public string DataTypeKey => MenuKey;
        public int Order => MenuDataPriority;

        protected List<FlowchartDataItem.CachedBlockExecution> cachedBlockExecutions = new List<FlowchartDataItem.CachedBlockExecution>();

        public void PreDecode()
        {
            cachedBlockExecutions.Clear();
        }

        public void PostDecode()
        {
            FlowchartDataItem.ProcessCachedExecutions(cachedBlockExecutions);
        }

        public SaveDataItem[] Encode()
        {
            var activeMenu = MenuDialog.ActiveMenuDialog;
            if (activeMenu == null || !activeMenu.isActiveAndEnabled || activeMenu.FirstTouchedByCommand == null)
                return Array.Empty<SaveDataItem>();

            var cmd = activeMenu.FirstTouchedByCommand;
            var menuFlowchartCommandData = new FlowchartDataItem()
            {
                flowchartName = cmd.GetFlowchart().GetName(),
            };

            menuFlowchartCommandData.blockDatas.Add(new FlowchartDataItem.BlockData(cmd.ParentBlock.BlockName,
                cmd.CommandIndex,
                ExecutionState.Executing,
                cmd.ParentBlock.GetExecutionCount(),
                cmd.ParentBlock.PreviousActiveCommandIndex,
                cmd.ParentBlock.JumpToCommandIndex));

            var sdi = new SaveDataItem()
            {
                DataType = DataTypeKey,
                Data = JsonUtility.ToJson(menuFlowchartCommandData)
            };

            return new SaveDataItem[] { sdi };
        }

        public bool Decode(SaveDataItem sdi)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartDataItem>(sdi.Data);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode save data item");
                return false;
            }

            var flowchart = FlowchartDataItem.FindFlowchartByName(flowchartData.flowchartName);

            if (flowchart == null)
                return false;

            //we want to grab all executions and cache for later
            flowchartData.Decode(flowchart, cachedBlockExecutions);

            return true;
        }
    }
}