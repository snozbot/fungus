using Fungus;

namespace SaveSystemTests
{
    public class FlowchartVec3VarSavingTests : FlowchartVarSavingTests<VectorVarSaver>
    {
        protected override string VariableHolderName => "Vec_3_Flowchart";
        protected override VarSaver.ContentType SaveContentAs => VarSaver.ContentType.jsonString;

    }
}
