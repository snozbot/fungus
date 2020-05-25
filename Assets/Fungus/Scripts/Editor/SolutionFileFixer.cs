using UnityEditor;

#if UNITY_2019_3_12
namespace Fungus.EditorUtils
{
    //fix for a bug introduced in unity 2019.3.12 that prevents cross asmdef references from showing
    //  doco and code hinting correctly in vs2019
    // https://forum.unity.com/threads/2019-3-12f1-build-errors.880312/
    public class SolutionFileFixer : AssetPostprocessor
    {
        private static string OnGeneratedCSProject(string path, string content)
        {
            return content.Replace("<ReferenceOutputAssembly>false</ReferenceOutputAssembly>", "<ReferenceOutputAssembly>true</ReferenceOutputAssembly>");
        }
    }
}
#endif