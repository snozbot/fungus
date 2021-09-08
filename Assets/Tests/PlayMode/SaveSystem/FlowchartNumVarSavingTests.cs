using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartNumVarSavingTests : FlowchartVarSavingTests<NumericVarSaveEncoder>
    {
        
        protected override string VariableHolderName => "NumericFlowchart";

        protected override void PrepareInvalidInputs()
        {
            string colorFlowchartName = "ColorFlowchart";
            IList<Variable> colorVars = GetVarsOfFlowchartNamed(colorFlowchartName);

            string stringFlowchartName = "StringFlowchart";
            IList<Variable> stringVars = GetVarsOfFlowchartNamed(stringFlowchartName);

            List<Variable> gatheredUp = new List<Variable>();
            gatheredUp.AddRange(colorVars);
            gatheredUp.AddRange(stringVars);

            invalidInputs = gatheredUp;
        }

        protected override void PrepareExpectedResults()
        {
            ExpectedResults.Clear();

            foreach (var varEl in variablesToEncode)
            {
                var varAsString = varEl.GetValue().ToString();
                // ^As opposed to turning it into a json. Converting a number to
                // a JSON only gets you an empty json object
                ExpectedResults.Add(varAsString);
            }
        }

    }

    
}
