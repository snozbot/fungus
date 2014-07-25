using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class FungusEditorWindow : EditorWindow
{
	private List<Sequence> windowSequenceMap = new List<Sequence>();
    private Vector2 scrollPos;                         // ScrollViews use a Vector2 to track the state of each scroll bar

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

	SequenceController GetSequenceController()
	{
		GameObject activeObject = Selection.activeGameObject;

		while (activeObject != null)
		{
			SequenceController sequenceController = activeObject.GetComponent<SequenceController>();
			Sequence sequence = activeObject.GetComponent<Sequence>();

			if (sequenceController != null)
			{
				// Found sequence controller
				return sequenceController;
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
		SequenceController sequenceController = GetSequenceController();
		if (sequenceController == null)
		{
			return;
		}

		string labelText = sequenceController.name;
		if (Application.isPlaying)
		{
			if (sequenceController.activeSequence == null)
			{
				labelText += ": Idle";
			}
			else
			{
				labelText += ": Active";
			}
		}

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(labelText);
		GUILayout.Space(30);
		GUILayout.EndHorizontal();

        // BeginScrollView lets you specify a region that 'looks into' a much larger area. In this case, we create a canvas
        // 1000px X 1000px in size that's constrained to the same region as the EditorWindow. If the scrollbars are modified,
        // the new values are stored in the Vector2 that's returned.
        // http://docs.unity3d.com/Documentation/ScriptReference/GUI.BeginScrollView.html
        scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, new Rect(0, 0, 1000, 1000));

        // In games, GUI.Window pops up a window on your screen. In the Editor, GUI.Window shows a sub-window inside an EditorWindow.
        // All calls to GUI.Window need to be wrapped in a BeginWindows / EndWindows pair.
        // http://docs.unity3d.com/Documentation/ScriptReference/EditorWindow.BeginWindows.html
        BeginWindows();

		Sequence[] sequences = sequenceController.GetComponentsInChildren<Sequence>();

		windowSequenceMap.Clear();
		for (int i = 0; i < sequences.Length; ++i)
		{
			Sequence sequence = sequences[i];
			sequence.nodeRect = GUI.Window(i, sequence.nodeRect, DrawWindow, sequence.name);
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
				GUI.backgroundColor = Color.yellow;
			}
			else
			{
				GUI.backgroundColor = Color.white;
			}

			if (GUILayout.Button(commandName, style, GUILayout.ExpandWidth(true)))
			{
				// Highlight the command in inspector
				FungusCommandEditor.selectedCommand = command;
				EditorUtility.SetDirty( command );
			}
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
					color = Color.yellow;
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
