using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(Page))]
	public class PageEditor : Editor 
	{
		void OnSceneGUI () 
		{
			Page t = target as Page;

			// Render the parent view to help user position the page
			Transform parent = t.transform.parent;
			if (parent != null)
			{
				View view = parent.gameObject.GetComponent<View>();
				if (view != null)
				{
					ViewEditor.DrawView(view);
				}
			}

			if (t.enabled)
			{
				EditPageBounds();
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty(target);
			}
		}

		void EditPageBounds()
		{
			Page t = target as Page;
			Vector3 pos = t.transform.position;

			Vector3[] verts = new Vector3[4];
			verts[0] = new Vector3(pos.x + t.pageBounds.min.x, pos.y + t.pageBounds.min.y, 0);
			verts[1] = new Vector3(pos.x + t.pageBounds.min.x, pos.y + t.pageBounds.max.y, 0);
			verts[2] = new Vector3(pos.x + t.pageBounds.max.x, pos.y + t.pageBounds.max.y, 0);
			verts[3] = new Vector3(pos.x + t.pageBounds.max.x, pos.y + t.pageBounds.min.y, 0);
			
			Handles.DrawSolidRectangleWithOutline(verts, new Color(1,1,1,0.2f), new Color(0,0,0,1));

			for(int i = 0; i < 4; ++i)
			{
				Vector3 vert = verts[i];
				Vector3 newPos = Handles.FreeMoveHandle(vert,
				                                        Quaternion.identity,
				                                        HandleUtility.GetHandleSize(pos) * 0.1f,
				                                        Vector3.zero,
				                                        Handles.CubeCap);
				newPos.z = 0;
				verts[i] = newPos;
				
				if (vert != newPos)
				{
					switch(i)
					{
					case 0:
						verts[1].x = newPos.x;
						verts[3].y = newPos.y;
						break;
					case 1:
						verts[0].x = newPos.x;
						verts[2].y = newPos.y;
						break;
					case 2:
						verts[3].x = newPos.x;
						verts[1].y = newPos.y;
						break;
					case 3:
						verts[2].x = newPos.x;
						verts[0].y = newPos.y;
						break;
					}
					break;
				}
			}

			Bounds newBounds = new Bounds(verts[0], Vector3.zero);
			newBounds.Encapsulate(verts[1]);
			newBounds.Encapsulate(verts[2]);
			newBounds.Encapsulate(verts[3]);

			t.transform.position = newBounds.center;
			newBounds.center = Vector3.zero;

			t.pageBounds = newBounds;
		}
	}
}
