// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
#if UNITY_5_0 || UNITY_5_1
using System.Reflection;
#endif

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(FungusEditorResources))]
    internal class FungusEditorResourcesInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (serializedObject.FindProperty("updateOnReloadScripts").boolValue)
            {
                GUILayout.Label("Updating...");
            }
            else
            {
                if (GUILayout.Button("Sync with EditorResources folder"))
                {
                    FungusEditorResources.GenerateResourcesScript();
                }

                DrawDefaultInspector();
            }
        }
    }

    // Handle reimporting all assets
    internal class EditorResourcesPostProcessor : AssetPostprocessor 
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] _, string[] __, string[] ___) 
        {
            foreach (var path in importedAssets)
            {
                if (path.EndsWith("FungusEditorResources.asset"))
                {
                    var asset = AssetDatabase.LoadAssetAtPath(path, typeof(FungusEditorResources)) as FungusEditorResources;
                    if (asset != null)
                    {
                        FungusEditorResources.UpdateTextureReferences(asset);
                        AssetDatabase.SaveAssets();
                        return;
                    }
                }
            }
        }
    }

    internal partial class FungusEditorResources : ScriptableObject
    {
        [Serializable]
        internal class EditorTexture
        {
            [SerializeField] private Texture2D free;
            [SerializeField] private Texture2D pro;

            public Texture2D Texture2D
            {
                get { return EditorGUIUtility.isProSkin && pro != null ? pro : free; }
            }

            public EditorTexture(Texture2D free, Texture2D pro)
            {
                this.free = free;
                this.pro = pro;
            }
        }

        private static FungusEditorResources instance;
        private static readonly string editorResourcesFolderName = "\"EditorResources\"";
        private static readonly string PartialEditorResourcesPath = System.IO.Path.Combine("Fungus", "EditorResources");
        [SerializeField] [HideInInspector] private bool updateOnReloadScripts = false;

        internal static FungusEditorResources Instance
        {
            get
            {
                if (instance == null)
                {
                    var guids = AssetDatabase.FindAssets("FungusEditorResources t:FungusEditorResources");

                    if (guids.Length == 0)
                    {
                        instance = ScriptableObject.CreateInstance(typeof(FungusEditorResources)) as FungusEditorResources;
                        AssetDatabase.CreateAsset(instance, GetRootFolder() + "/FungusEditorResources.asset");
                    }
                    else 
                    {
                        if (guids.Length > 1)
                        {
                            Debug.LogError("Multiple FungusEditorResources assets found!");
                        }

                        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        instance = AssetDatabase.LoadAssetAtPath(path, typeof(FungusEditorResources)) as FungusEditorResources;
                    }
                }

                return instance;
            }
        }

        private static string GetRootFolder()
        {
            var res = AssetDatabase.FindAssets(editorResourcesFolderName);

            foreach (var item in res)
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                var safePath = System.IO.Path.GetFullPath(path);
                if (safePath.IndexOf(PartialEditorResourcesPath) != -1)
                    return path;
            }

            return string.Empty;
        }

        internal static void GenerateResourcesScript()
        {
            // Get all unique filenames
            var textureNames = new HashSet<string>();
            var guids = AssetDatabase.FindAssets("t:Texture2D", new [] { GetRootFolder() });
            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            
            foreach (var path in paths)
            {
                textureNames.Add(Path.GetFileNameWithoutExtension(path));
            }

            var scriptGuid = AssetDatabase.FindAssets("FungusEditorResources t:MonoScript")[0];
            var relativePath = AssetDatabase.GUIDToAssetPath(scriptGuid).Replace("FungusEditorResources.cs", "FungusEditorResourcesGenerated.cs");
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
                writer.WriteLine("    internal partial class FungusEditorResources : ScriptableObject");
                writer.WriteLine("    {");
                
                foreach (var name in textureNames)
                {
                    writer.WriteLine("        [SerializeField] private EditorTexture " + name + ";");
                }

                writer.WriteLine("");

                foreach (var name in textureNames)
                {
                    var pascalCase = string.Join("", name.Split(new [] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(
                        s => s.Substring(0, 1).ToUpper() + s.Substring(1)).ToArray()
                    );
                    writer.WriteLine("        public static Texture2D " + pascalCase + " { get { return Instance." + name + ".Texture2D; } }");
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Instance.updateOnReloadScripts = true;
            AssetDatabase.ImportAsset(relativePath);
        }

        [DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            if (Instance.updateOnReloadScripts)
            {                
                UpdateTextureReferences(Instance);
            }
        }

        internal static void UpdateTextureReferences(FungusEditorResources instance)
        {
            // Iterate through all fields in instance and set texture references
            var serializedObject = new SerializedObject(instance);
            var prop = serializedObject.GetIterator();
            var rootFolder = new [] { GetRootFolder() };

            prop.NextVisible(true);
            while (prop.NextVisible(false))
            {
                if (prop.propertyType == SerializedPropertyType.Generic)
                {
                    var guids = AssetDatabase.FindAssets(prop.name + " t:Texture2D", rootFolder);
                    var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).Where(
                        path => path.Contains(prop.name + ".")
                    );

                    foreach (var path in paths)
                    {
                        var texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                        if (path.ToLower().Contains("/pro/"))
                        {
                            prop.FindPropertyRelative("pro").objectReferenceValue = texture;
                        }
                        else
                        {
                            prop.FindPropertyRelative("free").objectReferenceValue = texture;
                        }
                    }       
                }
            }

            serializedObject.FindProperty("updateOnReloadScripts").boolValue = false;

            // The ApplyModifiedPropertiesWithoutUndo() function wasn't documented until Unity 5.2
#if UNITY_5_0 || UNITY_5_1
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var applyMethod = typeof(SerializedObject).GetMethod("ApplyModifiedPropertiesWithoutUndo", flags);
            applyMethod.Invoke(serializedObject, null);
#else
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
#endif
        }
    }
}
