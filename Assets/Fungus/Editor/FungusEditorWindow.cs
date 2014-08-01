using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	public class FungusEditorWindow : EditorWindow
	{
		[System.NonSerialized]
		public Vector2 scrollPos;                         // ScrollViews use a Vector2 to track the state of each scroll bar
		
		private List<Sequence> windowSequenceMap = new List<Sequence>();
		
		private GameObject cachedSelection;

	    [MenuItem("Window/Fungus Editor")]
	    static void Init()
	    {
	        GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor");
	    }

		public void OnInspectorUpdate()
		{
			Repaint();
		}

		FungusScript GetFungusScript()
		{
			GameObject activeObject = Selection.activeGameObject;

			while (activeObject != null)
			{
				FungusScript fungusScript = activeObject.GetComponent<FungusScript>();
				Sequence sequence = activeObject.GetComponent<Sequence>();

				if (fungusScript != null)
				{
					// Found sequence controller
					return fungusScript;
				}
				else if (sequence != null &&
				         sequence.transform.parent != null)
				{
					// Check parent for sequence controller
					activeObject = sequence.transform.parent.gameObject;
				}
				else
				{
					activeObject = null;
				}
			}

			return null;
		}

	    void OnGUI()
	    {
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>();

			Rect scrollViewRect = new Rect();

			foreach (Sequence s in sequences)
			{
				scrollViewRect.xMin = Mathf.Min(scrollViewRect.xMin, s.nodeRect.xMin);
				scrollViewRect.xMax = Mathf.Max(scrollViewRect.xMax, s.nodeRect.xMax);
				scrollViewRect.yMin = Mathf.Min(scrollViewRect.yMin, s.nodeRect.yMin);
				scrollViewRect.yMax = Mathf.Max(scrollViewRect.yMax, s.nodeRect.yMax);
			}

			// Empty buffer area around edges of scroll rect
			float bufferScale = 0.1f;
			scrollViewRect.xMin -= position.width * bufferScale;
			scrollViewRect.yMin -= position.height * bufferScale;
			scrollViewRect.xMax += position.width * bufferScale;
			scrollViewRect.yMax += position.height * bufferScale;

			scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, scrollViewRect);

	        // In games, GUI.Window pops up a window on your screen. In the Editor, GUI.Window shows a sub-window inside an EditorWindow.
	        // All calls to GUI.Window need to be wrapped in a BeginWindows / EndWindows pair.
	        // http://docs.unity3d.com/Documentation/ScriptReference/EditorWindow.BeginWindows.html
	        BeginWindows();

			windowSequenceMap.Clear();
			for (int i = 0; i < sequences.Length; ++i)
			{
				Sequence sequence = sequences[i];
				sequence.nodeRect = GUILayout.Window(i, sequence.nodeRect, DrawWindow, sequence.name, GUILayout.Width(100), GUILayout.Height(100), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				windowSequenceMap.Add(sequence);
			}

			// Draw connections
			foreach (Sequence s in windowSequenceMap)
			{
				DrawConnections(s, false);
			}
			foreach (Sequence s in windowSequenceMap)
			{
				DrawConnections(s, true);
			}

	        EndWindows();

	        GUI.EndScrollView();

			string labelText = fungusScript.name;
			if (Application.isPlaying)
			{
				if (fungusScript.activeSequence == null)
				{
					labelText += " (Idle)";
				}
				else
				{
					labelText += " (Running)";
				}
			}
			else
			{
				labelText += " (Edit)";
			}
			
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			if (GUILayout.Button(labelText))
			{
				Selection.activeGameObject = fungusScript.gameObject;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(15);
			GUILayout.EndVertical();
	    }

	    void DrawWindow(int windowId)
	    {
			// Select game object when node is clicked
			if (Event.current.button == 0 && 
		    	Event.current.type == EventType.MouseUp) 
			{
				if (windowId < windowSequenceMap.Count)
				{
					Sequence s = windowSequenceMap[windowId];
					if (s != null)
					{
						Selection.activeGameObject = s.gameObject;
					}
				}
			}

			if (FungusCommandEditor.selectedCommand != null)
			{
				if (Selection.activeGameObject == null)
				{
					FungusCommandEditor.selectedCommand = null;
				}
				else
				{
					FungusCommand command = Selection.activeGameObject.GetComponent<FungusCommand>();
					if (command == null)
					{
						FungusCommandEditor.selectedCommand = null;
					}
					else if (command.gameObject != FungusCommandEditor.selectedCommand.gameObject)
					{
						FungusCommandEditor.selectedCommand = null;
					}
				}
			}

			Sequence sequence = windowSequenceMap[windowId];

			GUIStyle style = new GUIStyle(GUI.skin.button);

			FungusCommand[] commands = sequence.gameObject.GetComponents<FungusCommand>();
			foreach (FungusCommand command in commands)
			{
				string commandName = command.GetType().Name;
				commandName = commandName.Replace("Command", "");

				if (command.errorMessage.Length > 0)
				{
					GUI.backgroundColor = Color.red;
				}
				else if (ShouldHighlight(command))
				{
					if (command.IsExecuting())
					{
						GUI.backgroundColor = Color.green;
					}
					else
					{
						GUI.backgroundColor = Color.yellow;
					}
				}
				else
				{
					GUI.backgroundColor = Color.white;
				}

				if (GUILayout.Button(commandName, style, GUILayout.ExpandWidth(true)))
				{
					// Highlight the command in inspector
					FungusCommandEditor.selectedCommand = command;
				}

				EditorUtility.SetDirty( command );
			}

			// Add an invisible element if there are no commands to avoid window width/height collapsing
			if (commands.Length == 0)
			{
				GUILayout.Space(10);
			}

	        GUI.DragWindow();
	    }

		void DrawConnections(Sequence sequence, bool highlightedOnly)
		{
			List<Sequence> connectedSequences = new List<Sequence>();

			FungusCommand[] commands = sequence.GetComponentsInChildren<FungusCommand>();
			foreach (FungusCommand command in commands)
			{
				bool highlight = ShouldHighlight(command);

				if (highlightedOnly && !highlight ||
				    !highlightedOnly && highlight)
				{
					continue;
				}

				connectedSequences.Clear();
				command.GetConnectedSequences(ref connectedSequences);

				foreach (Sequence sequenceB in connectedSequences)
				{
					Rect rectA = sequence.nodeRect;
					Rect rectB = sequenceB.nodeRect;

					Vector2 pointA;
					Vector2 pointB;

					Vector2 p1 = rectA.center;
					Vector2 p2 = rectB.center;
					GLDraw.segment_rect_intersection(rectA, ref p1, ref p2);
					pointA = p2;

					p1 = rectB.center;
					p2 = rectA.center;
					GLDraw.segment_rect_intersection(rectB, ref p1, ref p2);
					pointB = p2;

					Color color = Color.grey;
					if (highlight)
					{
						if (command.IsExecuting())
						{
							color = Color.green;
						}
						else
						{
							color = Color.yellow;
						}
					}

					GLDraw.DrawConnectingCurve(pointA, pointB, color, 2);
				}
			}
		}

		bool ShouldHighlight(FungusCommand command)
		{
			return (command.IsExecuting() || (FungusCommandEditor.selectedCommand == command));
		}
	}

}