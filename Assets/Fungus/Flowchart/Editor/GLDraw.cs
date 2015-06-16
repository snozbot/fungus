using UnityEngine;
using System.Collections;
using System;

public class GLDraw
{
    /*
     * Clipping code: http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=230386#post230386
     * Thick line drawing code: http://unifycommunity.com/wiki/index.php?title=VectorLine
     */
    protected static bool clippingEnabled;
    protected static Rect clippingBounds;
    public static Material lineMaterial = null;

    /* @ Credit: "http://cs-people.bu.edu/jalon/cs480/Oct11Lab/clip.c" */
    protected static bool clip_test(float p, float q, ref float u1, ref float u2)
    {
        float r;
        bool retval = true;

        if (p < 0.0)
        {
            r = q / p;
            if (r > u2)
                retval = false;
            else if (r > u1)
                u1 = r;
        }
        else if (p > 0.0)
        {
            r = q / p;
            if (r < u1)
                retval = false;
            else if (r < u2)
                u2 = r;
        }
        else if (q < 0.0)
            retval = false;

        return retval;
    }

    public static bool segment_rect_intersection(Rect bounds, ref Vector2 p1, ref Vector2 p2)
    {
        float u1 = 0.0f, u2 = 1.0f, dx = p2.x - p1.x, dy;

        if (clip_test(-dx, p1.x - bounds.xMin, ref u1, ref u2))
        {
            if (clip_test(dx, bounds.xMax - p1.x, ref u1, ref u2))
            {
                dy = p2.y - p1.y;
                if (clip_test(-dy, p1.y - bounds.yMin, ref u1, ref u2))
                {
                    if (clip_test(dy, bounds.yMax - p1.y, ref u1, ref u2))
                    {
                        if (u2 < 1.0)
                        {
                            p2.x = p1.x + u2 * dx;
                            p2.y = p1.y + u2 * dy;
                        }

                        if (u1 > 0.0)
                        {
                            p1.x += u1 * dx;
                            p1.y += u1 * dy;
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

	public static void BeginGroup(Rect position)
	{
		clippingEnabled = true;
		clippingBounds = new Rect(0, 0, position.width, position.height);
		GUI.BeginGroup(position);
	}
	
	public static void EndGroup()
	{
		GUI.EndGroup();
		clippingBounds = new Rect(0, 0, Screen.width, Screen.height);
		clippingEnabled = false;
	}

	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPos, Rect viewRect, Rect clipRect)
	{
		clippingEnabled = true;
		clippingBounds = clipRect;
		return GUI.BeginScrollView(position, scrollPos, viewRect, GUIStyle.none, GUIStyle.none);
	}
	
	public static void EndScrollView()
	{
		GUI.EndScrollView();
		clippingBounds = new Rect(0, 0, Screen.width, Screen.height);
		clippingEnabled = false;
	}

    public static void CreateMaterial()
    {
        if (lineMaterial != null)
            return;

		lineMaterial = Resources.Load("GLLineDraw", typeof(Material)) as Material;
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        if (Event.current == null)
            return;
        if (Event.current.type != EventType.repaint)
            return;

        if (clippingEnabled)
            if (!segment_rect_intersection(clippingBounds, ref start, ref end))
                return;

        CreateMaterial();

        lineMaterial.SetPass(0);

        Vector3 startPt;
        Vector3 endPt;

        if (width == 1)
        {
            GL.Begin(GL.LINES);
			GL.Color(color);
            startPt = new Vector3(start.x, start.y, 0);
            endPt = new Vector3(end.x, end.y, 0);
            GL.Vertex(startPt);
            GL.Vertex(endPt);
        }
        else
        {
            GL.Begin(GL.QUADS);
			GL.Color(color);
            startPt = new Vector3(end.y, start.x, 0);
            endPt = new Vector3(start.y, end.x, 0);
            Vector3 perpendicular = (startPt - endPt).normalized * width;
            Vector3 v1 = new Vector3(start.x, start.y, 0);
            Vector3 v2 = new Vector3(end.x, end.y, 0);
            GL.Vertex(v1 - perpendicular);
            GL.Vertex(v1 + perpendicular);
            GL.Vertex(v2 + perpendicular);
            GL.Vertex(v2 - perpendicular);
        }
        GL.End();
    }

	public static void DrawRect(Rect rect, Color color)
	{
		if (Event.current == null)
			return;
		if (Event.current.type != EventType.repaint)
			return;

		CreateMaterial();
		// set the current material
		lineMaterial.SetPass( 0 );
		GL.Begin( GL.QUADS );
		GL.Color( color );
		GL.Vertex3( rect.xMin, rect.yMin, 0 );
		GL.Vertex3( rect.xMax, rect.yMin, 0 );
		GL.Vertex3( rect.xMax, rect.yMax, 0 );
		GL.Vertex3( rect.xMin, rect.yMax, 0 );
		GL.End();
	}

    public static void DrawBox(Rect box, Color color, float width)
    {
        Vector2 p1 = new Vector2(box.xMin, box.yMin);
        Vector2 p2 = new Vector2(box.xMax, box.yMin);
        Vector2 p3 = new Vector2(box.xMax, box.yMax);
        Vector2 p4 = new Vector2(box.xMin, box.yMax);
        DrawLine(p1, p2, color, width);
        DrawLine(p2, p3, color, width);
        DrawLine(p3, p4, color, width);
        DrawLine(p4, p1, color, width);
    }

    public static void DrawBox(Vector2 topLeftCorner, Vector2 bottomRightCorner, Color color, float width)
    {
        Rect box = new Rect(topLeftCorner.x, topLeftCorner.y, bottomRightCorner.x - topLeftCorner.x, bottomRightCorner.y - topLeftCorner.y);
        DrawBox(box, color, width);
    }

    public static void DrawRoundedBox(Rect box, float radius, Color color, float width)
    {
        Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
        p1 = new Vector2(box.xMin + radius, box.yMin);
        p2 = new Vector2(box.xMax - radius, box.yMin);
        p3 = new Vector2(box.xMax, box.yMin + radius);
        p4 = new Vector2(box.xMax, box.yMax - radius);
        p5 = new Vector2(box.xMax - radius, box.yMax);
        p6 = new Vector2(box.xMin + radius, box.yMax);
        p7 = new Vector2(box.xMin, box.yMax - radius);
        p8 = new Vector2(box.xMin, box.yMin + radius);

        DrawLine(p1, p2, color, width);
        DrawLine(p3, p4, color, width);
        DrawLine(p5, p6, color, width);
        DrawLine(p7, p8, color, width);

        Vector2 t1, t2;
        float halfRadius = radius / 2;

        t1 = new Vector2(p8.x, p8.y + halfRadius);
        t2 = new Vector2(p1.x - halfRadius, p1.y);
        DrawBezier(p8, t1, p1, t2, color, width);

        t1 = new Vector2(p2.x + halfRadius, p2.y);
        t2 = new Vector2(p3.x, p3.y - halfRadius);
        DrawBezier(p2, t1, p3, t2, color, width);

        t1 = new Vector2(p4.x, p4.y + halfRadius);
        t2 = new Vector2(p5.x + halfRadius, p5.y);
        DrawBezier(p4, t1, p5, t2, color, width);

        t1 = new Vector2(p6.x - halfRadius, p6.y);
        t2 = new Vector2(p7.x, p7.y + halfRadius);
        DrawBezier(p6, t1, p7, t2, color, width);
    }

    public static void DrawConnectingCurve(Vector2 start, Vector2 end, Color color, float width)
    {
        Vector2 distance = start - end;

        Vector2 tangentA = start;
        tangentA.x -= distance.x * 0.5f;
        Vector2 tangentB = end;
        tangentB.x += distance.x * 0.5f;

        int segments = Mathf.FloorToInt((distance.magnitude / 20) * 3);

        DrawBezier(start, tangentA, end, tangentB, color, width, segments);

		Vector2 pA = CubeBezier(start, tangentA, end, tangentB, 0.6f);
		Vector2 pB = CubeBezier(start, tangentA, end, tangentB, 0.7f);

		float arrowHeadSize = 5;
		
		Vector2 arrowPosA = pB;
		Vector2 arrowPosB = arrowPosA;
		Vector2 arrowPosC = arrowPosA;
		
		Vector2 dir = (pB - pA).normalized;

		arrowPosB.x += dir.y * arrowHeadSize;
		arrowPosB.y -= dir.x * arrowHeadSize;
		arrowPosB -= dir * arrowHeadSize;

		arrowPosC.x -= dir.y * arrowHeadSize;
		arrowPosC.y += dir.x * arrowHeadSize;
		arrowPosC -= dir * arrowHeadSize;

		DrawLine(arrowPosA, arrowPosB, color, 1.025f);
		DrawLine(arrowPosA, arrowPosC, color, 1.025f);
	}

    public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width)
    {
        int segments = Mathf.FloorToInt((start - end).magnitude / 20) * 3; // Three segments per distance of 20
        DrawBezier(start, startTangent, end, endTangent, color, width, segments);
    }

    public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments)
    {
        Vector2 startVector = CubeBezier(start, startTangent, end, endTangent, 0);
        for (int i = 1; i <= segments; i++)
        {
            Vector2 endVector = CubeBezier(start, startTangent, end, endTangent, i / (float)segments);
            DrawLine(startVector, endVector, color, width);
            startVector = endVector;
        }
    }

    private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
    {
        float rt = 1 - t;
        float rtt = rt * t;
        return rt * rt * rt * s + 3 * rt * rtt * st + 3 * rtt * t * et + t * t * t * e;
    }
}