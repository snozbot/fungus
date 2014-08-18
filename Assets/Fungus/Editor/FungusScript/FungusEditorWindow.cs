using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	public class FungusEditorWindow : EditorWindow
	{
		private List<Sequence> windowSequenceMap = new List<Sequence>();
		
	    [MenuItem("Window/Fungus Editor")]
	    static void Init()
	    {
	        GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor");
	    }

		public void OnInspectorUpdate()
		{
			Repaint();
		}

		static public FungusScript GetFungusScript()
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

			EditorUtility.SetDirty(fungusScript);

			if (Event.current.button == 0 && 
			    Event.current.type == EventType.MouseDown) 
			{
				fungusScript.selectedSequence = null;
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
			float bufferScale = 0.25f;
			scrollViewRect.xMin -= position.width * bufferScale;
			scrollViewRect.yMin -= position.height * bufferScale;
			scrollViewRect.xMax += position.width * bufferScale;
			scrollViewRect.yMax += position.height * bufferScale;

			fungusScript.scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), fungusScript.scrollPos, scrollViewRect);

	        BeginWindows();

			GUIStyle windowStyle = new GUIStyle(GUI.skin.window);

			windowSequenceMap.Clear();
			for (int i = 0; i < sequences.Length; ++i)
			{
				Sequence sequence = sequences[i];
			
				float titleWidth = windowStyle.CalcSize(new GUIContent(sequence.name)).x;
				float windowWidth = Mathf.Max (titleWidth + 10, 100);

				sequence.nodeRect = GUILayout.Window(i, sequence.nodeRect, DrawWindow, sequence.name, GUILayout.Width(windowWidth), GUILayout.Height(20), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

				windowSequenceMap.Add(sequence);
			}

			// Draw connections
			foreach (Sequence s in windowSequenceMap)
			{
				DrawConnections(fungusScript, s, false);
			}
			foreach (Sequence s in windowSequenceMap)
			{
				DrawConnections(fungusScript, s, true);
			}

	        EndWindows();

			if (fungusScript.selectedSequence != null ||
			    fungusScript.executingSequence != null)
			{
				Rect outlineRect = new Rect();
				if (fungusScript.executingSequence != null)
				{
					outlineRect = fungusScript.executingSequence.nodeRect;
				}
				else if (fungusScript.selectedSequence != null)
				{
					outlineRect = fungusScript.selectedSequence.nodeRect;
				}
				outlineRect.width += 10;
				outlineRect.x -= 5;
				outlineRect.height += 10;
				outlineRect.y -= 5;
				GLDraw.DrawBox(outlineRect, Color.green, 2);
			}

			GUI.EndScrollView();

			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUI.backgroundColor = Color.white;
			if (GUILayout.Button("Show Variables"))
			{
				EditorWindow.GetWindow<VariablesWindow>(true, "Fungus Variables");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(15);
			GUILayout.EndVertical();
	    }

	    void DrawWindow(int windowId)
	    {
			// Select sequence when node is clicked
			if (Event.current.button == 0 && 
		    	Event.current.type == EventType.MouseDown) 
			{
				if (windowId < windowSequenceMap.Count)
				{
					Sequence s = windowSequenceMap[windowId];
					if (s != null)
					{
						FungusScript fungusScript = s.GetFungusScript();
						fungusScript.selectedSequence = s;
					}
				}
			}

			Sequence sequence = windowSequenceMap[windowId];

			GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.wordWrap = true;
			GUILayout.Label(sequence.description, labelStyle);

			GUILayout.Space(1);

	        GUI.DragWindow();
	    }

		void DrawConnections(FungusScript fungusScript, Sequence sequence, bool highlightedOnly)
		{
			List<Sequence> connectedSequences = new List<Sequence>();

			bool sequenceIsSelected = (fungusScript.selectedSequence == sequence);

			FungusCommand[] commands = sequence.GetComponentsInChildren<FungusCommand>();
			foreach (FungusCommand command in commands)
			{
				bool highlight = command.IsExecuting() || (sequenceIsSelected && command.expanded);

				if (highlightedOnly && !highlight ||
				    !highlightedOnly && highlight)
				{
					continue;
				}

				connectedSequences.Clear();
				command.GetConnectedSequences(ref connectedSequences);

				foreach (Sequence sequenceB in connectedSequences)
				{
					DrawRectConnection(sequence.nodeRect, sequenceB.nodeRect, highlight);
				}
			}
		}

		void DrawRectConnection(Rect rectA, Rect rectB, bool highlight)
		{
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
				color = Color.green;
			}
			
			GLDraw.DrawConnectingCurve(pointA, pointB, color, 2);
		}
	}

}