using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{
	/**
	 * Temp hidden object which lets us use the entire inspector window to inspect
	 * the sequence command list.
	 */
	public class SequenceInspector : ScriptableObject 
	{
		public Sequence sequence;
	}

	/**
	 * Custom editor for the temp hidden object.
	 */
	[CustomEditor (typeof(SequenceInspector), true)]
	public class SequenceInspectorEditor : Editor
	{
		protected Vector2 sequenceScrollPos;
		protected Vector2 commandScrollPos;
		protected bool resize = false;
		protected float topPanelHeight = 50;

		public override void OnInspectorGUI () 
		{
			SequenceInspector sequenceInspector = target as SequenceInspector;
			Sequence sequence = sequenceInspector.sequence;

			if (sequence == null)
			{
				return;
			}

			FungusScript fungusScript = sequence.GetFungusScript();

			SequenceEditor sequenceEditor = Editor.CreateEditor(sequence) as SequenceEditor;
			sequenceEditor.DrawSequenceName(fungusScript);

			sequenceScrollPos = GUILayout.BeginScrollView(sequenceScrollPos, GUILayout.Height(fungusScript.sequenceViewHeight));
			sequenceEditor.DrawSequenceGUI(fungusScript);
			GUILayout.EndScrollView();

			Command inspectCommand = null;
			if (fungusScript.selectedCommands.Count == 1)
			{
				inspectCommand = fungusScript.selectedCommands[0];
			}

			if (Application.isPlaying &&
			    inspectCommand != null &&
			    inspectCommand.parentSequence != sequence)
			{
				Repaint();
				DestroyImmediate(sequenceEditor);
				return;
			}

			ResizeScrollView(fungusScript);

			GUILayout.Space(7);

			sequenceEditor.DrawButtonToolbar();

			commandScrollPos = GUILayout.BeginScrollView(commandScrollPos);

			if (inspectCommand != null)
			{
				CommandEditor commandEditor = Editor.CreateEditor(inspectCommand) as CommandEditor;
				commandEditor.DrawCommandInspectorGUI();
				DestroyImmediate(commandEditor);
			}

			GUILayout.EndScrollView();

			Repaint();

			DestroyImmediate(sequenceEditor);
		}

		private void ResizeScrollView(FungusScript fungusScript)
		{
			Rect cursorChangeRect = new Rect(0, fungusScript.sequenceViewHeight + topPanelHeight, Screen.width, 4f);
			
			GUI.color = Color.grey;
			GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;

			EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);
			
			if (Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
			{
				resize = true;
			}
			
			if (resize)
			{
				float height = Event.current.mousePosition.y - topPanelHeight;
				height = Mathf.Max(200, height);
				height = Mathf.Min(Screen.height - 200,height);

				Undo.RecordObject(fungusScript, "Resize view");
				fungusScript.sequenceViewHeight = height;
			}

			// Stop resizing if mouse is outside inspector window.
			// This isn't standard Unity UI behavior but it is robust and safe.
			if (resize && Event.current.type == EventType.mouseDrag)
			{
				Rect windowRect = new Rect(0, 0, Screen.width, Screen.height);
				if (!windowRect.Contains(Event.current.mousePosition))
				{
					resize = false;
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				resize = false;
			}
		}
	}

}
