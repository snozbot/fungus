using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

[CanEditMultipleObjects]
[CustomEditor (typeof(View))]
public class ViewEditor : Editor 
{
	void OnSceneGUI () 
	{
		View t = target as View;
		if (t.enabled)
		{
			EditViewBounds();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
	
	void EditViewBounds()
	{
		View t = target as View;

		DrawView(t);

		Vector3 pos = t.transform.position;
		float viewSize = t.viewSize;

		Vector3 newViewPos = Handles.PositionHandle(pos, Quaternion.identity);

		t.transform.position = newViewPos;

		Vector3[] handles = new Vector3[2];
		handles[0] = pos + new Vector3(0, -viewSize, 0);
		handles[1] = pos + new Vector3(0, viewSize, 0);

		for (int i = 0; i < 2; ++i)
		{
			Vector3 newPos = Handles.FreeMoveHandle(handles[i],
			                                        Quaternion.identity,
			                                        HandleUtility.GetHandleSize(pos) * 0.1f,
			                                        Vector3.zero,
			                                        Handles.CubeCap);
			if (newPos != handles[i])
			{
				t.viewSize = Mathf.Abs(newPos.y - pos.y);
				break;
			}
		}
	}

	public static void DrawView(View view)
	{	
		Vector3 pos = view.transform.position;
		float viewSize = view.viewSize;
		
		// Draw 2:1 aspect ratio box
		{
			float height = viewSize;
			float width = height * (2f / 1f);
			
			Vector3[] verts = new Vector3[4];
			verts[0] = pos + new Vector3(-width, -height, 0);
			verts[1] = pos + new Vector3(-width, height, 0);
			verts[2] = pos + new Vector3(width, height, 0);
			verts[3] = pos + new Vector3(width, -height, 0);
			
			Handles.DrawSolidRectangleWithOutline(verts, new Color(1,1,1,0f), new Color(0,1,0,0.25f) );
		}
		
		// Draw 4:3 aspect ratio box
		{
			float height = viewSize;
			float width = height * (4f / 3f);
			
			Vector3[] verts = new Vector3[4];
			verts[0] = pos + new Vector3(-width, -height, 0);
			verts[1] = pos + new Vector3(-width, height, 0);
			verts[2] = pos + new Vector3(width, height, 0);
			verts[3] = pos + new Vector3(width, -height, 0);
			
			Handles.DrawSolidRectangleWithOutline(verts, new Color(1,1,1,0f), new Color(0,1,0,1) );
		}
	}
}
