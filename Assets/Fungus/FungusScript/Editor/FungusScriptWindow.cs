using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public class FungusScriptWindow : EditorWindow
	{
		bool resize = false;
		Rect cursorChangeRect;
		public const float minViewWidth = 300;

		static bool locked = false;
		static GUIStyle lockButtonStyle;
		static FungusScript activeFungusScript;

		private List<Sequence> windowSequenceMap = new List<Sequence>();

	    [MenuItem("Window/Fungus Script")]
	    static void Init()
	    {
	        GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
	    }

		[MenuItem("GameObject/Fungus/Fungus Script")]
		static void CreateFungusScript()
		{
			GameObject newFungusScriptGO = new GameObject();
			newFungusScriptGO.name = "FungusScript";
			FungusScript fungusScript = newFungusScriptGO.AddComponent<FungusScript>();
			GameObject newSequenceGO = new GameObject();
			newSequenceGO.transform.parent = newFungusScriptGO.transform;
			newSequenceGO.name = "Start";
			newSequenceGO.hideFlags = HideFlags.HideInHierarchy;
			Sequence sequence = newSequenceGO.AddComponent<Sequence>();
			fungusScript.startSequence = sequence;
		}
		
		// Implementing this method causes the padlock image to display on the window
		// https://leahayes.wordpress.com/2013/04/30/adding-the-little-padlock-button-to-your-editorwindow/#more-455
		void ShowButton(Rect position) {
			if (lockButtonStyle == null)
			{
				lockButtonStyle = "IN LockButton";
			}
			locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
		}

		public void OnInspectorUpdate()
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

		void OnGUI()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				GUILayout.Label("No Fungus Script scene object selected");
				return;
			}

			if (PrefabUtility.GetPrefabType(fungusScript) == PrefabType.Prefab)
			{
				GUILayout.Label("No Fungus Script scene object selected (selected object is a prefab)");
				return;
			}

			GUILayout.BeginHorizontal();
			DrawScriptView(fungusScript);
			ResizeViews(fungusScript);
			DrawSequenceView(fungusScript);
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
			Rect scriptViewRect = new Rect(0, 0, this.position.width - fungusScript.commandViewWidth, this.position.height);

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
			
			BeginWindows();
			
			GUIStyle windowStyle = new GUIStyle(EditorStyles.toolbarButton);
			windowStyle.stretchHeight = true;
			windowStyle.fixedHeight = 40;

			windowSequenceMap.Clear();
			for (int i = 0; i < sequences.Length; ++i)
			{
				Sequence sequence = sequences[i];

				float titleWidth = windowStyle.CalcSize(new GUIContent(sequence.name)).x;
				float windowWidth = Mathf.Max (titleWidth + 10, 100);

				if (fungusScript.selectedSequence == sequence ||
				    fungusScript.executingSequence == sequence)
				{
					GUI.backgroundColor = Color.green;
				}
					
				sequence.nodeRect = GUILayout.Window(i, sequence.nodeRect, DrawWindow, "", windowStyle, GUILayout.Width(windowWidth), GUILayout.Height(20), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

				GUI.backgroundColor = Color.white;

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

			GLDraw.EndScrollView();
		}
		
		void ResizeViews(FungusScript fungusScript)
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
		
		void DrawSequenceView(FungusScript fungusScript)
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
				Sequence newSequence = fungusScript.CreateSequence(fungusScript.scriptScrollPos);
				Undo.RegisterCreatedObjectUndo(newSequence, "New Sequence");
				fungusScript.selectedSequence = newSequence;
				fungusScript.selectedCommand = null;
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
					fungusScript.selectedCommand = null;
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
					fungusScript.selectedCommand = null;
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
							fungusScript.selectedSequence = s;
							fungusScript.selectedCommand = null;
							Selection.activeGameObject = fungusScript.gameObject;
							GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
						}
					}
				}
			}

			Sequence sequence = windowSequenceMap[windowId];

			GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.alignment = TextAnchor.MiddleCenter;

			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Label(sequence.name, labelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

	        GUI.DragWindow();
	    }

		void DrawConnections(FungusScript fungusScript, Sequence sequence, bool highlightedOnly)
		{
			if (sequence == null)
			{
				return;
			}

			List<Sequence> connectedSequences = new List<Sequence>();

			bool sequenceIsSelected = (fungusScript.selectedSequence == sequence);

			foreach (Command command in sequence.commandList)
			{
				bool commandIsSelected = (fungusScript.selectedCommand == command);

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