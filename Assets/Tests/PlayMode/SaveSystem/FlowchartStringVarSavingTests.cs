using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartStringVarSavingTests : FlowchartVarSavingTests<PrimitiveVarSaver>
    {
        protected override string VariableHolderName => "StringFlowchart";
        protected override VarSaver.ContentType SaveContentAs => VarSaver.ContentType.regularString;

    }
}
