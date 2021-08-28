using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartStringVarSavingTests : FlowchartSavingTests
    {
        public override void SetUp()
        {
            base.SetUp();
            stringFlowchartGO = GameObject.Find(stringFlowchartName);
            stringFlowchart = stringFlowchartGO.GetComponent<Flowchart>();

            flowchartEncodersGO = GameObject.Find(flowchartEncodersGOName);
            stringSaver = flowchartEncodersGO.GetComponent<StringVarSaveEncoder>();
        }

        protected GameObject stringFlowchartGO;
        protected Flowchart stringFlowchart;
        protected string stringFlowchartName = "StringFlowchart";

        protected StringVarSaveEncoder stringSaver;

        [Test]
        public void EncodeStringVars_PassingSingles()
        {
            IList<Variable> varsToSave = stringFlowchart.Variables;
            IList<StringPair> savedVars = new List<StringPair>();

            foreach (var varEl in varsToSave)
            {
                var encodingResult = stringSaver.Encode(varEl);
                savedVars.Add(encodingResult);
            }

            // We want to be sure that the values are what we expect
            IList<string> expectedInOrder = new string[] { "Hi", "I'm", "Paul"};
            IList<string> result = savedVars.GetValues();
            bool success = expectedInOrder.HasSameContentsInOrderAs(result);
            Assert.IsTrue(success);
        }

        [Test]
        public void EncodeNumericVars_PassingIList()
        {
            IList<Variable> varsToSave = stringFlowchart.Variables;
            IList<StringPair> savedVars = stringSaver.Encode(varsToSave);

            // We want to be sure that the values are what we expect
            IList<string> expectedInOrder = new string[] { "Hi", "I'm", "Paul" };
            IList<string> result = savedVars.GetValues();
            bool success = expectedInOrder.HasSameContentsInOrderAs(result);
            Assert.IsTrue(success);
        }

    }
}
