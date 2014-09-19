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
			GameObject newSequenceGO = new GameObject();
			newSequenceGO.transform.parent = newFungusScriptGO.transform;
			newSequenceGO.name = "Start";
			newSequenceGO.hideFlags = HideFlags.HideInHierarchy;
			Sequence sequence = newSequenceGO.AddComponent<Sequence>();
			fungusScript.startSequence = sequence;
			fungusScript.selectedSequence = sequence;
			Undo.RegisterCreatedObjectUndo(newFungusScriptGO, "Create Fungus Script");
		}		
	}

}