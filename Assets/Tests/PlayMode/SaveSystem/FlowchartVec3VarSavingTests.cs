using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartVec3VarSavingTests : FlowchartVarSavingTests<Vec3VarSaver>
    {
        protected override string VariableHolderName => "Vec_3_Flowchart";

        protected override void PrepareInvalidInputs()
        {
            string vec2VarFlowchart = "Vec_2_Flowchart";
            IList<Variable> vec2Vars = GetVarsOfFlowchartNamed(vec2VarFlowchart);

            string stringFlowchartName = "StringFlowchart";
            IList<Variable> stringVars = GetVarsOfFlowchartNamed(stringFlowchartName);

            List<Variable> gatheredUp = new List<Variable>();
            gatheredUp.AddRange(vec2Vars);
            gatheredUp.AddRange(stringVars);

            invalidInputs = gatheredUp;
        }
    }
}
