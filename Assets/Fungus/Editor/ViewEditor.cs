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
	[DrawGizmo(GizmoType.NotSelected)]
	static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
	{
		View view = objectTransform.gameObject.GetComponent<View>();
		if (view != null)
		{
			DrawView(view);
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
		float widthA = height * view.primaryAspectRatio;
		float widthB = height * view.secondaryAspectRatio;

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
