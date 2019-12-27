using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/**
 * Load and run a prefab with lots of conditionals to ensure ifs, else, elifs, nesting, etc.
 * FungusLua tests for interaction with fungus systems
 * if this can prefabs, so all non narrative demo sceenes, convert them to tests where possible
 * - collection loop can become a prefab
 * - collection physics could but doesnt make sense
 * - mathquiz should stay but could use a facimily 
 * - 
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
        //    // Use the Assert class to test conditions
        //    var cubePre = Resources.Load<GameObject>("ErrorCube");
        //    Assert.NotNull(cubePre);
        //}

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FungusPlayModeTestWithEnumeratorPasses()
        {
            yield return EditorUtils.TestUtils.RunPrefabTest("LoopTest", -1.0f);
        }
    }
}
