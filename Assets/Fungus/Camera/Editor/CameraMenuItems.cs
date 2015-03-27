using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class CameraMenuItems 
	{
		[MenuItem("GameObject/Fungus/Camera/View")]
		static void CreateView()
		{
			FungusScriptMenuItems.SpawnPrefab("View");
		}
	}

}