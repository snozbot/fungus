using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/**
 * Set and compare variable types
 */


namespace Fungus.Tests
{
    [TestFixture]
    public class FungusPlayModeTest
    {
        //// A Test behaves as an ordinary method
        //[Test]
        //public void FungusPlayModeTestSimplePasses()
        //{
        //}

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Looping()
        {
            yield return EditorUtils.TestUtils.RunPrefabFlowchartTests("LoopTest", true);
        }

        [UnityTest]
        public IEnumerator ControlFlow()
        {
            yield return EditorUtils.TestUtils.RunPrefabFlowchartTests("FlowTest", true);
        }

        [UnityTest]
        public IEnumerator VariableSets()
        {
            yield return EditorUtils.TestUtils.RunPrefabFlowchartTests("VarSetTest", true);
        }
    }
}
