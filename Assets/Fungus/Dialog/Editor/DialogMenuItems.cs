using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	// The prefab names are prefixed with Fungus to avoid clashes with any other prefabs in the project
	public class DialogMenuItems 
	{

		[MenuItem("Tools/Fungus/Create/Character")]
		static void CreateCharacter()
		{
			FungusScriptMenuItems.SpawnPrefab("Character");
		}

		[MenuItem("Tools/Fungus/Create/Say Dialog")]
		static void CreateSayDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("SayDialog");
		}

		[MenuItem("Tools/Fungus/Create/Menu Dialog")]
		static void CreateMenuDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("MenuDialog");
		}

		[MenuItem("Tools/Fungus/Create/Tag")]
		static void CreateTag()
		{
			FungusScriptMenuItems.SpawnPrefab("Tag");
		}

		[MenuItem("Tools/Fungus/Create/Audio Tag")]
		static void CreateAudioTag()
		{
			FungusScriptMenuItems.SpawnPrefab("AudioTag");
		}

		[MenuItem("Tools/Fungus/Create/Stage")]
		static void CreateStage()
		{
			FungusScriptMenuItems.SpawnPrefab("Stage");
		}
		
		[MenuItem("Tools/Fungus/Create/Stage Position")]
		static void CreateStagePosition()
		{
			FungusScriptMenuItems.SpawnPrefab("StagePosition");
		}
	}

}