using Fungus.LionManeSaveSys;

namespace SaveSystemTests
{
    public class FlowchartColorVarSavingTests : FlowchartVarSavingTests<ColorVarSaver>
    {
        protected override string VariableHolderName => "ColorFlowchart";
        protected override VarSaver.ContentType SaveContentAs => VarSaver.ContentType.jsonString;
    }
}
