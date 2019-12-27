namespace Fungus.EditorUtils
{
    static public class TestUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabTestName">Name of prefab in the editor resources folder to tunr, expects it to have a flowchart on in
        /// that will try to execute at frame 0</param>
        /// <param name="timeToComplete">Time to allow the flowchart to run, if negative will wait until flowchart completes</param>
        /// <returns></returns>
        static public System.Collections.IEnumerator RunPrefabTest(string prefabTestName, float timeToComplete)
        {
            var resPrefab = UnityEngine.Resources.Load<UnityEngine.GameObject>(prefabTestName);
            NUnit.Framework.Assert.NotNull(resPrefab);
            var resTest = UnityEngine.Object.Instantiate(resPrefab);

            //give it a few frames to kickin
            yield return null;
            yield return null;
            yield return null;

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            if (timeToComplete >= 0)
            {
                yield return new UnityEngine.WaitForSeconds(timeToComplete);
            }
            else
            {
                var f = resTest.GetComponent<Flowchart>();
                while (f.HasExecutingBlocks())
                {
                    yield return null;
                }
            }
            UnityEngine.Object.Destroy(resTest);
        }
    }
}