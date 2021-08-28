using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Fungus;

namespace SaveSystemTests
{
    public abstract class FlowchartSavingTests<TVarSaver>: SaveSysPlayModeTest where TVarSaver: VarSaveEncoder
    {
        protected override string PathToScene => "Prefabs/FlowchartSavingTests";
        
        public override void SetUp()
        {
            base.SetUp();
            allFlowcharts = GameObject.FindObjectsOfType<Flowchart>();
            GetVariableHolder();
            GetSaverNeeded();
            PrepareExpectedResults();
        }

        protected Flowchart[] allFlowcharts;

        protected virtual void GetVariableHolder()
        {
            variableHolderGO = GameObject.Find(VariableHolderName);
            variableHolder = variableHolderGO.GetComponent<Flowchart>();
            variablesToEncode = variableHolder.Variables;
        }

        protected GameObject variableHolderGO;
        protected abstract string VariableHolderName { get; }
        protected Flowchart variableHolder;
        protected IList<Variable> variablesToEncode;
        

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
            var colorVars = variableHolder.Variables;

            foreach (var colorVarEl in colorVars)
            {
                var colorAsString = colorVarEl.GetValue().ToString();
                ExpectedResults.Add(colorAsString);
            }
        }

        protected IList<string> ExpectedResults { get; } = new List<string>();

        [Test]
        public void EncodeVars_PassingSingles()
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
        public void EncodeColorVars_PassingIList()
        {
            encodingResults = VarsEncodedWithOneEncodeCall();

            AssertEncodingSuccess();
        }

        protected virtual IList<StringPair> VarsEncodedWithOneEncodeCall()
        {
            // One that involves passing multiple vars at once to the encoder
            return varSaver.Encode(variablesToEncode);
        }

    }
}
