using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System.Collections;

namespace Fungus
{
	/**
	 * Temp hidden object which lets us use the entire inspector window to inspect
	 * the block command list.
	 */
	public class BlockInspector : ScriptableObject 
	{
		[FormerlySerializedAs("sequence")]
		public Block block;
	}

	/**
	 * Custom editor for the temp hidden object.
	 */
	[CustomEditor (typeof(BlockInspector), true)]
	public class BlockInspectorEditor : Editor
	{
		protected Vector2 blockScrollPos;
		protected Vector2 commandScrollPos;
		protected bool resize = false;
		protected float topPanelHeight = 50;

		public override void OnInspectorGUI () 
		{
			BlockInspector blockInspector = target as BlockInspector;
			Block block = blockInspector.block;

			if (block == null)
			{
				return;
			}

			Flowchart flowchart = block.GetFlowchart();

			BlockEditor blockEditor = Editor.CreateEditor(block) as BlockEditor;
			blockEditor.DrawBlockName(flowchart);

			// Using a custom rect area to get the correct 5px indent for the scroll views
			Rect blockRect = new Rect(5, topPanelHeight, Screen.width - 6, Screen.height - 70);
			GUILayout.BeginArea(blockRect);

			blockScrollPos = GUILayout.BeginScrollView(blockScrollPos, GUILayout.Height(flowchart.blockViewHeight));
			blockEditor.DrawBlockGUI(flowchart);
			GUILayout.EndScrollView();

			Command inspectCommand = null;
			if (flowchart.selectedCommands.Count == 1)
			{
				inspectCommand = flowchart.selectedCommands[0];
			}

			if (Application.isPlaying &&
			    inspectCommand != null &&
			    inspectCommand.parentBlock != block)
			{
				GUILayout.EndArea();
				Repaint();
				DestroyImmediate(blockEditor);
				return;
			}

			ResizeScrollView(flowchart);

			GUILayout.Space(7);

			blockEditor.DrawButtonToolbar();

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
			Rect resizeRect = new Rect(0, topPanelHeight + flowchart.blockViewHeight + 1, Screen.width, 4f);
			GUI.color = new Color(0.64f, 0.64f, 0.64f);
			GUI.DrawTexture(resizeRect, EditorGUIUtility.whiteTexture);
			resizeRect.height = 1;
			GUI.color = new Color32(132, 132, 132, 255);
			GUI.DrawTexture(resizeRect, EditorGUIUtility.whiteTexture);
			resizeRect.y += 3;
			GUI.DrawTexture(resizeRect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;

			Repaint();

			DestroyImmediate(blockEditor);
		}

		private void ResizeScrollView(Flowchart flowchart)
		{
			Rect cursorChangeRect = new Rect(0, flowchart.blockViewHeight + 1, Screen.width, 4f);

			EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);
			
			if (Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
			{
				resize = true;
			}

			if (resize)
			{
				Undo.RecordObject(flowchart, "Resize view");
				flowchart.blockViewHeight = Event.current.mousePosition.y;
			}

			// Make sure block view is always visible
			float height = flowchart.blockViewHeight;
			height = Mathf.Max(200, height);
			height = Mathf.Min(Screen.height - 200,height);
			flowchart.blockViewHeight = height;

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
