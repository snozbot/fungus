// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEditor;

namespace Fungus.EditorUtils
{
    public class FlowchartMenuItems
    {
        [MenuItem("Tools/Fungus/Create/Flowchart", false, 0)]
        static void CreateFlowchart()
        {
            GameObject go = SpawnPrefab("Flowchart");
            go.transform.position = Vector3.zero;

            // This is the latest version of Flowchart, so no need to update.
            var flowchart = go.GetComponent<Flowchart>();
            if (flowchart != null)
            {
                flowchart.Version = FungusConstants.CurrentVersion;
            }

            // Only the first created Flowchart in the scene should have a default GameStarted block
            if (GameObject.FindObjectsOfType<Flowchart>().Length > 1)
            {
                var block = go.GetComponent<Block>();
                GameObject.DestroyImmediate(block._EventHandler);
                block._EventHandler = null;
            }
        }

        [MenuItem("Tools/Fungus/Create/Fungus Logo", false, 1000)]
        static void CreateFungusLogo()
        {
            SpawnPrefab("FungusLogo");
        }

        [MenuItem("Tools/Fungus/Utilities/Export Fungus Package")]
        static void ExportFungusPackage()
        {
            string path = EditorUtility.SaveFilePanel("Export Fungus Package", "", "Fungus", "unitypackage");           
            if(path.Length == 0) 
            {
                return;
            }

            string[] folders = new string[] {"Assets/Fungus", "Assets/FungusExamples" };

            AssetDatabase.ExportPackage(folders, path, ExportPackageOptions.Recurse);
        }
            
        public static GameObject SpawnPrefab(string prefabName)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
            if (prefab == null)
            {
                return null;
            }

            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = prefab.name;

            SceneView view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                Camera sceneCam = view.camera;
                Vector3 pos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
                pos.z = 0f;
                go.transform.position = pos;
            }

            Selection.activeGameObject = go;
            
            Undo.RegisterCreatedObjectUndo(go, "Create Object");

            return go;
        }
    }
}