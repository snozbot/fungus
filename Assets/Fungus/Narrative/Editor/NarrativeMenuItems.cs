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
			FungusScriptMenuItems.SpawnPrefab("Character");
		}

		[MenuItem("Tools/Fungus/Create/Say Dialog", false, 51)]
		static void CreateSayDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("SayDialog");
		}

		[MenuItem("Tools/Fungus/Create/Menu Dialog", false, 52)]
		static void CreateMenuDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("MenuDialog");
		}

		[MenuItem("Tools/Fungus/Create/Tag", false, 53)]
		static void CreateTag()
		{
			FungusScriptMenuItems.SpawnPrefab("Tag");
		}

		[MenuItem("Tools/Fungus/Create/Audio Tag", false, 54)]
		static void CreateAudioTag()
		{
			FungusScriptMenuItems.SpawnPrefab("AudioTag");
		}

		[MenuItem("Tools/Fungus/Create/Stage", false, 55)]
		static void CreateStage()
		{
			FungusScriptMenuItems.SpawnPrefab("Stage");
		}
		
		[MenuItem("Tools/Fungus/Create/Stage Position", false, 56)]
		static void CreateStagePosition()
		{
			FungusScriptMenuItems.SpawnPrefab("StagePosition");
		}
	}

}