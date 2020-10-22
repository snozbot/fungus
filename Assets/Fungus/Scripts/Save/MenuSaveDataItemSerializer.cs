// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the flowchart.block.command that kicked off a menu that is presently active.
    ///
    /// This is encoded as a separate item from FungusSystem and other Flowchart saving so that it can be forced
    /// to run after any other serialization that it may need to override.
    /// </summary>
    public class MenuSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string MenuKey = "MenuData";
        protected const int MenuDataPriority = 2000;

        public string DataTypeKey => MenuKey;
        public int Order => MenuDataPriority;

        protected List<FlowchartSaveDataItem.CachedBlockExecution> cachedBlockExecutions = new List<FlowchartSaveDataItem.CachedBlockExecution>();

        public void PreDecode()
        {
            cachedBlockExecutions.Clear();
        }

        public void PostDecode()
        {
            FlowchartSaveDataItem.ProcessCachedExecutions(cachedBlockExecutions);
        }

        public StringPair[] Encode()
        {
            var activeMenu = MenuDialog.ActiveMenuDialog;
            if (activeMenu == null || !activeMenu.isActiveAndEnabled || activeMenu.FirstTouchedByCommand == null)
                return Array.Empty<StringPair>();

            var cmd = activeMenu.FirstTouchedByCommand;
            var menuFlowchartCommandData = new FlowchartSaveDataItem()
            {
                flowchartName = cmd.GetFlowchart().GetName(),
            };

            menuFlowchartCommandData.blockDatas.Add(new FlowchartSaveDataItem.BlockData(cmd.ParentBlock.BlockName,
                cmd.CommandIndex,
                ExecutionState.Executing,
                cmd.ParentBlock.GetExecutionCount(),
                cmd.ParentBlock.PreviousActiveCommandIndex,
                cmd.ParentBlock.JumpToCommandIndex));

            return SaveDataItemUtils.CreateSingleElement(DataTypeKey, menuFlowchartCommandData);
        }

        public bool Decode(StringPair sdi)
        {
            var flowchartData = JsonUtility.FromJson<FlowchartSaveDataItem>(sdi.val);
            if (flowchartData == null)
            {
                Debug.LogError("Failed to decode save data item");
                return false;
            }

            var flowchart = FlowchartSaveDataItem.FindFlowchartByName(flowchartData.flowchartName);

            if (flowchart == null)
                return false;

            //we want to grab all executions and cache for later
            flowchartData.Decode(flowchart, cachedBlockExecutions);

            return true;
        }
    }
}
