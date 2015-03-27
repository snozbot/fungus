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
			FungusScriptMenuItems.SpawnPrefab("Character");
		}

		[MenuItem("GameObject/Fungus/Dialog/SayDialog")]
		static void CreateSayDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("SayDialog");
		}

		[MenuItem("GameObject/Fungus/Dialog/MenuDialog")]
		static void CreateMenuDialog()
		{
			FungusScriptMenuItems.SpawnPrefab("MenuDialog");
		}

		[MenuItem("GameObject/Fungus/Dialog/Tag")]
		static void CreateTag()
		{
			FungusScriptMenuItems.SpawnPrefab("Tag");
		}

		[MenuItem("GameObject/Fungus/Dialog/AudioTag")]
		static void CreateAudioTag()
		{
			FungusScriptMenuItems.SpawnPrefab("AudioTag");
		}

		[MenuItem("GameObject/Fungus/Portrait/Stage")]
		static void CreateStage()
		{
			FungusScriptMenuItems.SpawnPrefab("Stage");
		}
		
		[MenuItem("GameObject/Fungus/Portrait/StagePosition")]
		static void CreateStagePosition()
		{
			FungusScriptMenuItems.SpawnPrefab("StagePosition");
		}
	}

}