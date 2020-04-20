// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine.Assertions;

namespace Fungus.EditorUtils
{
    static public class TestUtils
    {
        /// <summary>
        /// Loads a prefab from the resources folder, optionally waiting a preset amount of time for it to complete or until says it is done.
        /// </summary>
        /// <param name="prefabTestName">Name of prefab in the editor resources folder to tunr, expects it to have a flowchart on in
        /// that will try to execute at frame 0</param>
        /// <param name="runBlocksManually">if true will run blocks in sequence until all are complete</param>
        /// <param name="maxFramesToComplete">max frames for the test to be allowed to run, will assert if it runs beyond this limit</param>
        /// <returns></returns>
        static public System.Collections.IEnumerator RunPrefabFlowchartTests(string prefabTestName, bool runBlocksManually = true, int maxFramesToComplete = 1000)
        {
            var resPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>(prefabTestName);
            Assert.IsNotNull(resPrefab);
            var resTest = UnityEngine.Object.Instantiate(resPrefab);

            //give it a few frames to kick in
            yield return null;
            yield return null;
            yield return null;

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            var f = resTest.GetComponent<Flowchart>();
            Assert.IsNotNull(f);

            int frame = 3;

            //ensure there isn't something already doing a job
            while (f.HasExecutingBlocks())
            {
                frame++;
                Assert.IsTrue(frame < maxFramesToComplete);
                yield return null;
            }

            if (runBlocksManually)
            {
                var blocks = f.GetComponents<Block>();
                foreach (var block in blocks)
                {
                    block.StartExecution();
                    while (f.HasExecutingBlocks())
                    {
                        frame++;
                        Assert.IsTrue(frame < maxFramesToComplete);
                        yield return null;
                    }
                }
            }

            UnityEngine.Object.DestroyImmediate(resTest);
        }
    }
}