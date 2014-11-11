using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public class FungusScriptWindow : EditorWindow
	{
		static GUIStyle lockButtonStyle;

		public static List<Sequence> deleteList = new List<Sequence>();

		protected List<Sequence> windowSequenceMap = new List<Sequence>();

		// The ReorderableList control doesn't drag properly when used with GUI.DragWindow(),
		// so we just implement dragging ourselves.
		protected int dragWindowId = -1;
		protected Vector2 startDragPosition;
		protected Sequence selectedSequence;

		protected const float minZoomValue = 0.25f;
		protected const float maxZoomValue = 1f;

		protected static SequenceInspector sequenceInspector;
		protected bool followExecution = true;

		[MenuItem("Window/Fungus Script")]
	    static void Init()
	    {
	        GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
	    }

		public virtual void OnEnable()
		{
			followExecution = true;
		}

		protected void OnInspectorUpdate()
		{
			Repaint();
		}

		static public FungusScript GetFungusScript()
		{
			// Using a temp hidden object to track the active Fungus Script across 
			// serialization / deserialization when playing the game in the editor.
			FungusState fungusState = GameObject.FindObjectOfType<FungusState>();
			if (fungusState == null)
			{
				GameObject go = new GameObject("_FungusState");
				go.hideFlags = HideFlags.HideInHierarchy;
				fungusState = go.AddComponent<FungusState>();
			}

			if (Selection.activeGameObject != null)
			{
				FungusScript fs = Selection.activeGameObject.GetComponent<FungusScript>();
				if (fs != null)
				{
					fungusState.selectedFungusScript = fs;
				}
			}

			return fungusState.selectedFungusScript;
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
				SetSelectedSequence(fungusScript, null);
				fungusScript.ClearSelectedCommands();
			}
			deleteList.Clear();

			DrawScriptView(fungusScript);
			DrawOverlay(fungusScript);

			if (Application.isPlaying &&
			    fungusScript.executingSequence != null && 
			    followExecution)
			{
				// Set SequenceInspector object as the selected object
				if (Selection.activeGameObject == fungusScript.gameObject ||
					Selection.activeGameObject == sequenceInspector)
				{
					SetSelectedSequence(fungusScript, fungusScript.executingSequence);
				}

				// Make sure SequenceInspector is using the currently executing sequence
				if (sequenceInspector != null)
				{
					sequenceInspector.sequence = fungusScript.executingSequence;
				}
			}

			// Redraw on next frame to get crisp refresh rate
			Repaint();
		}

		protected virtual void DrawOverlay(FungusScript fungusScript)
		{
			GUILayout.Space(8);
			
			GUILayout.BeginHorizontal();
			
			GUILayout.Space(8);
			
			if (GUILayout.Button(FungusEditorResources.texAddButton))
			{
				Vector2 newNodePosition = new Vector2(50 - fungusScript.scrollPos.x, 
				                                      50 - fungusScript.scrollPos.y);
				CreateSequence(fungusScript, newNodePosition);
			}

			GUILayout.Space(8);

			fungusScript.zoom = GUILayout.HorizontalSlider(fungusScript.zoom, minZoomValue, maxZoomValue, GUILayout.Width(100));

			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical(GUILayout.Width(300));
		
			GUILayout.FlexibleSpace();

			fungusScript.variablesScrollPos = GUILayout.BeginScrollView(fungusScript.variablesScrollPos, GUILayout.MaxHeight(position.height * 0.75f));

			GUILayout.FlexibleSpace();

			GUILayout.Space(8);

			FungusScriptEditor fungusScriptEditor = Editor.CreateEditor (fungusScript) as FungusScriptEditor;
			fungusScriptEditor.DrawVariablesGUI();
			DestroyImmediate(fungusScriptEditor);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();

			GUILayout.FlexibleSpace();

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

			// Calc rect for script view
			Rect scriptViewRect = new Rect(0, 0, this.position.width / fungusScript.zoom, this.position.height / fungusScript.zoom);

			EditorZoomArea.Begin(fungusScript.zoom, scriptViewRect);

			DrawGrid(fungusScript);
			
			GLDraw.BeginGroup(scriptViewRect);

			if (Event.current.button == 0 && 
				Event.current.type == EventType.MouseDown)
			{
				selectedSequence = fungusScript.selectedSequence;
				SetSelectedSequence(fungusScript, null);
				if (!EditorGUI.actionKey)
				{
					fungusScript.ClearSelectedCommands();
				}
				Selection.activeGameObject = fungusScript.gameObject;
				followExecution = false;
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

			GUIStyle nodeStyle = new GUIStyle("flow node 3");

			windowSequenceMap.Clear();
			for (int i = 0; i < sequences.Length; ++i)
			{
				Sequence sequence = sequences[i];

				// Hack to support legacy design where sequences were child gameobjects (will be removed soon)
				sequence.UpdateSequenceName();

				float nodeWidth = nodeStyle.CalcSize(new GUIContent(sequence.sequenceName)).x;

				sequence.nodeRect.width = Mathf.Max(120, nodeWidth);
				sequence.nodeRect.height = 30;

				if (Event.current.button == 0)
				{
					if (Event.current.type == EventType.MouseDrag && dragWindowId == i)
					{
						sequence.nodeRect.x += Event.current.delta.x;
						sequence.nodeRect.y += Event.current.delta.y;
					}
					else if (Event.current.type == EventType.MouseUp &&
					         dragWindowId == i)
					{
						Vector2 newPos = new Vector2(sequence.nodeRect.x, sequence.nodeRect.y);
						
						sequence.nodeRect.x = startDragPosition.x;
						sequence.nodeRect.y = startDragPosition.y;
						
						Undo.RecordObject(sequence, "Node Position");
						
						sequence.nodeRect.x = newPos.x;
						sequence.nodeRect.y = newPos.y;

						dragWindowId = -1;
					}
				}

				Rect windowRect = new Rect(sequence.nodeRect);
				windowRect.x += fungusScript.scrollPos.x;
				windowRect.y += fungusScript.scrollPos.y;

				GUILayout.Window(i, windowRect, DrawWindow, "", windowStyle);

				GUI.backgroundColor = Color.white;

				windowSequenceMap.Add(sequence);
			}

			EndWindows();

			// Right click to drag view
			if (Event.current.button == 1 && Event.current.type == EventType.MouseDrag)
			{
				fungusScript.scrollPos += Event.current.delta;
			}
			else if (Event.current.type == EventType.ScrollWheel)
			{
				fungusScript.zoom -= Event.current.delta.y * 0.01f;
				fungusScript.zoom = Mathf.Clamp(fungusScript.zoom, minZoomValue, maxZoomValue);
			}

			GLDraw.EndGroup();

			EditorZoomArea.End();
		}

		protected virtual void DrawGrid(FungusScript fungusScript)
		{
			float width = this.position.width / fungusScript.zoom;
			float height = this.position.height / fungusScript.zoom;

			// Match background color of scene view
			if (EditorGUIUtility.isProSkin)
			{
				GUI.color = new Color32(71, 71, 71, 255); 
			}
			else
			{
				GUI.color = new Color32(86, 86, 86, 255); 
			}
			GUI.DrawTexture( new Rect(0,0, width, height), EditorGUIUtility.whiteTexture );

			GUI.color = Color.white;
			Color color = new Color32(96, 96, 96, 255);

			float gridSize = 128f;
			
			float x = fungusScript.scrollPos.x % gridSize;
			while (x < width)
			{
				GLDraw.DrawLine(new Vector2(x, 0), new Vector2(x, height), color, 1f);
				x += gridSize;
			}
			
			float y = fungusScript.scrollPos.y % gridSize;
			while (y < height)
			{
				GLDraw.DrawLine(new Vector2(0, y), new Vector2(width, y), color, 1f);
				y += gridSize;
			}
		}

		public static Sequence CreateSequence(FungusScript fungusScript, Vector2 position)
		{
			Sequence newSequence = fungusScript.CreateSequence(position);
			Undo.RegisterCreatedObjectUndo(newSequence, "New Sequence");
			SetSelectedSequence(fungusScript, newSequence);
			fungusScript.ClearSelectedCommands();

			return newSequence;
		}

		protected virtual void DeleteSequence(FungusScript fungusScript, Sequence sequence)
		{
			foreach (Command command in sequence.commandList)
			{
				Undo.DestroyObjectImmediate(command);
			}
			
			Undo.DestroyObjectImmediate(sequence);
			SetSelectedSequence(fungusScript, null);
			fungusScript.ClearSelectedCommands();
		}

		protected virtual void DrawWindow(int windowId)
		{
			Sequence sequence = windowSequenceMap[windowId];
			FungusScript fungusScript = sequence.GetFungusScript();
								
			// Select sequence when node is clicked
			if (Event.current.button == 0 && 
		    	Event.current.type == EventType.MouseDown)
			{
				// Check if might be start of a window drag
				if (Event.current.button == 0 &&
				    Event.current.mousePosition.y < 26)
				{
					dragWindowId = windowId;
					startDragPosition.x = sequence.nodeRect.x;
					startDragPosition.y = sequence.nodeRect.y;
				}

				if (windowId < windowSequenceMap.Count)
				{
					Undo.RecordObject(fungusScript, "Select");

					SetSelectedSequence(fungusScript, sequence);
					GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)

					if (Application.isPlaying)
					{
						// If user selected a non-executing sequence then stop following execution
						followExecution = (fungusScript.selectedSequence == fungusScript.executingSequence);
					}
				}
			}

			GUIStyle nodeStyle = null;
			if (fungusScript.selectedSequence == sequence ||
			    fungusScript.executingSequence == sequence)
			{
				// Green node
				nodeStyle = new GUIStyle("flow node 3");
			}
			else
			{
				// Yellow node
				nodeStyle = new GUIStyle("flow node 4");
			}

			GUILayout.Box(sequence.sequenceName, nodeStyle, GUILayout.Width(sequence.nodeRect.width), GUILayout.Height(sequence.nodeRect.height));

			if (Event.current.type == EventType.ContextClick)
			{
				GenericMenu menu = new GenericMenu ();
				
				menu.AddItem(new GUIContent ("Duplicate"), false, DuplicateSequence, sequence);
				menu.AddItem(new GUIContent ("Delete"), false, DeleteSequence, sequence);

				menu.ShowAsContext();			
			}
	    }

		protected virtual void DrawConnections(FungusScript fungusScript, Sequence sequence, bool highlightedOnly)
		{
			if (sequence == null)
			{
				return;
			}

			List<Sequence> connectedSequences = new List<Sequence>();

			bool sequenceIsSelected = (fungusScript.selectedSequence == sequence);

			foreach (Command command in sequence.commandList)
			{
				if (command == null)
				{
					continue;
				}

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
					startRect.x += fungusScript.scrollPos.x;
					startRect.y += fungusScript.scrollPos.y;

					Rect endRect = new Rect(sequenceB.nodeRect);
					endRect.x += fungusScript.scrollPos.x;
					endRect.y += fungusScript.scrollPos.y;

					DrawRectConnection(startRect, endRect, highlight);
				}
			}
		}

		protected virtual void DrawRectConnection(Rect rectA, Rect rectB, bool highlight)
		{
			Vector2[] pointsA = new Vector2[] {
				new Vector2(rectA.xMin, rectA.center.y),
				new Vector2(rectA.xMin + rectA.width / 2, rectA.yMin),
				new Vector2(rectA.xMin + rectA.width / 2, rectA.yMax),
				new Vector2(rectA.xMax, rectA.center.y) 
			};

			Vector2[] pointsB = new Vector2[] {
				new Vector2(rectB.xMin, rectB.center.y),
				new Vector2(rectB.xMin + rectB.width / 2, rectB.yMin),
				new Vector2(rectB.xMin + rectB.width / 2, rectB.yMax),
				new Vector2(rectB.xMax, rectB.center.y)
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

		public static void DeleteSequence(object obj)
		{
			Sequence sequence = obj as Sequence;
			FungusScriptWindow.deleteList.Add(sequence);
		}
		
		protected static void DuplicateSequence(object obj)
		{
			FungusScript fungusScript = GetFungusScript();
			Sequence sequence = obj as Sequence;

			Vector2 newPosition = new Vector2(sequence.nodeRect.position.x + 
			                                  sequence.nodeRect.width + 20, 
			                                  sequence.nodeRect.y);

			Sequence oldSequence = sequence;

			Sequence newSequence = FungusScriptWindow.CreateSequence(fungusScript, newPosition);
			newSequence.sequenceName = oldSequence.sequenceName + " (Copy)";

			foreach (Command command in oldSequence.commandList)
			{
				System.Type type = command.GetType();
				Command newCommand = Undo.AddComponent(fungusScript.gameObject, type) as Command;
				System.Reflection.FieldInfo[] fields = type.GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(newCommand, field.GetValue(command));
				}
				newSequence.commandList.Add(newCommand);
			}
		}

		protected static void SetSelectedSequence(FungusScript fungusScript, Sequence sequence)
		{
			fungusScript.selectedSequence = sequence;

			if (sequenceInspector == null)
			{
				// Create a Scriptable Object with a custom editor which we can use to inspect the selected sequence.
				// Editors for Scriptable Objects display using the full height of the inspector window.
				sequenceInspector = ScriptableObject.CreateInstance<SequenceInspector>() as SequenceInspector;
				sequenceInspector.hideFlags = HideFlags.DontSave;
			}

			sequenceInspector.sequence = sequence;

			if (sequence != null)
			{
				Selection.activeObject = sequenceInspector;
			}

			EditorUtility.SetDirty(sequenceInspector);
		}
	}
}