using Fungus.LionManeSaveSys;

namespace SaveSystemTests
{
    public class FlowchartVec2VarSavingTests : FlowchartVarSavingTests<VectorVarSaver>
    {
        protected override string VariableHolderName => "Vec_2_Flowchart";
        protected override VarSaver.ContentType SaveContentAs
        {
            get { return VarSaver.ContentType.jsonString; }
        }

    }
}
