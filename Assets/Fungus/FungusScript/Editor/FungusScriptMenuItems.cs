using UnityEngine;
using UnityEditor;
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
			fungusScript.startSequence = sequence;
			fungusScript.selectedSequence = sequence;
			fungusScript.scrollPos = new Vector2(200, 200);
			Undo.RegisterCreatedObjectUndo(newFungusScriptGO, "Create Fungus Script");
		}		
	}

}