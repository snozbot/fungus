// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    internal static partial class FungusEditorResources
    {
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        
        static FungusEditorResources()
        {
            LoadTexturesFromNames();
        }

        private static void LoadTexturesFromNames()
        {
            var baseDirectories = AssetDatabase.FindAssets("\"Fungus Editor Resources\"").Select(
                guid => AssetDatabase.GUIDToAssetPath(guid)
            ).ToArray();
            
            foreach (var name in resourceNames)
            {
                LoadTexturesFromGUIDs(AssetDatabase.FindAssets(name + " t:Texture2D", baseDirectories));
            }
        }

        private static void LoadAllTexturesInFolder()
        {
            var rootGuid = AssetDatabase.FindAssets("\"Fungus Editor Resources\"")[0];
            var root = AssetDatabase.GUIDToAssetPath(rootGuid);
            LoadTexturesFromGUIDs(AssetDatabase.FindAssets("t:Texture2D", new [] { root }));
        }

        private static void LoadTexturesFromGUIDs(string[] guids)
        {
            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).OrderBy(path => path.ToLower().Contains("/pro/"));
            
            foreach (var path in paths)
            {
                if (path.ToLower().Contains("/pro/") && !EditorGUIUtility.isProSkin)
                {
                    return;
                }
                var texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                textures[texture.name] = texture;
            }
        }

        [MenuItem("Tools/Fungus/Utilities/Update Editor Resources Script")]
        private static void GenerateResourcesScript()
        {
            textures.Clear();
            LoadAllTexturesInFolder();

            var guid = AssetDatabase.FindAssets("FungusEditorResources t:MonoScript")[0];
            var relativePath = AssetDatabase.GUIDToAssetPath(guid).Replace("FungusEditorResources.cs", "FungusEditorResourcesGenerated.cs");
            var absolutePath = Application.dataPath + relativePath.Substring("Assets".Length);
            
            using (var writer = new StreamWriter(absolutePath))
            {
                writer.WriteLine("// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).");
                writer.WriteLine("// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)");
                writer.WriteLine("");				
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("");
                writer.WriteLine("namespace Fungus.EditorUtils");
                writer.WriteLine("{");
                writer.WriteLine("    internal static partial class FungusEditorResources");
                writer.WriteLine("    {");
                writer.WriteLine("        private static readonly string[] resourceNames = new [] {");
                
                foreach (var pair in textures)
                {
                    writer.WriteLine("            \"" + pair.Key + "\",");
                }

                writer.WriteLine("        };");
                writer.WriteLine("");

                foreach (var pair in textures)
                {
                    var name = pair.Key;
                    var pascalCase = string.Join("", name.Split(new [] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(
                        s => s.Substring(0, 1).ToUpper() + s.Substring(1)).ToArray()
                    );
                    writer.WriteLine("        public static Texture2D " + pascalCase + " { get { return GetTexture(\"" + name + "\"); } }");
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.ImportAsset(relativePath);
        }

        private static Texture2D GetTexture(string name)
        {
            Texture2D texture;
            if (!textures.TryGetValue(name, out texture))
            {
                Debug.LogWarning("Texture \"" + name + "\" not found!");
            }
            
            return texture;
        }
    }
}
