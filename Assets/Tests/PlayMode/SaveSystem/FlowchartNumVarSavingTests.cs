using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartNumVarSavingTests : FlowchartSavingTests<NumericVarSaveEncoder>
    {
        
        protected override string VariableHolderName => "NumericFlowchart";

        [Test]
        public void EncodeNumericVars_PassingSingles()
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

        [Test]
        public void NumVarEncoderRejectsInvalidInput_PassingSingles()
        {
            IList<StringVariable> stringVars = GameObject.FindObjectsOfType<StringVariable>();

            for (int i = 0; i < stringVars.Count; i++)
            {
                var currentVar = stringVars[i];
                Assert.Throws<System.InvalidOperationException>(() => { PassNonNumVarToNumberEncoder(currentVar); });
            }
        }

        protected virtual void PassNonNumVarToNumberEncoder(Variable shouldNotWork)
        {
            varSaver.Encode(shouldNotWork);
        }

        [Test]
        public void NumVarEncoderRejectsInvalidInput_PassingIList()
        {
            IList<StringVariable> stringVars = GameObject.FindObjectsOfType<StringVariable>();

            Assert.Throws<System.InvalidOperationException>(() => { PassNonNumVarListToNumberEncoder(stringVars); });

        }

        protected virtual void PassNonNumVarListToNumberEncoder<T>(IList<T> noneOfTheseShouldWork) where T : Variable
        {
            varSaver.Encode((IList<Variable>)noneOfTheseShouldWork);
        }

    }

    
}
