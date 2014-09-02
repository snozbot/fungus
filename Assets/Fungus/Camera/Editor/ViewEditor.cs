using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

[CanEditMultipleObjects]
[CustomEditor (typeof(View))]
public class ViewEditor : Editor 
{
	// Draw Views when they're not selected
	[DrawGizmo(GizmoType.NotSelected | GizmoType.SelectedOrChild)]
	static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
	{
		View view = objectTransform.gameObject.GetComponent<View>();
		if (view != null)
		{
			DrawView(view);
		}
	}

	Vector2 LookupAspectRatio(int index)
	{
		switch (index)
		{
		default:
		case 1:
			return new Vector2(4, 3);
		case 2:
			return new Vector2(3, 2);
		case 3:
			return new Vector2(16, 10);
		case 4:
			return new Vector2(17, 10);
		case 5:
			return new Vector2(16, 9);
		case 6:
			return new Vector2(2, 1);
		case 7:
			return new Vector2(3, 4);
		case 8:
			return new Vector2(2, 3);
		case 9:
			return new Vector2(10, 16);
		case 10:
			return new Vector2(10, 17);
		case 11:
			return new Vector2(9, 16);
		case 12:
			return new Vector2(1, 2);
		}
	}

	public override void OnInspectorGUI()
	{
		View t = target as View;

		EditorGUI.BeginChangeCheck();

		string[] ratios = { "<None>", "Landscape / 4:3", "Landscape / 3:2", "Landscape / 16:10", "Landscape / 17:10", "Landscape / 16:9", "Landscape / 2:1", "Portrait / 3:4", "Portrait / 2:3", "Portrait / 10:16", "Portrait / 10:17", "Portrait / 9:16", "Portrait / 1:2" };

		Vector2 primaryAspectRatio = EditorGUILayout.Vector2Field(new GUIContent("Primary Aspect Ratio", "Width and height values that define the primary aspect ratio (e.g. 4:3)"), t.primaryAspectRatio);
		int primaryIndex = EditorGUILayout.Popup("Select Aspect Ratio", 0, ratios);
		if (primaryIndex > 0)
		{
			primaryAspectRatio = LookupAspectRatio(primaryIndex);
		}
		EditorGUILayout.Separator();

		Vector2 secondaryAspectRatio = EditorGUILayout.Vector2Field(new GUIContent("Secondary Aspect Ratio", "Width and height values that define the primary aspect ratio (e.g. 4:3)"), t.secondaryAspectRatio);
		int secondaryIndex = EditorGUILayout.Popup("Select Aspect Ratio", 0, ratios);
		if (secondaryIndex > 0)
		{
			secondaryAspectRatio = LookupAspectRatio(secondaryIndex);
		}
		EditorGUILayout.Separator();

		Color primaryColor = EditorGUILayout.ColorField(new GUIContent("Primary Color", "Color for inner primary aspect ratio rectangle"), t.primaryColor);
		Color secondaryColor = EditorGUILayout.ColorField(new GUIContent("Secondary Color", "Color for outer secondary aspect ratio rectangle"), t.secondaryColor);

		if (EditorGUI.EndChangeCheck())
		{
			// Avoid divide by zero errors
			if (primaryAspectRatio.y == 0)
			{
				primaryAspectRatio.y = 1;
			}
			if (secondaryAspectRatio.y == 0)
			{
				secondaryAspectRatio.y = 1;
			}

			t.primaryAspectRatio = primaryAspectRatio;
			t.secondaryAspectRatio = secondaryAspectRatio;
			t.primaryColor = primaryColor;
			t.secondaryColor = secondaryColor;

			SceneView.RepaintAll();
		}
	}
	
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
		View view = target as View;

		DrawView(view);

		Vector3 pos = view.transform.position;

		float viewSize = CalculateLocalViewSize(view);

		Vector3[] handles = new Vector3[2];
		handles[0] = view.transform.TransformPoint(new Vector3(0, -viewSize, 0));
		handles[1] = view.transform.TransformPoint(new Vector3(0, viewSize, 0));

		Handles.color = view.primaryColor;

		for (int i = 0; i < 2; ++i)
		{
			Vector3 newPos = Handles.FreeMoveHandle(handles[i],
			                                        Quaternion.identity,
			                                        HandleUtility.GetHandleSize(pos) * 0.1f,
			                                        Vector3.zero,
			                                        Handles.CubeCap);
			if (newPos != handles[i])
			{
				Undo.RecordObject(view, "Changed view size");
				view.viewSize = (newPos - pos).magnitude;
				break;
			}
		}
	}

	public static void DrawView(View view)
	{	
		float height = CalculateLocalViewSize(view);
		float widthA = height * (view.primaryAspectRatio.x / view.primaryAspectRatio.y);
		float widthB = height * (view.secondaryAspectRatio.x / view.secondaryAspectRatio.y);

		// Draw left box
		{
			Vector3[] verts = new Vector3[4];
			verts[0] = view.transform.TransformPoint(new Vector3(-widthB, -height, 0));
			verts[1] = view.transform.TransformPoint(new Vector3(-widthB, height, 0));
			verts[2] = view.transform.TransformPoint(new Vector3(-widthA, height, 0));
			verts[3] = view.transform.TransformPoint(new Vector3(-widthA, -height, 0));

			Handles.DrawSolidRectangleWithOutline(verts, view.secondaryColor, view.primaryColor );
		}

		// Draw right box
		{
			Vector3[] verts = new Vector3[4];
			verts[0] = view.transform.TransformPoint(new Vector3(widthA, -height, 0));
			verts[1] = view.transform.TransformPoint(new Vector3(widthA, height, 0));
			verts[2] = view.transform.TransformPoint(new Vector3(widthB, height, 0));
			verts[3] = view.transform.TransformPoint(new Vector3(widthB, -height, 0));
			
			Handles.DrawSolidRectangleWithOutline(verts, view.secondaryColor, view.primaryColor );
		}

		// Draw center box
		{
			Vector3[] verts = new Vector3[4];
			verts[0] = view.transform.TransformPoint(new Vector3(-widthA, -height, 0));
			verts[1] = view.transform.TransformPoint(new Vector3(-widthA, height, 0));
			verts[2] = view.transform.TransformPoint(new Vector3(widthA, height, 0));
			verts[3] = view.transform.TransformPoint(new Vector3(widthA, -height, 0));

			Handles.DrawSolidRectangleWithOutline(verts, new Color(1,1,1,0f), view.primaryColor );
		}
	}

	// Calculate view size in local coordinates
	// Kinda expensive, but accurate and only called in editor.
	static float CalculateLocalViewSize(View view)
	{
		return view.transform.InverseTransformPoint(view.transform.position + new Vector3(0, view.viewSize, 0)).magnitude;
	}
}
