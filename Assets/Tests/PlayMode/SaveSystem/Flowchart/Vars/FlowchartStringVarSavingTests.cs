using Fungus.LionManeSaveSys;

namespace SaveSystemTests
{
    public class FlowchartStringVarSavingTests : FlowchartVarSavingTests<PrimitiveVarSaver>
    {
        protected override string VariableHolderName => "StringFlowchart";
        protected override VarSaver.ContentType SaveContentAs => VarSaver.ContentType.regularString;

    }
}
