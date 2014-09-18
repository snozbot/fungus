using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CustomEditor (typeof(Call))]
	public class CallEditor : CommandEditor 
	{
		protected SerializedProperty targetSequenceProp;

		protected virtual void OnEnable()
		{
			targetSequenceProp = serializedObject.FindProperty("targetSequence");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			Call t = target as Call;

			FungusScript fungusScript = t.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			SequenceEditor.SequenceField(targetSequenceProp,
			                             new GUIContent("Target Sequence", "Sequence to call"), 
										 new GUIContent("<Continue>"), 
										 fungusScript);

			serializedObject.ApplyModifiedProperties();
		}
	}

}
