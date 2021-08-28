using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartStringVarSavingTests : FlowchartSavingTests<StringVarSaveEncoder>
    {
        protected override string VariableHolderName => "StringFlowchart";

    }
}
