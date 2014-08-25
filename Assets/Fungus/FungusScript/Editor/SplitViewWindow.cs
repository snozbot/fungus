using UnityEngine;
using UnityEditor;

public class SplitViewWindow : EditorWindow
{
	private Vector2 scrollPos = Vector2.zero;
	float commandViewWidth;
	bool resize = false;
	Rect cursorChangeRect;

	public float minViewWidth = 150;
	
	[MenuItem("MyWindows/SplitView")]
	public static void Init(){
		GetWindow<SplitViewWindow>();
	}
	
	void OnEnable()
	{
		commandViewWidth = minViewWidth;
		cursorChangeRect = new Rect(this.position.width - commandViewWidth, 0, 4f, this.position.height);
	}
	
	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		DrawScriptView();
		ResizeViews();
		GUILayout.EndHorizontal();

		Repaint();
	}
	
	void DrawScriptView()
	{
		Rect scriptViewRect = new Rect(0, 0, this.position.width - commandViewWidth, this.position.height);
		
		scrollPos = GUI.BeginScrollView(scriptViewRect, scrollPos, scriptViewRect);

		GUI.EndScrollView();		
	}

	void ResizeViews()
	{
		cursorChangeRect.x = this.position.width - commandViewWidth;
		cursorChangeRect.height = this.position.height;

		GUI.color = Color.grey;
		GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
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

	void DrawCommandView()
	{
	}
}