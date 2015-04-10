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
			FlowchartMenuItems.SpawnPrefab("Character");
		}

		[MenuItem("Tools/Fungus/Create/Say Dialog", false, 51)]
		static void CreateSayDialog()
		{
			FlowchartMenuItems.SpawnPrefab("SayDialog");
		}

		[MenuItem("Tools/Fungus/Create/Menu Dialog", false, 52)]
		static void CreateMenuDialog()
		{
			FlowchartMenuItems.SpawnPrefab("MenuDialog");
		}

		[MenuItem("Tools/Fungus/Create/Tag", false, 53)]
		static void CreateTag()
		{
			FlowchartMenuItems.SpawnPrefab("Tag");
		}

		[MenuItem("Tools/Fungus/Create/Audio Tag", false, 54)]
		static void CreateAudioTag()
		{
			FlowchartMenuItems.SpawnPrefab("AudioTag");
		}

		[MenuItem("Tools/Fungus/Create/Stage", false, 55)]
		static void CreateStage()
		{
			FlowchartMenuItems.SpawnPrefab("Stage");
		}
		
		[MenuItem("Tools/Fungus/Create/Stage Position", false, 56)]
		static void CreateStagePosition()
		{
			FlowchartMenuItems.SpawnPrefab("StagePosition");
		}

		[MenuItem("Tools/Fungus/Create/Localization", false, 57)]
		static void CreateLocalization()
		{
			FlowchartMenuItems.SpawnPrefab("Localization");
		}
	}

}