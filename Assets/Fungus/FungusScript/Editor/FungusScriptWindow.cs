using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public class FungusScriptWindow : EditorWindow
	{
		static bool locked = false;
		static GUIStyle lockButtonStyle;
		static FungusScript activeFungusScript;

		public static List<Sequence> deleteList = new List<Sequence>();

		protected List<Sequence> windowSequenceMap = new List<Sequence>();

		// The ReorderableList control doesn't drag properly when used with GUI.DragWindow(),
		// so we just implement dragging ourselves.
		protected bool dragging;
		protected Vector2 startDragPosition;
		protected Sequence selectedSequence;

	    [MenuItem("Window/Fungus Script")]
	    static void Init()
	    {
	        GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
	    }

		// Implementing this method causes the padlock image to display on the window
		// https://leahayes.wordpress.com/2013/04/30/adding-the-little-padlock-button-to-your-editorwindow/#more-455
		protected virtual void ShowButton(Rect position) {
			if (lockButtonStyle == null)
			{
				lockButtonStyle = "IN LockButton";
			}
			locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
		}

		public virtual void OnInspectorUpdate()
		{
			Repaint();
		}

		static public FungusScript GetFungusScript()
		{
			if (locked && activeFungusScript != null)
			{
				return activeFungusScript;
			}

			locked = false;

			if (Selection.activeGameObject != null)
			{
				activeFungusScript = Selection.activeGameObject.GetComponent<FungusScript>();
				return activeFungusScript;
			}

			return null;
		}

		protected virtual void OnGUI()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				GUILayout.Label("No Fungus Script scene object selected");
				return;
			}

			// Delete any scheduled objects
			foreach (Sequence deleteSequence in deleteList)
			{
				foreach (Command command in deleteSequence.commandList)
				{
					Undo.DestroyObjectImmediate(command);
				}
				
				Undo.DestroyObjectImmediate(deleteSequence);
				fungusScript.selectedSequence = null;
				fungusScript.selectedCommands.Clear();
			}
			deleteList.Clear();

			GUILayout.BeginHorizontal();
			DrawScriptView(fungusScript);
			GUILayout.EndHorizontal();		
		}
		
		protected virtual void DrawScriptView(FungusScript fungusScript)
		{
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>(true);

			foreach (Sequence s in sequences)
			{
				fungusScript.scrollViewRect.xMin = Mathf.Min(fungusScript.scrollViewRect.xMin, s.nodeRect.xMin - 400);
				fungusScript.scrollViewRect.xMax = Mathf.Max(fungusScript.scrollViewRect.xMax, s.nodeRect.xMax + 400);
				fungusScript.scrollViewRect.yMin = Mathf.Min(fungusScript.scrollViewRect.yMin, s.nodeRect.yMin - 400);
				fungusScript.scrollViewRect.yMax = Mathf.Max(fungusScript.scrollViewRect.yMax, s.nodeRect.yMax + 400);
			}

			// Calc rect for left hand script view
			Rect scriptViewRect = new Rect(0, 0, this.position.width, this.position.height);

			// Clip GL drawing so not to overlap scrollbars
			Rect clipRect = new Rect(fungusScript.scrollPos.x + fungusScript.scrollViewRect.x,
			                         fungusScript.scrollPos.y + fungusScript.scrollViewRect.y,
			                         scriptViewRect.width,
			                         scriptViewRect.height);

			GUILayoutUtility.GetRect(scriptViewRect.width, scriptViewRect.height);

			fungusScript.scrollPos = GLDraw.BeginScrollView(scriptViewRect, fungusScript.scrollPos, fungusScript.scrollViewRect, clipRect);
		
			Vector2 newNodePosition = new Vector2(fungusScript.scrollViewRect.xMin + fungusScript.scrollPos.x + 8, 
			                                      fungusScript.scrollViewRect.yMin + fungusScript.scrollPos.y + 8);

			if (GUI.Button(new Rect(newNodePosition.x, newNodePosition.y, 16, 16), "", new GUIStyle("OL Plus")))
			{
				Vector2 nodePosition = new Vector2(newNodePosition.x + fungusScript.scrollPos.x + 30,
				                                   newNodePosition.y + fungusScript.scrollPos.y + 30);

				CreateSequenceCallback(nodePosition);
			}

			if (Event.current.button == 0 && 
				Event.current.type == EventType.MouseDown)
			{
				selectedSequence = fungusScript.selectedSequence;
				fungusScript.selectedSequence = null;
				fungusScript.selectedCommands.Clear();
			}

			// Draw connections
			foreach (Sequence s in sequences)
			{
				DrawConnections(fungusScript, s, false);
			}
			foreach (Sequence s in sequences)
			{
				DrawConnections(fungusScript, s, true);
			}

			GUIStyle windowStyle = new GUIStyle();
			windowStyle.stretchHeight = true;

			BeginWindows();

			windowSequenceMap.Clear();
			for (int i = 0; i < sequences.Length; ++i)
			{
				Sequence sequence = sequences[i];

				// Hack to support legacy design where sequences were child gameobjects (will be removed soon)
				sequence.UpdateSequenceName();

				sequence.nodeRect.width = 240;
				sequence.nodeRect.height = CalcRectHeight(sequence.commandList.Count);

				GUILayout.Window(i, sequence.nodeRect, DrawWindow, "", windowStyle);

				GUI.backgroundColor = Color.white;

				windowSequenceMap.Add(sequence);
			}

			EndWindows();

			if (Event.current.button == 1 &&
			    Event.current.type == EventType.MouseDrag)
			{
				fungusScript.scrollPos -= Event.current.delta;
			}

			GLDraw.EndScrollView();
		}

		/*
		protected virtual void ResizeViews(FungusScript fungusScript)
		{
			cursorChangeRect = new Rect(this.position.width - fungusScript.commandViewWidth, 0, 4f, this.position.height);

			GUI.color = Color.grey;
			GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
			EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeHorizontal);
			
			if (Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
			{
				resize = true;
			}
			if (resize)
			{
				fungusScript.commandViewWidth = this.position.width - Event.current.mousePosition.x;
				fungusScript.commandViewWidth = Mathf.Max(minViewWidth, fungusScript.commandViewWidth);
				fungusScript.commandViewWidth = Mathf.Min(this.position.width - minViewWidth, fungusScript.commandViewWidth);
			}
			if(Event.current.type == EventType.MouseUp)
			{
				resize = false;        
			}
		}

		protected virtual void DrawSequenceView(FungusScript fungusScript)
		{
			GUILayout.Space(5);

			fungusScript.commandScrollPos = GUILayout.BeginScrollView(fungusScript.commandScrollPos);

			EditorGUILayout.BeginVertical();

			GUILayout.Box("Sequence", GUILayout.ExpandWidth(true));

			GUILayout.BeginHorizontal();
			
			if (fungusScript.selectedSequence == null)
			{
				GUILayout.FlexibleSpace();
			}
			
			if (GUILayout.Button(fungusScript.selectedSequence == null ? "Create Sequence" : "Create", 
			                     fungusScript.selectedSequence == null ?  EditorStyles.miniButton : EditorStyles.miniButtonLeft))
			{
				Vector2 newPosition;
				if (fungusScript.selectedSequence == null)
				{
					newPosition = newNodePosition;
				}
				else
				{
					Rect selectedRect = fungusScript.selectedSequence.nodeRect;
					newPosition = new Vector2(selectedRect.position.x + selectedRect.width + 20, selectedRect.y);
				}

				CreateSequence(fungusScript, newPosition);
			}
			
			if (fungusScript.selectedSequence == null)
			{
				GUILayout.FlexibleSpace();
			}
			else
			{
				if (GUILayout.Button("Delete", EditorStyles.miniButtonMid))
				{
					DeleteSequence(fungusScript, fungusScript.selectedSequence);
				}
				if (GUILayout.Button("Duplicate", EditorStyles.miniButtonRight))
				{
					DuplicateSequence(fungusScript, fungusScript.selectedSequence);
				}
			}
			
			GUILayout.EndHorizontal();

			if (fungusScript.selectedSequence != null)
			{
				EditorGUILayout.Separator();

				SequenceEditor sequenceEditor = Editor.CreateEditor(fungusScript.selectedSequence) as SequenceEditor;
				sequenceEditor.DrawSequenceGUI(fungusScript);
				DestroyImmediate(sequenceEditor);

				GUILayout.FlexibleSpace();
			}

			EditorGUILayout.EndVertical();

			GUILayout.EndScrollView();
		}
		*/

		public static Sequence CreateSequence(FungusScript fungusScript, Vector2 position)
		{
			Sequence newSequence = fungusScript.CreateSequence(position);
			Undo.RegisterCreatedObjectUndo(newSequence, "New Sequence");
			fungusScript.selectedSequence = newSequence;
			fungusScript.selectedCommands.Clear();

			return newSequence;
		}

		protected virtual void DeleteSequence(FungusScript fungusScript, Sequence sequence)
		{
			foreach (Command command in sequence.commandList)
			{
				Undo.DestroyObjectImmediate(command);
			}
			
			Undo.DestroyObjectImmediate(sequence);
			fungusScript.selectedSequence = null;
			fungusScript.selectedCommands.Clear();
		}

		protected virtual void CreateSequenceCallback(object item)
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript != null)
			{
				Vector2 position = (Vector2)item;
				position -= fungusScript.scrollPos;
				CreateSequence(fungusScript, position);
			}				
		}

		protected virtual void DrawWindow(int windowId)
		{
			Sequence sequence = windowSequenceMap[windowId];
			FungusScript fungusScript = sequence.GetFungusScript();

			if (!Application.isPlaying &&
			    Event.current.button == 0)
			{
			    if (Event.current.type == EventType.MouseDrag && dragging)
				{
					sequence.nodeRect.x += Event.current.delta.x;
					sequence.nodeRect.y += Event.current.delta.y;
				}
				else if (Event.current.type == EventType.MouseUp &&
				         dragging)
				{
					Vector2 newPos = new Vector2(sequence.nodeRect.x, sequence.nodeRect.y);

					sequence.nodeRect.x = startDragPosition.x;
					sequence.nodeRect.y = startDragPosition.y;

					Undo.RecordObject(sequence, "Node Position");

					sequence.nodeRect.x = newPos.x;
					sequence.nodeRect.y = newPos.y;

					dragging = false;
				}
			}
					
			// Select sequence when node is clicked
			if (!Application.isPlaying &&
			    Event.current.button == 0 && 
		    	(Event.current.type == EventType.MouseDown))
			{
				// Check if might be start of a window drag
				if (Event.current.mousePosition.y < 26)
				{
					dragging = true;
					startDragPosition.x = sequence.nodeRect.x;
					startDragPosition.y = sequence.nodeRect.y;
				}

				if (windowId < windowSequenceMap.Count)
				{
					Undo.RecordObject(fungusScript, "Select");
					if (sequence != fungusScript.selectedSequence || !EditorGUI.actionKey)
					{
						fungusScript.selectedCommands.Clear();
					}

					if (selectedSequence != sequence &&
					    Event.current.mousePosition.x > sequence.nodeRect.width - 30f)
					{
						Event.current.Use();
					}

					fungusScript.selectedSequence = sequence;
					GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
				}
			}

			if (fungusScript.selectedSequence == sequence ||
			    fungusScript.executingSequence == sequence)
			{
				GUI.backgroundColor = Color.green;
				
				Rect highlightRect = new Rect(0, 0, sequence.nodeRect.width, 24);
				GUI.Box(highlightRect, "");
				GUI.backgroundColor = Color.white;
			}

			GUILayout.BeginVertical();

			SequenceEditor sequenceEditor = Editor.CreateEditor(sequence) as SequenceEditor;
			sequenceEditor.DrawCommandListGUI(sequence.GetFungusScript());
			DestroyImmediate(sequenceEditor);

			GUILayout.EndVertical();
	    }

		protected virtual void DrawConnections(FungusScript fungusScript, Sequence sequence, bool highlightedOnly)
		{
			if (sequence == null)
			{
				return;
			}

			List<Sequence> connectedSequences = new List<Sequence>();

			bool sequenceIsSelected = (fungusScript.selectedSequence == sequence);

			int index = 0;

			foreach (Command command in sequence.commandList)
			{
				index++;

				bool commandIsSelected = false;
				foreach (Command selectedCommand in fungusScript.selectedCommands)
				{
					if (selectedCommand == command)
					{
						commandIsSelected = true;
						break;
					}
				}

				bool highlight = command.IsExecuting() || (sequenceIsSelected && commandIsSelected);

				if (highlightedOnly && !highlight ||
				    !highlightedOnly && highlight)
				{
					continue;
				}

				connectedSequences.Clear();
				command.GetConnectedSequences(ref connectedSequences);

				foreach (Sequence sequenceB in connectedSequences)
				{
					if (sequenceB == null ||
					    sequenceB.GetFungusScript() != fungusScript)
					{
						continue;
					}

					Rect startRect = new Rect(sequence.nodeRect);
					startRect.y += CalcRectHeight(sequence.commandList.Count);
					startRect.height = 0;

					Rect endRect = new Rect(sequenceB.nodeRect);
					endRect.height = 22;

					DrawRectConnection(startRect, endRect, highlight);
				}
			}
		}

		protected virtual void DrawRectConnection(Rect rectA, Rect rectB, bool highlight)
		{
			Vector2[] pointsA = new Vector2[] {
				new Vector2(rectA.xMin, rectA.center.y),
				new Vector2(rectA.xMin + rectA.width / 2, rectA.yMax + 15),
				new Vector2(rectA.xMax, rectA.center.y) 
			};

			Vector2[] pointsB = new Vector2[] {
				new Vector2(rectB.xMin, rectB.center.y + 4),
				new Vector2(rectB.xMin + rectB.width / 2, rectB.yMin),
				new Vector2(rectB.xMax, rectB.center.y + 4)
			};

			Vector2 pointA = Vector2.zero;
			Vector2 pointB = Vector2.zero;
			float minDist = float.MaxValue;

			foreach (Vector2 a in pointsA)
			{
				foreach (Vector2 b in pointsB)
				{
					float d = Vector2.Distance(a, b);
					if (d < minDist)
					{
						pointA = a;
						pointB = b;
						minDist = d;
					}
				}
			}

			Color color = Color.grey;
			if (highlight)
			{
				color = Color.green;
			}

			GLDraw.DrawConnectingCurve(pointA, pointB, color, 1.025f);

			Rect dotARect = new Rect(pointA.x - 5, pointA.y - 5, 10, 10);
			GUI.Label(dotARect, "", new GUIStyle("U2D.dragDotActive"));

			Rect dotBRect = new Rect(pointB.x - 5, pointB.y - 5, 10, 10);
			GUI.Label(dotBRect, "", new GUIStyle("U2D.dragDotActive"));
		}

		protected virtual float CalcRectHeight(int numCommands)
		{
			return (numCommands * 20) + 34;
		}
	}
}