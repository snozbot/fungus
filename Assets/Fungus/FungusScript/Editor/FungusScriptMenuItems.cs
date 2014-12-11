using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace Fungus
{

	public class FungusScriptMenuItems
	{
		[MenuItem("GameObject/Fungus/Fungus Script")]
		static void CreateFungusScript()
		{
			GameObject newFungusScriptGO = new GameObject();
			newFungusScriptGO.name = "FungusScript";
			FungusScript fungusScript = newFungusScriptGO.AddComponent<FungusScript>();
			Sequence sequence = Undo.AddComponent<Sequence>(newFungusScriptGO);
			sequence.nodeRect.x += 60;
			sequence.nodeRect.y += 60;
			fungusScript.selectedSequence = sequence;
			fungusScript.scrollPos = Vector2.zero;
			Undo.RegisterCreatedObjectUndo(newFungusScriptGO, "Create Fungus Script");
		}

		public static GameObject SpawnPrefab(string prefabFile)
		{
			string prefabName = Path.GetFileNameWithoutExtension(prefabFile);

			// This will only succeed if Fungus is located in the root folder of the project
			GameObject prefab = Resources.LoadAssetAtPath(prefabFile, typeof(GameObject)) as GameObject;
			if (prefab == null)
			{
				return null;
			}

			GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			go.name = prefabName;
			
			Camera sceneCam = SceneView.currentDrawingSceneView.camera;

			Vector3 pos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
			pos.z = 0f;
			go.transform.position = pos;

			Selection.activeGameObject = go;
			
			Undo.RegisterCreatedObjectUndo(go, "Create Object");

			return go;
		}
	}

}