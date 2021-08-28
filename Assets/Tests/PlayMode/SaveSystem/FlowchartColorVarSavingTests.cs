using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartColorVarSavingTests : FlowchartVarSavingTests<ColorVarSaveEncoder>
    {
        protected override string VariableHolderName => "ColorFlowchart";

        protected override void PrepareInvalidInputs()
        {
            string numericFlowchartName = "NumericFlowchart";
            IList<Variable> numericVars = GetVarsOfFlowchartNamed(numericFlowchartName);

            string stringFlowchartName = "StringFlowchart";
            IList<Variable> stringVars = GetVarsOfFlowchartNamed(stringFlowchartName);

            List<Variable> gatheredUp = new List<Variable>();
            gatheredUp.AddRange(numericVars);
            gatheredUp.AddRange(stringVars);

            invalidInputs = gatheredUp;
        }

    }
}
