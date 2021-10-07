using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Fungus;

namespace SaveSystemTests
{
    public abstract class FlowchartVarSavingTests<TVarSaver>: SaveSysPlayModeTest where TVarSaver: VarSaver
    {
        protected override string PathToScene => "Prefabs/FlowchartSavingTests";

        #region Prep work

        public override void SetUp()
        {
            base.SetUp();
            allFlowcharts = GameObject.FindObjectsOfType<Flowchart>();
            variablesToEncode = GetVarsOfFlowchartNamed(VariableHolderName);
            GetSaverNeeded();
            PrepareExpectedResults();
            PrepareInvalidInputs();
        }

        protected Flowchart[] allFlowcharts;
        protected IList<Variable> variablesToEncode;

        protected virtual IList<Variable> GetVarsOfFlowchartNamed(string name)
        {
            GameObject flowchartGO = GameObject.Find(name);
            Flowchart flowchart = flowchartGO.GetComponent<Flowchart>();
            return flowchart.Variables;
        }

        protected abstract string VariableHolderName { get; }

        protected virtual void GetSaverNeeded()
        {
            GetEncoderHolder();
            varSaver = hasEncoders.GetComponent<TVarSaver>();
        }

        protected virtual void GetEncoderHolder()
        {
            hasEncoders = GameObject.Find(encoderContainerGOName);
        }

        protected GameObject hasEncoders;
        protected string encoderContainerGOName = "FlowchartEncoders";
        protected TVarSaver varSaver;

        protected virtual void PrepareExpectedResults()
        {
            ExpectedResults.Clear();

            foreach (var varEl in variablesToEncode)
            {
                var varAsString = JsonUtility.ToJson(varEl.GetValue());
                ExpectedResults.Add(varAsString);
            }
        }

        protected IList<string> ExpectedResults { get; } = new List<string>();

        protected abstract void PrepareInvalidInputs();

        protected IList<Variable> invalidInputs;

        #endregion

        #region Tests

        [Test]
        public virtual void EncodeVars_PassingSingles()
        {
            encodingResults = VarsEncodedWithMultipleEncodeCalls();

            AssertEncodingSuccess();
        }

        protected IList<StringPair> encodingResults;

        protected virtual IList<StringPair> VarsEncodedWithMultipleEncodeCalls()
        {
            IList<StringPair> savedVars = new List<StringPair>();

            foreach (var varEl in variablesToEncode)
            {
                var result = varSaver.Encode(varEl);
                savedVars.Add(result);
            }

            return savedVars;
        }

        protected virtual void AssertEncodingSuccess()
        {
            IList<string> whatWeGot = encodingResults.GetValues();
            bool success = ExpectedResults.HasSameContentsInOrderAs(whatWeGot);
            Assert.IsTrue(success);
        }

        [Test]
        public virtual void EncodeVars_PassingIList()
        {
            encodingResults = VarsEncodedWithOneEncodeCall();

            AssertEncodingSuccess();
        }

        protected virtual IList<StringPair> VarsEncodedWithOneEncodeCall()
        {
            // One that involves passing multiple vars at once to the encoder
            return varSaver.Encode(variablesToEncode);
        }

        [Test]
        public virtual void RejectSingleInvalidInputs()
        {
            foreach (var invalid in invalidInputs)
            {
                Assert.Throws<System.InvalidOperationException>(() => varSaver.Encode(invalid));
            }
        }

        [Test]
        public virtual void RejectMultipleInvalidInputsAtOnce()
        {
            Assert.Throws<System.InvalidOperationException>(() => varSaver.Encode(invalidInputs));
            
        }
        #endregion

    }
}
