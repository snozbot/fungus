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


        [Test]
        public void EncodeStringVars_PassingSingles()
        {
            IList<StringPair> savedVars = new List<StringPair>();

            foreach (var varEl in variablesToEncode)
            {
                var encodingResult = varSaver.Encode(varEl);
                savedVars.Add(encodingResult);
            }

            // We want to be sure that the values are what we expect
            IList<string> result = savedVars.GetValues();
            bool success = ExpectedResults.HasSameContentsInOrderAs(result);
            Assert.IsTrue(success);
        }

        [Test]
        public void EncodeNumericVars_PassingIList()
        {
            IList<StringPair> savedVars = varSaver.Encode(variablesToEncode);

            // We want to be sure that the values are what we expect
            IList<string> result = savedVars.GetValues();
            bool success = ExpectedResults.HasSameContentsInOrderAs(result);
            Assert.IsTrue(success);
        }

    }
}
