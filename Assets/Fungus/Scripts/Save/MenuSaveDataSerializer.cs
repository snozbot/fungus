// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the flowchart.block.command that kicked off a menu that is presently active.
    /// </summary>
    public class MenuSaveDataSerializer : BaseFlowchartSaveDataSerializer
    {
        protected const string MenuKey = "MenuData";
        protected const int MenuDataPriority = 2000;

        public override string DataTypeKey => MenuKey;
        public override int Order => MenuDataPriority;

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

                menuFlowchartCommandData.AddBlockData(new FlowchartData.BlockData( cmd.ParentBlock.BlockName,
                    cmd.CommandIndex,
                    ExecutionState.Executing,
                    cmd.ParentBlock.GetExecutionCount()));

                var menuLogitem = SaveDataItem.Create(MenuKey, JsonUtility.ToJson(menuFlowchartCommandData));
                data.SaveDataItems.Add(menuLogitem);
            }
        }
    }
}