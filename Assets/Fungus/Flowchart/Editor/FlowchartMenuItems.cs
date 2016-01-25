using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace Fungus
{

	public class FlowchartMenuItems
	{
		[MenuItem("Tools/Fungus/Create/Flowchart", false, 0)]
		static void CreateFlowchart()
		{
			GameObject go = SpawnPrefab("Flowchart");
			go.transform.position = Vector3.zero;

			// Only the first created Flowchart in the scene should have a default GameStarted block
			if (GameObject.FindObjectsOfType<Flowchart>().Length > 1)
			{
				Block block = go.GetComponent<Block>();
				block.eventHandler = null;
				GameObject.DestroyImmediate(block.eventHandler);
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
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab == null)
			{
				return null;
			}

			GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			PrefabUtility.DisconnectPrefabInstance(go);

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