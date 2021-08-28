using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartNumVarSavingTests : FlowchartSavingTests
    {
        public override void SetUp()
        {
            base.SetUp();
            numericFlowchartGO = GameObject.Find(numericFlowchartName);
            numericFlowchart = numericFlowchartGO.GetComponent<Flowchart>();

            flowchartEncodersGO = GameObject.Find(flowchartEncodersGOName);
            numberSaver = flowchartEncodersGO.GetComponent<NumericVarSaveEncoder>();
        }

        protected GameObject numericFlowchartGO;
        protected Flowchart numericFlowchart;
        protected string numericFlowchartName = "NumericFlowchart";

        protected NumericVarSaveEncoder numberSaver;

        [Test]
        public void EncodeNumericVars_PassingSingles()
        {
            IList<Variable> varsToSave = numericFlowchart.Variables;
            IList<StringPair> savedVars = new List<StringPair>();

            foreach (var varEl in varsToSave)
            {
                var encodingResult = numberSaver.Encode(varEl);
                savedVars.Add(encodingResult);
            }

            // We want to be sure that the values are what we expect
            IList<string> expectedInOrder = new string[] { "10", "25", "55", "1.23", "5.6789", "123.4568" };
            IList<string> result = savedVars.GetValues();
            bool success = expectedInOrder.HasSameContentsInOrderAs(result);
            Assert.IsTrue(success);
        }

        [Test]
        public void EncodeNumericVars_PassingIList()
        {
            IList<Variable> varsToSave = numericFlowchart.Variables;
            IList<StringPair> savedVars = numberSaver.Encode(varsToSave);

            // We want to be sure that the values are what we expect
            IList<string> expectedInOrder = new string[] { "10", "25", "55", "1.23", "5.6789", "123.4568" };
            IList<string> result = savedVars.GetValues();
            bool success = expectedInOrder.HasSameContentsInOrderAs(result);
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
            numberSaver.Encode(shouldNotWork);
        }

        [Test]
        public void NumVarEncoderRejectsInvalidInput_PassingIList()
        {
            IList<StringVariable> stringVars = GameObject.FindObjectsOfType<StringVariable>();

            Assert.Throws<System.InvalidOperationException>(() => { PassNonNumVarListToNumberEncoder(stringVars); });

        }

        protected virtual void PassNonNumVarListToNumberEncoder<T>(IList<T> noneOfTheseShouldWork) where T : Variable
        {
            numberSaver.Encode((IList<Variable>)noneOfTheseShouldWork);
        }

    }

    
}
