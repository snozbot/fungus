/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace Fungus
{

    public class MenuItems 
    {
        [MenuItem("Tools/Fungus/Create/Lua", false, 2000)]
        static void CreateLua()
        {
            SpawnPrefab("Prefabs/Lua", false);
        }

        [MenuItem("Tools/Fungus/Create/Lua File", false, 2001)]
        static void CreateLuaFile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Lua File", "script.txt", "txt", "Please select a file name for the new Lua script. Note: Lua files in Unity use the .txt extension.");
            if(path.Length == 0) 
            {
                return;
            }

            File.WriteAllText(path, "");
            AssetDatabase.Refresh();

#if UNITY_5_3_OR_NEWER
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
#else
            Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
#endif            
            if (asset != null)
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(asset);
            }            
        }

        [MenuItem("Tools/Fungus/Create/Lua Environment", false, 2100)]
        static void CreateLuaEnvironment()
        {
            SpawnPrefab("Prefabs/LuaEnvironment", false);
        }

        [MenuItem("Tools/Fungus/Create/Lua Bindings", false, 2101)]
        static void CreateLuaBindings()
        {
            SpawnPrefab("Prefabs/LuaBindings", false);
        }

        [MenuItem("Tools/Fungus/Create/Lua Script", false, 2102)]
        static void CreateLuaScript()
        {
            SpawnPrefab("Prefabs/LuaScript", false);
        }

        [MenuItem("Tools/Fungus/Create/Lua Store", false, 2103)]
        static void CreateLuaStore()
        {
            SpawnPrefab("Prefabs/LuaStore", false);
        }

        /// <summary>
        /// Spawns a prefab in the scene based on it's filename in a Resources folder in the project.
        /// If centerInScene is true then the object will be placed centered in the view window with z = 0.
        /// If centerInScene is false the the object will be placed at (0,0,0)
        /// </summary>
        public static GameObject SpawnPrefab(string prefabName, bool centerInScene)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabName);
            if (prefab == null)
            {
                return null;
            }

            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            PrefabUtility.DisconnectPrefabInstance(go);

            if (centerInScene)
            {
                SceneView view = SceneView.lastActiveSceneView;
                if (view != null)
                {
                    Camera sceneCam = view.camera;
                    Vector3 pos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
                    pos.z = 0f;
                    go.transform.position = pos;
                }
            }
            else
            {
                go.transform.position = Vector3.zero;
            }

            Selection.activeGameObject = go;

            Undo.RegisterCreatedObjectUndo(go, "Create Object");

            return go;
        }

        /// <summary>
        /// Create new asset from <see cref="ScriptableObject"/> type with unique name at
        /// selected folder in project window. Asset creation can be cancelled by pressing
        /// escape key when asset is initially being named.
        /// </summary>
        /// <typeparam name="T">Type of scriptable object.</typeparam>
        public static void CreateAsset<T>() where T : ScriptableObject {
            var asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, typeof(T).Name + ".asset");
        }

        /// <summary>
        /// Create an info text object which displays help info in the top left of the sceen.
        /// </summary>
        [MenuItem("Tools/Fungus/Utilities/Info Text")]
        static void SpawnInfoText()
        {
            SpawnPrefab("Prefabs/InfoText", false);
        }
    }

}
