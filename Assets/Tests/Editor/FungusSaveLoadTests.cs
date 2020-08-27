// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fungus.Tests
{
    [TestFixture]
    public class FungusSaveLoadTests
    {
        //by no means exaustive but provides us a basic test in editor mode that flowchart
        // serialisation is at all functional
        [UnityTest]
        public IEnumerator ManualSaveLoadFlowchart()
        {
            const int InitialVariableValue = 8;
            const int ChangedVariableValue = 8;

            //create flowchart
            var go = new GameObject();
            var f = go.AddComponent<Flowchart>();
            var b = f.CreateBlock(new Vector2());
            var vi = f.gameObject.AddComponent<IntegerVariable>();
            vi.Key = f.GetUniqueVariableKey("");
            f.Variables.Add(vi);

            yield return b.Execute(0, null);
            vi.Value = InitialVariableValue;

            var fd = FlowchartData.Encode(f);

            //change data
            yield return b.Execute(0, null);
            yield return b.Execute(0, null);
            vi.Value = ChangedVariableValue;

            //decode to revert
            fd.Decode(f);

            Assert.That(b.GetExecutionCount() == 1);
            Assert.That(vi.Value == InitialVariableValue);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void ManualSaveLoadValueCollection()
        {
            throw new System.NotImplementedException();
            //ValueTypeCollectionData
        }

        [Test]
        public void ManualSaveLoadGlobalVars()
        {
            throw new System.NotImplementedException();
            //GlobalVariableData
        }
    }
}