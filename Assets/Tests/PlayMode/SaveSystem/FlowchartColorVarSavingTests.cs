using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartColorVarSavingTests : FlowchartSavingTests
    {
        public override void SetUp()
        {
            base.SetUp();
            colorFlowchartGO = GameObject.Find(stringFlowchartName);
            colorFlowchart = colorFlowchartGO.GetComponent<Flowchart>();

            flowchartEncodersGO = GameObject.Find(flowchartEncodersGOName);
            colorSaver = flowchartEncodersGO.GetComponent<ColorVarSaveEncoder>();
            PrepareExpectedResult();
        }

        protected GameObject colorFlowchartGO;
        protected Flowchart colorFlowchart;
        protected string stringFlowchartName = "ColorFlowchart";

        protected ColorVarSaveEncoder colorSaver;

        protected virtual void PrepareExpectedResult()
        {
            expected.Clear();
            var colorVars = colorFlowchart.Variables;

            foreach (var colorVarEl in colorVars)
            {
                var colorAsString = colorVarEl.GetValue().ToString();
                expected.Add(colorAsString);
            }
        }

        protected IList<string> expected = new List<string>();

        [Test]
        public void EncodeColorVars_PassingSingles()
        {
            IList<Variable> varsToSave = colorFlowchart.Variables;
            IList<StringPair> savedVars = new List<StringPair>();
  
            foreach (var varEl in varsToSave)
            {
                var encodingResult = colorSaver.Encode(varEl);
                savedVars.Add(encodingResult);
            }

            // We want to be sure that the values are what we expect
            IList<string> result = savedVars.GetValues();
            bool success = expected.HasSameContentsInOrderAs(result);
            Assert.IsTrue(success);
        }

        protected Color StringToColor(string stringifiedColor)
        {
            int openingParen = stringifiedColor.IndexOf('(');
            int closingParen = stringifiedColor.IndexOf(')');
            int amountOfChars = closingParen - (openingParen + 1);
            string colorsTogether = stringifiedColor.Substring(openingParen + 1, amountOfChars);
            IList<string> colorsSplit = colorsTogether.Split(',');

            Color result = new Color();
            float red = float.Parse(colorsSplit[0].Trim());
            float green = float.Parse(colorsSplit[1].Trim());
            float blue = float.Parse(colorsSplit[2].Trim());
            float alpha = float.Parse(colorsSplit[3].Trim());
            // ^Need the trims since the Split call leaves needless spaces

            result.r = red;
            result.g = green;
            result.b = blue;
            result.a = alpha;

            return result;
        }

        protected Color ColorIn255Scale(Color input)
        {
            input.r = (float) System.Math.Round(input.r * 255, 3);
            input.g = (float) System.Math.Round(input.g * 255, 3);
            input.b = (float) System.Math.Round(input.b * 255, 3);
            input.a = (float) System.Math.Round(input.a * 255, 3);

            return input;
        }

        [Test]
        public void EncodeColorVars_PassingIList()
        {
            IList<Variable> varsToSave = colorFlowchart.Variables;
            IList<StringPair> savedVars = colorSaver.Encode(varsToSave);
            
            // We want to be sure that the values are what we expect
            IList<string> result = savedVars.GetValues();
            bool success = expected.HasSameContentsInOrderAs(result);
            Assert.IsTrue(success);
        }
    }
}
