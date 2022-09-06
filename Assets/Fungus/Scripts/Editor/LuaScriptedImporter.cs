// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

using System.IO;

namespace Fungus.EditorUtils
{
#if UNITY_2018_4_OR_NEWER
    [UnityEditor.AssetImporters.ScriptedImporter(1, "lua")]
	public class LuaScriptedImporter : UnityEditor.AssetImporters.ScriptedImporter
	{
	    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
	    {
	        TextAsset lua = new TextAsset(File.ReadAllText(ctx.assetPath));
	        ctx.AddObjectToAsset("main", lua);
	        ctx.SetMainObject(lua);
	    }
	}
#endif
}