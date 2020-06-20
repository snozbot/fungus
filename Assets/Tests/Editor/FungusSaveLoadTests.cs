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
            //create flowchart
            var go = new GameObject();
            var f = go.AddComponent<Flowchart>();
            var b = f.CreateBlock(new Vector2());
            var vi = f.gameObject.AddComponent<IntegerVariable>();
            vi.Key = f.GetUniqueVariableKey("");
            f.Variables.Add(vi);


            yield return b.Execute(0, null);
            vi.Value = 8;

            var fd = FlowchartData.Encode(f);

            //change data
            yield return b.Execute(0, null);
            yield return b.Execute(0, null);
            vi.Value = 5;

            //decode to revert
            fd.Decode(f);

            Assert.That(b.GetExecutionCount() == 1);
            Assert.That(vi.Value == 8);

            Object.DestroyImmediate(go);
        }
    }
}