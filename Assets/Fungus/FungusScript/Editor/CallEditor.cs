using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Call))]
	public class CallEditor : FungusCommandEditor 
	{
		public override void DrawCommandGUI()
		{
			Call t = target as Call;

			FungusScript fungusScript = t.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			Sequence targetSequence = SequenceEditor.SequenceField(new GUIContent("Target Sequence", "Sequence to call"), 
			                                                       new GUIContent("<Continue>"), 
			                                                       fungusScript, 
			                                                       t.targetSequence);
			if (targetSequence != t.targetSequence)
			{
				Undo.RecordObject(t, "Set Target Sequence");
				t.targetSequence = targetSequence;
			}
		}
	}

}
