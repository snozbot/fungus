using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartStringVarSavingTests : FlowchartVarSavingTests
    {
        protected override string VariableHolderName => "StringFlowchart";

        protected override void PrepareInvalidInputs()
        {
            string numericFlowchartName = "NumericFlowchart";
            IList<Variable> numericVars = GetVarsOfFlowchartNamed(numericFlowchartName);

            string colorFlowchartName = "ColorFlowchart";
            IList<Variable> colorVars = GetVarsOfFlowchartNamed(colorFlowchartName);

            List<Variable> gatheredUp = new List<Variable>();
            gatheredUp.AddRange(numericVars);
            gatheredUp.AddRange(colorVars);

            invalidInputs = gatheredUp;
        }

        protected override void PrepareExpectedResults()
        {
            ExpectedResults.Clear();

            foreach (var varEl in variablesToEncode)
            {
                var varAsString = varEl.GetValue().ToString();
                ExpectedResults.Add(varAsString);
            }
        }
    }
}
