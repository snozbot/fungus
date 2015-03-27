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

			// Using a custom rect area to get the correct 5px indent for the scroll views
			Rect sequenceRect = new Rect(5, topPanelHeight, Screen.width - 6, Screen.height);
			GUILayout.BeginArea(sequenceRect);

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
				GUILayout.EndArea();
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

			GUILayout.EndArea();

			// Draw the resize bar after everything else has finished drawing
			// This is mainly to avoid incorrect indenting.
			Rect resizeRect = new Rect(0, topPanelHeight + fungusScript.sequenceViewHeight + 1, Screen.width, 4f);
			GUI.color = new Color(0.64f, 0.64f, 0.64f);
			GUI.DrawTexture(resizeRect, EditorGUIUtility.whiteTexture);
			resizeRect.height = 1;
			GUI.color = new Color32(132, 132, 132, 255);
			GUI.DrawTexture(resizeRect, EditorGUIUtility.whiteTexture);
			resizeRect.y += 3;
			GUI.DrawTexture(resizeRect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;

			Repaint();

			DestroyImmediate(sequenceEditor);
		}

		private void ResizeScrollView(FungusScript fungusScript)
		{
			Rect cursorChangeRect = new Rect(0, fungusScript.sequenceViewHeight + 1, Screen.width, 4f);

			EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);
			
			if (Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
			{
				resize = true;
			}

			if (resize)
			{
				Undo.RecordObject(fungusScript, "Resize view");
				fungusScript.sequenceViewHeight = Event.current.mousePosition.y;
			}

			// Make sure sequence view is always visible
			float height = fungusScript.sequenceViewHeight;
			height = Mathf.Max(200, height);
			height = Mathf.Min(Screen.height - 200,height);
			fungusScript.sequenceViewHeight = height;

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
