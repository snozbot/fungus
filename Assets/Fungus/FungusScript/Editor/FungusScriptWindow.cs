using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	public class FungusScriptWindow : EditorWindow
	{
		float commandViewWidth;
		bool resize = false;
		Rect cursorChangeRect;
		public const float minViewWidth = 300;

		private List<Sequence> windowSequenceMap = new List<Sequence>();

	    [MenuItem("Window/Fungus Script")]
	    static void Init()
	    {
	        GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
	    }

		void OnEnable()
		{
			commandViewWidth = minViewWidth;
			cursorChangeRect = new Rect(this.position.width - commandViewWidth, 0, 4f, this.position.height);
		}

		public void OnInspectorUpdate()
		{
			Repaint();
		}

		static public FungusScript GetFungusScript()
		{
			if (Selection.activeGameObject != null)
			{
				return Selection.activeGameObject.GetComponent<FungusScript>();
			}

			return null;
		}

		void OnGUI()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				GUILayout.Label("No Fungus Script object selected");
				return;
			}

			GUILayout.BeginHorizontal();
			DrawScriptView(fungusScript);
			ResizeViews();
			DrawCommandView(fungusScript);
			GUILayout.EndHorizontal();
		}
		
		void DrawScriptView(FungusScript fungusScript)
		{
			EditorUtility.SetDirty(fungusScript);
			
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
			
			// Calc rect for left hand script view
			Rect scriptViewRect = new Rect(0, 0, this.position.width - commandViewWidth, this.position.height);

			// Clip GL drawing so not to overlap scrollbars
			Rect clipRect = new Rect(fungusScript.scriptScrollPos.x + scrollViewRect.x,
			                         fungusScript.scriptScrollPos.y + scrollViewRect.y,
			                         scriptViewRect.width - 15,
			                         scriptViewRect.height - 15);

			GUILayoutUtility.GetRect(scriptViewRect.width, scriptViewRect.height);

			fungusScript.scriptScrollPos = GLDraw.BeginScrollView(scriptViewRect, fungusScript.scriptScrollPos, scrollViewRect, clipRect);
			
			if (Event.current.type == EventType.ContextClick)
			{
				GenericMenu menu = new GenericMenu();
				Vector2 mousePos = Event.current.mousePosition;
				mousePos += fungusScript.scriptScrollPos;
				menu.AddItem (new GUIContent ("Create Sequence"), false, CreateSequenceCallback, mousePos);
				menu.ShowAsContext ();
				
				Event.current.Use();
			}
			
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
			
			GLDraw.EndScrollView();
		}
		
		void ResizeViews()
		{
			cursorChangeRect.x = this.position.width - commandViewWidth;
			cursorChangeRect.height = this.position.height;
			
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
				commandViewWidth = this.position.width - Event.current.mousePosition.x;
				commandViewWidth = Mathf.Max(minViewWidth, commandViewWidth);
				commandViewWidth = Mathf.Min(this.position.width - minViewWidth, commandViewWidth);
			}
			if(Event.current.type == EventType.MouseUp)
			{
				resize = false;        
			}
		}
		
		void DrawCommandView(FungusScript fungusScript)
		{
			GUILayout.Space(5);

			fungusScript.commandScrollPos = GUILayout.BeginScrollView(fungusScript.commandScrollPos);

			EditorGUILayout.BeginVertical();

			EditorGUILayout.Separator();
			GUI.backgroundColor = Color.yellow;
			GUILayout.Box("Sequence", GUILayout.ExpandWidth(true));
			GUI.backgroundColor = Color.white;
			
			GUILayout.BeginHorizontal();
			
			if (fungusScript.selectedSequence == null)
			{
				GUILayout.FlexibleSpace();
			}
			
			if (GUILayout.Button(fungusScript.selectedSequence == null ? "Create Sequence" : "Create", 
			                     fungusScript.selectedSequence == null ?  EditorStyles.miniButton : EditorStyles.miniButtonLeft))
			{
				Sequence newSequence = fungusScript.CreateSequence(fungusScript.scriptScrollPos);
				Undo.RegisterCreatedObjectUndo(newSequence, "New Sequence");
				fungusScript.selectedSequence = newSequence;
			}
			
			if (fungusScript.selectedSequence == null)
			{
				GUILayout.FlexibleSpace();
			}
			
			if (fungusScript.selectedSequence != null)
			{
				if (GUILayout.Button("Delete", EditorStyles.miniButtonMid))
				{
					Undo.DestroyObjectImmediate(fungusScript.selectedSequence.gameObject);
					fungusScript.selectedSequence = null;
				}
				if (GUILayout.Button("Duplicate", EditorStyles.miniButtonRight))
				{
					GameObject copy = GameObject.Instantiate(fungusScript.selectedSequence.gameObject) as GameObject;
					copy.transform.parent = fungusScript.transform;
					copy.transform.hideFlags = HideFlags.HideInHierarchy;
					copy.name = fungusScript.selectedSequence.name;
					
					Sequence sequenceCopy = copy.GetComponent<Sequence>();
					sequenceCopy.nodeRect.x += sequenceCopy.nodeRect.width + 10;
					
					Undo.RegisterCreatedObjectUndo(copy, "Duplicate Sequence");
					fungusScript.selectedSequence = sequenceCopy;
				}
			}
			
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();

			FungusScriptEditor editor = Editor.CreateEditor(fungusScript) as FungusScriptEditor;
			editor.DrawSequenceGUI(fungusScript);

			GUILayout.FlexibleSpace();

			EditorGUILayout.EndVertical();

			GUILayout.EndScrollView();
		}

		void CreateSequenceCallback(object item)
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript != null)
			{
				Vector2 position = (Vector2)item;
				position -= fungusScript.scriptScrollPos;
				Sequence newSequence = fungusScript.CreateSequence(position);
				Undo.RegisterCreatedObjectUndo(newSequence, "New Sequence");
				fungusScript.selectedSequence = newSequence;
			}				
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

						Selection.activeGameObject = fungusScript.gameObject;
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