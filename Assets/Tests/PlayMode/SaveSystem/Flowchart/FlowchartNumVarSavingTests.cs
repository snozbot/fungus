using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartNumVarSavingTests : FlowchartVarSavingTests<PrimitiveVarSaver>
    {
        protected override string VariableHolderName => "NumericFlowchart";
        protected override VarSaver.ContentType SaveContentAs => VarSaver.ContentType.regularString;

    }

    
}
