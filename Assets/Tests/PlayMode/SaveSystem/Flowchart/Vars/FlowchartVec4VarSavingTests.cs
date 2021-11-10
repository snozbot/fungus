using Fungus.LionManeSaveSys;

namespace SaveSystemTests
{
    public class FlowchartVec4VarSavingTests : FlowchartVarSavingTests<VectorVarSaver>
    {
        protected override string VariableHolderName => "Vec_4_Flowchart";
        protected override VarSaver.ContentType SaveContentAs
        {
            get { return VarSaver.ContentType.jsonString; }
        }

    }
}
