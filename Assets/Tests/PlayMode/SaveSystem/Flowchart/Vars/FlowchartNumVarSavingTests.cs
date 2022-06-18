using Fungus.LionManeSaveSys;

namespace SaveSystemTests
{
    public class FlowchartNumVarSavingTests : FlowchartVarSavingTests<PrimitiveVarSaver>
    {
        protected override string VariableHolderName => "NumericFlowchart";
        protected override VarSaver.ContentType SaveContentAs => VarSaver.ContentType.regularString;

    }

}
