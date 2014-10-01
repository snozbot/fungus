using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public class FungusScriptWindow : EditorWindow
	{
		protected Vector2 newNodePosition = new Vector2();

		static bool locked = false;
		static GUIStyle lockButtonStyle;
		static FungusScript activeFungusScript;

		protected List<Sequence> windowSequenceMap = new List<Sequence>();

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

			GUILayout.BeginHorizontal();
			DrawScriptView(fungusScript);
			GUILayout.EndHorizontal();
		}
		
		protected virtual void DrawScriptView(FungusScript fungusScript)
		{
			EditorUtility.SetDirty(fungusScript);
			
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>(true);
			
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
			
			// Calc rect for left hand script view
			Rect scriptViewRect = new Rect(0, 0, this.position.width, this.position.height);

			// Clip GL drawing so not to overlap scrollbars
			Rect clipRect = new Rect(fungusScript.scriptScrollPos.x + scrollViewRect.x,
			                         fungusScript.scriptScrollPos.y + scrollViewRect.y,
			                         scriptViewRect.width - 15,
			                         scriptViewRect.height - 15);

			GUILayoutUtility.GetRect(scriptViewRect.width, scriptViewRect.height);

			fungusScript.scriptScrollPos = GLDraw.BeginScrollView(scriptViewRect, fungusScript.scriptScrollPos, scrollViewRect, clipRect);
			
			if (Event.current.type == EventType.ContextClick &&
			    clipRect.Contains(Event.current.mousePosition))
			{
				GenericMenu menu = new GenericMenu();
				Vector2 mousePos = Event.current.mousePosition;
				mousePos += fungusScript.scriptScrollPos;
				menu.AddItem (new GUIContent ("Create Sequence"), false, CreateSequenceCallback, mousePos);
				menu.ShowAsContext ();
				
				Event.current.Use();
			}

			// Calculate center of script view for positioning new nodes
			newNodePosition.x = scrollViewRect.xMin + fungusScript.scriptScrollPos.x + scriptViewRect.width / 2;
			newNodePosition.y = scrollViewRect.yMin + fungusScript.scriptScrollPos.y + scriptViewRect.height / 2;

			BeginWindows();

			if (Event.current.button == 0 && 
				Event.current.type == EventType.MouseDown)
			{
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

			windowSequenceMap.Clear();
			for (int i = 0; i < sequences.Length; ++i)
			{
				Sequence sequence = sequences[i];

				// Hack to support legacy design where sequences were child gameobjects (will be removed soon)
				sequence.UpdateSequenceName();

				if (fungusScript.selectedSequence == sequence ||
				    fungusScript.executingSequence == sequence)
				{
					GUI.backgroundColor = Color.green;

					Rect highlightRect = new Rect(sequence.nodeRect);
					highlightRect.y -= 1;
					highlightRect.height += 5;
					GUI.Box(highlightRect, "");
					GUI.backgroundColor = Color.white;
				}

				sequence.nodeRect.width = 240;
				sequence.nodeRect.height = 20;

				sequence.nodeRect = GUILayout.Window(i, sequence.nodeRect, DrawWindow, "", windowStyle);

				GUI.backgroundColor = Color.white;

				windowSequenceMap.Add(sequence);
			}

			EndWindows();

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

		protected virtual Sequence CreateSequence(FungusScript fungusScript, Vector2 position)
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

		protected virtual void DuplicateSequence(FungusScript fungusScript, Sequence sequence)
		{
			if (sequence == null)
			{
				return;
			}

			Vector2 newPosition = new Vector2(sequence.nodeRect.position.x + sequence.nodeRect.width + 20, sequence.nodeRect.y);
			Sequence newSequence = CreateSequence(fungusScript, newPosition);
			newSequence.sequenceName = sequence.sequenceName + " (Copy)";

			foreach (Command command in sequence.commandList)
			{
				System.Type type = command.GetType();
				Command newCommand = Undo.AddComponent(fungusScript.gameObject, type) as Command;
				System.Reflection.FieldInfo[] fields = type.GetFields();
				foreach (System.Reflection.FieldInfo field in fields)
				{
					field.SetValue(newCommand, field.GetValue(command));
				}
				newCommand.selected = false;
				newSequence.commandList.Add(newCommand);
			}
		}

		protected virtual void CreateSequenceCallback(object item)
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript != null)
			{
				Vector2 position = (Vector2)item;
				position -= fungusScript.scriptScrollPos;
				CreateSequence(fungusScript, position);
			}				
		}

		protected virtual void DrawWindow(int windowId)
		{
			// Select sequence when node is clicked
			if (!Application.isPlaying &&
			    Event.current.button == 0 && 
		    	Event.current.type == EventType.MouseDown) 
			{
				if (windowId < windowSequenceMap.Count)
				{
					Sequence s = windowSequenceMap[windowId];
					if (s != null)
					{
						FungusScript fungusScript = s.GetFungusScript();
						if (fungusScript != null)
						{
							if (s != fungusScript.selectedSequence || !EditorGUI.actionKey)
							{
								fungusScript.selectedCommands.Clear();
							}

							fungusScript.selectedSequence = s;
							GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
						}
					}
				}
			}

			Sequence sequence = windowSequenceMap[windowId];

			GUILayout.BeginVertical();

			SequenceEditor sequenceEditor = Editor.CreateEditor(sequence) as SequenceEditor;
			sequenceEditor.DrawCommandListGUI(sequence.GetFungusScript());
			DestroyImmediate(sequenceEditor);

			GUILayout.EndVertical();

	        GUI.DragWindow();
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

					DrawRectConnection(sequence.nodeRect, sequenceB.nodeRect, highlight);
				}
			}
		}

		protected virtual void DrawRectConnection(Rect rectA, Rect rectB, bool highlight)
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
			
			GLDraw.DrawConnectingCurve(pointA, pointB, color, 2f);
		}
	}

}