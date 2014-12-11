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

		protected const float minZoomValue = 0.25f;
		protected const float maxZoomValue = 1f;

		protected GUIStyle nodeStyle = new GUIStyle();

		protected static SequenceInspector sequenceInspector;

		public const float playIconFadeTime = 0.5f;

		[MenuItem("Window/Fungus Script")]
	    static void Init()
	    {
	        GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
	    }

		protected virtual void OnEnable()
		{
			// All sequence nodes use the same GUIStyle, but with a different background
			nodeStyle.border.left = 20;
			nodeStyle.border.right = 20;
			nodeStyle.border.top = 5;
			nodeStyle.border.bottom = 5;
			nodeStyle.padding.left = 20;
			nodeStyle.padding.right = 20;
			nodeStyle.padding.top = 5;
			nodeStyle.padding.bottom = 5;
			nodeStyle.contentOffset = Vector2.zero;
			nodeStyle.alignment = TextAnchor.MiddleCenter;
			nodeStyle.wordWrap = true;
		}

		protected virtual void OnInspectorUpdate()
		{
			// Ensure the Sequence Inspector is always showing the currently selected sequence
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			if (fungusScript.selectedSequence != null)
			{
			    if (sequenceInspector == null)
				{
					ShowSequenceInspector(fungusScript);
				}
				sequenceInspector.sequence = fungusScript.selectedSequence;
			}

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
				bool isSelected = (fungusScript.selectedSequence == deleteSequence);

				foreach (Command command in deleteSequence.commandList)
				{
					Undo.DestroyObjectImmediate(command);
				}
				
				Undo.DestroyObjectImmediate(deleteSequence);
				fungusScript.ClearSelectedCommands();

				if (isSelected)
				{
					// Revert to showing properties for the Fungus Script
					Selection.activeGameObject = fungusScript.gameObject;
				}
			}
			deleteList.Clear();

			DrawScriptView(fungusScript);
			DrawOverlay(fungusScript);

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

			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Label(fungusScript.name, EditorStyles.whiteBoldLabel);
			if (fungusScript.description.Length > 0)
			{
				GUILayout.Label(fungusScript.description, EditorStyles.whiteLargeLabel);
			}
			GUILayout.EndVertical();

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
				fungusScript.selectedSequence = null;
				if (!EditorGUI.actionKey)
				{
					fungusScript.ClearSelectedCommands();
				}
				Selection.activeGameObject = fungusScript.gameObject;
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

				float nodeWidthA = nodeStyle.CalcSize(new GUIContent(sequence.sequenceName)).x + 10;
				float nodeWidthB = 0f;
				if (sequence.eventHandler != null)
				{
					nodeWidthB = nodeStyle.CalcSize(new GUIContent(sequence.eventHandler.GetSummary())).x + 10;
				}

				sequence.nodeRect.width = Mathf.Max(Mathf.Max(nodeWidthA, nodeWidthB), 120);
				sequence.nodeRect.height = 40;

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

			// Draw play icons beside all executing sequences
			if (Application.isPlaying)
			{
				foreach (Sequence s in sequences)
				{
					if (s.IsExecuting())
					{
						s.executingIconTimer = playIconFadeTime;
					}

					if (s.executingIconTimer > 0f)
					{
						s.executingIconTimer = Mathf.Max(s.executingIconTimer - Time.deltaTime, 0f);

						Rect rect = new Rect(s.nodeRect);

						rect.x += fungusScript.scrollPos.x - 37;
						rect.y += fungusScript.scrollPos.y + 3;
						rect.width = 34;
						rect.height = 34;

						if (!s.IsExecuting() && s.executingIconTimer < playIconFadeTime)
						{
							float alpha = s.executingIconTimer / playIconFadeTime;
							GUI.color = new Color(1f, 1f, 1f, alpha); 
						}

						if (GUI.Button(rect, FungusEditorResources.texPlayBig as Texture, new GUIStyle()))
						{
							SelectSequence(fungusScript, s);
						}

						GUI.color = Color.white;
					}
				}
			}

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
			
			float y = (fungusScript.scrollPos.y % gridSize);
			while (y < height)
			{
				if (y >= 0)
				{
					GLDraw.DrawLine(new Vector2(0, y), new Vector2(width, y), color, 1f);
				}
				y += gridSize;
			}
		}

		protected virtual void SelectSequence(FungusScript fungusScript, Sequence sequence)
		{
			// Select the sequence and also select currently executing command
			ShowSequenceInspector(fungusScript);
			fungusScript.selectedSequence = sequence;
			fungusScript.ClearSelectedCommands();
			if (sequence.activeCommand != null)
			{
				fungusScript.AddSelectedCommand(sequence.activeCommand);
			}
		}
		
		public static Sequence CreateSequence(FungusScript fungusScript, Vector2 position)
		{
			Sequence newSequence = fungusScript.CreateSequence(position);
			Undo.RegisterCreatedObjectUndo(newSequence, "New Sequence");
			ShowSequenceInspector(fungusScript);
			fungusScript.selectedSequence = newSequence;
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
				if (Event.current.button == 0)
				{
					dragWindowId = windowId;
					startDragPosition.x = sequence.nodeRect.x;
					startDragPosition.y = sequence.nodeRect.y;
				}

				if (windowId < windowSequenceMap.Count)
				{
					Undo.RecordObject(fungusScript, "Select");

					SelectSequence(fungusScript, sequence);

					GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
				}
			}

			bool selected = (fungusScript.selectedSequence == sequence);

			GUIStyle nodeStyleCopy = new GUIStyle(nodeStyle);

			if (sequence.eventHandler != null)
			{
				nodeStyleCopy.normal.background = selected ? FungusEditorResources.texEventNodeOn : FungusEditorResources.texEventNodeOff;
			}
			else
			{
				// Count the number of unique connections (excluding self references)
				List<Sequence> uniqueList = new List<Sequence>();
				List<Sequence> connectedSequences = sequence.GetConnectedSequences();
				foreach (Sequence connectedSequence in connectedSequences)
				{
					if (connectedSequence == sequence ||
					    uniqueList.Contains(connectedSequence))
					{
						continue;
					}
					uniqueList.Add(connectedSequence);
				}

				if (uniqueList.Count > 1)
				{
					nodeStyleCopy.normal.background = selected ? FungusEditorResources.texChoiceNodeOn : FungusEditorResources.texChoiceNodeOff;
				}
				else
				{
					nodeStyleCopy.normal.background = selected ? FungusEditorResources.texProcessNodeOn : FungusEditorResources.texProcessNodeOff;
				}
			}

			// Show event handler name, or a custom summary if one is provided
			string nodeName = "";
			if (sequence.eventHandler != null)
			{
				string handlerSummary = sequence.eventHandler.GetSummary();
				nodeName = "(" + handlerSummary + ")\n";
			}
			nodeName += sequence.sequenceName;

			GUILayout.Box(nodeName, nodeStyleCopy, GUILayout.Width(sequence.nodeRect.width), GUILayout.Height(sequence.nodeRect.height));
			if (sequence.description.Length > 0)
			{
				GUILayout.Label(sequence.description, EditorStyles.whiteLabel);
			}

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
					    sequence == sequenceB ||
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
				new Vector2(rectA.xMin + 5, rectA.center.y),
				new Vector2(rectA.xMin + rectA.width / 2, rectA.yMin + 2),
				new Vector2(rectA.xMin + rectA.width / 2, rectA.yMax - 2),
				new Vector2(rectA.xMax - 5, rectA.center.y) 
			};

			Vector2[] pointsB = new Vector2[] {
				new Vector2(rectB.xMin + 5, rectB.center.y),
				new Vector2(rectB.xMin + rectB.width / 2, rectB.yMin + 2),
				new Vector2(rectB.xMin + rectB.width / 2, rectB.yMax - 2),
				new Vector2(rectB.xMax - 5, rectB.center.y)
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
			newSequence.sequenceName = fungusScript.GetUniqueSequenceKey(oldSequence.sequenceName + " (Copy)");

			Undo.RecordObject(newSequence, "Duplicate Sequence");

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

			if (oldSequence.eventHandler != null)
			{
				EventHandler eventHandler = oldSequence.eventHandler;
				System.Type type = eventHandler.GetType();
				EventHandler newEventHandler = Undo.AddComponent(fungusScript.gameObject, type) as EventHandler;
				System.Reflection.FieldInfo[] fields = type.GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(newEventHandler, field.GetValue(eventHandler));
				}
				newEventHandler.parentSequence = newSequence;
				newSequence.eventHandler = newEventHandler;
			}
		}

		protected static void ShowSequenceInspector(FungusScript fungusScript)
		{
			if (sequenceInspector == null)
			{
				// Create a Scriptable Object with a custom editor which we can use to inspect the selected sequence.
				// Editors for Scriptable Objects display using the full height of the inspector window.
				sequenceInspector = ScriptableObject.CreateInstance<SequenceInspector>() as SequenceInspector;
				sequenceInspector.hideFlags = HideFlags.DontSave;
			}

			Selection.activeObject = sequenceInspector;

			EditorUtility.SetDirty(sequenceInspector);
		}
	}
}