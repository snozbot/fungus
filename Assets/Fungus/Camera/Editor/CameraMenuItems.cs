using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class CameraMenuItems 
	{
		[MenuItem("Tools/Fungus/Create/View")]
		static void CreateView()
		{
			FungusScriptMenuItems.SpawnPrefab("View");
		}
	}

}