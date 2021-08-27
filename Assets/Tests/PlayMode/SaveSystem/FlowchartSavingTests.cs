using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartSavingTests: SaveSysPlayModeTest
    {
        protected override string PathToScene => "Prefabs/FlowchartSavingTests";
        protected Flowchart[] allFlowcharts;

        public override void SetUp()
        {
            base.SetUp();
            allFlowcharts = GameObject.FindObjectsOfType<Flowchart>();
            numericFlowchartGO = GameObject.Find(numericFlowchartName);
            numericFlowchart = numericFlowchartGO.GetComponent<Flowchart>();

            flowchartEncodersGO = GameObject.Find(flowchartEncodersGOName);
            numberSaver = flowchartEncodersGO.GetComponent<NumericVarSaveEncoder>();

        }

        protected GameObject numericFlowchartGO;
        protected Flowchart numericFlowchart;
        protected string numericFlowchartName = "NumericFlowchart";

        protected GameObject flowchartEncodersGO;
        protected string flowchartEncodersGOName = "FlowchartEncoders";
        protected NumericVarSaveEncoder numberSaver;

        #region Saving the various variable types
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
            string[] expectedInOrder = { "10", "25", "55", "1.23", "5.6789", "123.4568" };
            string[] result = GetSavedValuesInOrder(savedVars);
            bool success = SameContentsInOrder(expectedInOrder, result);
            Assert.IsTrue(success);
        }

        protected virtual string[] GetSavedValuesInOrder(IList<StringPair> savedValues)
        {
            string[] result = new string[savedValues.Count];

            for (int i = 0; i < savedValues.Count; i++)
            {
                var pair = savedValues[i];
                result[i] = pair.val;
            }

            return result;
        }

        protected virtual bool SameContentsInOrder<T>(IList<T> firstList, IList<T> secondList)
        {
            bool differentSizes = firstList.Count != secondList.Count;

            if (differentSizes)
                return false;

            for (int i = 0; i < firstList.Count; i++)
            {
                var firstItem = firstList[i];
                var secondItem = secondList[i];

                if (!firstItem.Equals(secondItem))
                    return false;
            }

            return true;
        }

        [Test]
        public void EncodeNumericVars_PassingIList()
        {
            IList<Variable> varsToSave = numericFlowchart.Variables;
            IList<StringPair> savedVars = numberSaver.Encode(varsToSave);
           
            // We want to be sure that the values are what we expect
            string[] expectedInOrder = { "10", "25", "55", "1.23", "5.6789", "123.4568" };
            string[] result = GetSavedValuesInOrder(savedVars);
            bool success = SameContentsInOrder(expectedInOrder, result);
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

        protected virtual void PassNonNumVarListToNumberEncoder<T>(IList<T> noneOfTheseShouldWork) where T: Variable
        {
            numberSaver.Encode( (IList<Variable>) noneOfTheseShouldWork);
        }

        #endregion
    }
}
