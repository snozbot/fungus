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

		// Set this flag to tell the context menu to appear.
		// The context menu is modal, so we need to defer displaying it if the background needs to be repainted
		public static bool showContextMenu;

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

			DrawScriptView(fungusScript);
			DrawControls(fungusScript);

			if (Event.current.type == EventType.Repaint &&
				showContextMenu)
			{
				ShowContextMenu();
				showContextMenu = false;
			}
		}

		protected virtual void DrawControls(FungusScript fungusScript)
		{
			GUILayout.Space(8);
			
			GUILayout.BeginHorizontal();
			
			GUILayout.Space(8);
			
			if (GUILayout.Button("", new GUIStyle("OL Plus")))
			{
				Vector2 newNodePosition = new Vector2(50 - fungusScript.scrollPos.x, 
				                                      50 - fungusScript.scrollPos.y);
				CreateSequence(fungusScript, newNodePosition);
			}
			
			GUILayout.FlexibleSpace();
			
			float minValue = 0.6f;
			float maxValue = 1f;
			float range = maxValue - minValue;
			
			fungusScript.zoom = GUILayout.HorizontalSlider(fungusScript.zoom, minValue, maxValue, GUILayout.Width(100));
			if (fungusScript.zoom < minValue + range * 0.25f)
			{
				fungusScript.zoom = minValue;
			}
			else if (fungusScript.zoom > minValue + range * 0.75f)
			{
				fungusScript.zoom = maxValue;
			}
			else
			{
				fungusScript.zoom = minValue + (range * 0.5f);
			}
			
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
				fungusScript.selectedSequence = null;
				if (!EditorGUI.actionKey)
				{
					fungusScript.ClearSelectedCommands();
				}
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

				sequence.nodeRect.height = CalcRectHeight(sequence.commandList.Count);

				Rect windowRect = new Rect(sequence.nodeRect);
				windowRect.x += fungusScript.scrollPos.x;
				windowRect.y += fungusScript.scrollPos.y;
				windowRect.width = 240;

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
				fungusScript.scrollPos -= Event.current.delta * 4f;
			}

			GLDraw.EndGroup();

			EditorZoomArea.End();
		}

		protected virtual void DrawGrid(FungusScript fungusScript)
		{
			float width = this.position.width / fungusScript.zoom;
			float height = this.position.height / fungusScript.zoom;
			
			// Test Unity Pro dark skin
			bool testProSkin = false;
			
			if (testProSkin)
			{
				GUI.color = new Color32(56, 56, 56, 255); 
				GUI.DrawTexture( new Rect(0,0, width, height), EditorGUIUtility.whiteTexture );
				GUI.color = Color.white;
			}
			
			Color color = new Color32(180, 180, 180, 255);
			if (testProSkin || EditorGUIUtility.isProSkin)
			{
				color = new Color32(64, 64, 64, 255);
			}
			
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
			fungusScript.ClearSelectedCommands();
			newSequence.nodeRect.width = 240;

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
			fungusScript.ClearSelectedCommands();
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
			    (Event.current.button == 0 || Event.current.button == 1) && 
		    	(Event.current.type == EventType.MouseDown))
			{
				// Check if might be start of a window drag
				if (Event.current.button == 0 &&
				    Event.current.mousePosition.y < 26)
				{
					dragging = true;
					startDragPosition.x = sequence.nodeRect.x;
					startDragPosition.y = sequence.nodeRect.y;
				}

				if (windowId < windowSequenceMap.Count)
				{
					Undo.RecordObject(fungusScript, "Select");
					if (sequence != selectedSequence || !EditorGUI.actionKey)
					{
						int commandIndex = CalcCommandIndex(Event.current.mousePosition.y);
						if (commandIndex < sequence.commandList.Count &&
						    fungusScript.selectedCommands.Contains(sequence.commandList[commandIndex]))
						{
							// Right clicking on an already selected command does not clear the selected list
						}
						else
						{
							fungusScript.ClearSelectedCommands();
						}
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
					startRect.y += CalcRectHeight(sequence.commandList.Count);
					startRect.height = 0;
					startRect.x += fungusScript.scrollPos.x;
					startRect.y += fungusScript.scrollPos.y;

					Rect endRect = new Rect(sequenceB.nodeRect);
					endRect.height = 22;
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

		protected virtual int CalcCommandIndex(float mouseY)
		{
			return (int)(mouseY - 34 + 7) / 20;
		}

		public static void ShowContextMenu()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			bool showCut = false;
			bool showCopy = false;
			bool showDelete = false;
			bool showPaste = false;
			
			if (fungusScript.selectedCommands.Count > 0)
			{
				showCut = true;
				showCopy = true;
				showDelete = true;
			}
			
			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
			
			if (commandCopyBuffer.HasCommands())
			{
				showPaste = true;
			}
			
			GenericMenu commandMenu = new GenericMenu();
			
			if (showCut)
			{
				commandMenu.AddItem (new GUIContent ("Cut"), false, Cut);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Cut"));
			}
			
			if (showCopy)
			{
				commandMenu.AddItem (new GUIContent ("Copy"), false, Copy);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Copy"));
			}
			
			if (showPaste)
			{
				commandMenu.AddItem (new GUIContent ("Paste"), false, Paste);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Paste"));
			}
			
			if (showDelete)
			{
				commandMenu.AddItem (new GUIContent ("Delete"), false, Delete);
			}
			else
			{
				commandMenu.AddDisabledItem(new GUIContent ("Delete"));
			}
			
			commandMenu.AddSeparator("");
			
			commandMenu.AddItem (new GUIContent ("Select All"), false, SelectAll);
			commandMenu.AddItem (new GUIContent ("Select None"), false, SelectNone);
			
			commandMenu.AddSeparator("");
			
			commandMenu.AddItem (new GUIContent ("Delete Sequence"), false, DeleteSequence);
			commandMenu.AddItem (new GUIContent ("Duplicate Sequence"), false, DuplicateSequence);
			
			commandMenu.ShowAsContext();
		}
		
		protected static void SelectAll()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}

			fungusScript.ClearSelectedCommands();
			Undo.RecordObject(fungusScript, "Select All");
			foreach (Command command in fungusScript.selectedSequence.commandList)
			{
				fungusScript.selectedCommands.Add(command);
			}
		}
		
		protected static void SelectNone()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}

			Undo.RecordObject(fungusScript, "Select None");
			fungusScript.ClearSelectedCommands();
		}
		
		protected static void Cut()
		{
			Copy();
			Delete();
		}
		
		protected static void Copy()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}

			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();
			commandCopyBuffer.Clear();
			
			foreach (Command command in fungusScript.selectedCommands)
			{
				System.Type type = command.GetType();
				Command newCommand = Undo.AddComponent(commandCopyBuffer.gameObject, type) as Command;
				System.Reflection.FieldInfo[] fields = type.GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(newCommand, field.GetValue(command));
				}
			}
		}
		
		protected static void Paste()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}

			CommandCopyBuffer commandCopyBuffer = CommandCopyBuffer.GetInstance();

			// Find where to paste commands in sequence (either at end or after last selected command)
			int pasteIndex = fungusScript.selectedSequence.commandList.Count;
			if (fungusScript.selectedCommands.Count > 0)
			{
				for (int i = 0; i < fungusScript.selectedSequence.commandList.Count; ++i)
				{
					Command command = fungusScript.selectedSequence.commandList[i];
					
					foreach (Command selectedCommand in fungusScript.selectedCommands)
					{
						if (command == selectedCommand)
						{
							pasteIndex = i + 1;
						}
					}
				}
			}
			
			foreach (Command command in commandCopyBuffer.GetCommands())
			{
				System.Type type = command.GetType();
				Command newCommand = Undo.AddComponent(fungusScript.selectedSequence.gameObject, type) as Command;
				System.Reflection.FieldInfo[] fields = type.GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(newCommand, field.GetValue(command));
				}
				
				Undo.RecordObject(fungusScript.selectedSequence, "Paste");
				fungusScript.selectedSequence.commandList.Insert(pasteIndex++, newCommand);
			}
		}
		
		protected static void Delete()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}

			for (int i = fungusScript.selectedSequence.commandList.Count - 1; i >= 0; --i)
			{
				Command command = fungusScript.selectedSequence.commandList[i];
				foreach (Command selectedCommand in fungusScript.selectedCommands)
				{
					if (command == selectedCommand)
					{
						Undo.RecordObject(fungusScript.selectedSequence, "Delete");
						fungusScript.selectedSequence.commandList.RemoveAt(i);
						Undo.DestroyObjectImmediate(command);
						
						break;
					}
				}
			}
			
			Undo.RecordObject(fungusScript, "Delete");
			fungusScript.ClearSelectedCommands();
			fungusScript.selectedSequence = null;
		}
		
		public static void DeleteSequence()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}

			FungusScriptWindow.deleteList.Add(fungusScript.selectedSequence);
		}
		
		protected static void DuplicateSequence()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null ||
			    fungusScript.selectedSequence == null)
			{
				return;
			}

			Vector2 newPosition = new Vector2(fungusScript.selectedSequence.nodeRect.position.x + 
			                                  fungusScript.selectedSequence.nodeRect.width + 20, 
			                                  fungusScript.selectedSequence.nodeRect.y);

			Sequence oldSequence = fungusScript.selectedSequence;

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

	}
}