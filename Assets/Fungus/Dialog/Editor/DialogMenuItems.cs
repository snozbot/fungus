using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	// The prefab names are prefixed with Fungus to avoid clashes with any other prefabs in the project
	public class DialogMenuItems 
	{

		[MenuItem("GameObject/Fungus/Dialog/Character")]
		static void CreateCharacter()
		{
			GameObject go = Resources.Load<GameObject>("FungusCharacter");
			if (go != null)
			{
				GameObject spawnedGO = PrefabUtility.InstantiatePrefab(go) as GameObject;
				spawnedGO.name = "Character";
			}
		}

		[MenuItem("GameObject/Fungus/Dialog/SayDialog")]
		static void CreateSayDialog()
		{
			GameObject go = Resources.Load<GameObject>("FungusSayDialog");
			if (go != null)
			{
				GameObject spawnedGO = PrefabUtility.InstantiatePrefab(go) as GameObject;
				spawnedGO.name = "SayDialog";
			}
		}

		[MenuItem("GameObject/Fungus/Dialog/MenuDialog")]
		static void CreateMenuDialog()
		{
			GameObject go = Resources.Load<GameObject>("FungusMenuDialog");
			if (go != null)
			{
				GameObject spawnedGO = PrefabUtility.InstantiatePrefab(go) as GameObject;
				spawnedGO.name = "MenuDialog";
			}
		}

		[MenuItem("GameObject/Fungus/Dialog/Tag")]
		static void CreateTag()
		{
			GameObject go = Resources.Load<GameObject>("FungusTag");
			if (go != null)
			{
				GameObject spawnedGO = PrefabUtility.InstantiatePrefab(go) as GameObject;
				spawnedGO.name = "Tag";
			}
		}

		[MenuItem("GameObject/Fungus/Dialog/AudioTag")]
		static void CreateAudioTag()
		{
			GameObject go = Resources.Load<GameObject>("FungusAudioTag");
			if (go != null)
			{
				GameObject spawnedGO = PrefabUtility.InstantiatePrefab(go) as GameObject;
				spawnedGO.name = "AudioTag";
			}
		}

		[MenuItem("GameObject/Fungus/Portrait/Stage")]
		static void CreateStage()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Portrait/Prefabs/Stage.prefab");
		}
		
		[MenuItem("GameObject/Fungus/Portrait/StagePosition")]
		static void CreateStagePosition()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Portrait/Prefabs/StagePosition.prefab");
		}
	}

}