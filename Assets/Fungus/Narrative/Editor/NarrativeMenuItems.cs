using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	// The prefab names are prefixed with Fungus to avoid clashes with any other prefabs in the project
	public class NarrativeMenuItems 
	{

		[MenuItem("Tools/Fungus/Create/Character", false, 50)]
		static void CreateCharacter()
		{
			GameObject go = FlowchartMenuItems.SpawnPrefab("Character");
			go.transform.position = Vector3.zero;
		}

		[MenuItem("Tools/Fungus/Create/Say Dialog", false, 51)]
		static void CreateSayDialog()
		{
			GameObject go = FlowchartMenuItems.SpawnPrefab("SayDialog");
			go.transform.position = Vector3.zero;
		}

		[MenuItem("Tools/Fungus/Create/Menu Dialog", false, 52)]
		static void CreateMenuDialog()
		{
			GameObject go = FlowchartMenuItems.SpawnPrefab("MenuDialog");
			go.transform.position = Vector3.zero;
		}

		[MenuItem("Tools/Fungus/Create/Tag", false, 53)]
		static void CreateTag()
		{
			GameObject go = FlowchartMenuItems.SpawnPrefab("Tag");
			go.transform.position = Vector3.zero;
		}

		[MenuItem("Tools/Fungus/Create/Audio Tag", false, 54)]
		static void CreateAudioTag()
		{
			GameObject go = FlowchartMenuItems.SpawnPrefab("AudioTag");
			go.transform.position = Vector3.zero;
		}

		[MenuItem("Tools/Fungus/Create/Stage", false, 55)]
		static void CreateStage()
		{
			GameObject go = FlowchartMenuItems.SpawnPrefab("Stage");
			go.transform.position = Vector3.zero;
		}
		
		[MenuItem("Tools/Fungus/Create/Stage Position", false, 56)]
		static void CreateStagePosition()
		{
			FlowchartMenuItems.SpawnPrefab("StagePosition");
		}

		[MenuItem("Tools/Fungus/Create/Localization", false, 57)]
		static void CreateLocalization()
		{
			GameObject go = FlowchartMenuItems.SpawnPrefab("Localization");
			go.transform.position = Vector3.zero;
		}
	}

}