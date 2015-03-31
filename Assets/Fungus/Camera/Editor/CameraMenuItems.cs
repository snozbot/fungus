using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class CameraMenuItems 
	{
		[MenuItem("Tools/Fungus/Create/View", false, 100)]
		static void CreateView()
		{
			FlowchartMenuItems.SpawnPrefab("View");
		}
	}

}