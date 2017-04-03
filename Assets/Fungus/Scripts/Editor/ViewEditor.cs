// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(View))]
    public class ViewEditor : Editor 
    {
        static Color viewColor = Color.yellow;

        protected SerializedProperty primaryAspectRatioProp;
        protected SerializedProperty secondaryAspectRatioProp;
        protected SerializedProperty viewSizeProp;

        // Draw Views when they're not selected
#if UNITY_5_0
        [DrawGizmo(GizmoType.NotSelected | GizmoType.SelectedOrChild)]
#else
        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
#endif
        static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            View view = objectTransform.gameObject.GetComponent<View>();
            if (view != null)
            {
                DrawView(view, false);
            }
        }

        protected virtual Vector2 LookupAspectRatio(int index)
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

        protected virtual void OnEnable()
        {
            primaryAspectRatioProp = serializedObject.FindProperty ("primaryAspectRatio");
            secondaryAspectRatioProp = serializedObject.FindProperty ("secondaryAspectRatio");
            viewSizeProp = serializedObject.FindProperty("viewSize");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(viewSizeProp);

            string[] ratios = { "<None>", "Landscape / 4:3", "Landscape / 3:2", "Landscape / 16:10", "Landscape / 17:10", "Landscape / 16:9", "Landscape / 2:1", "Portrait / 3:4", "Portrait / 2:3", "Portrait / 10:16", "Portrait / 10:17", "Portrait / 9:16", "Portrait / 1:2" };

            EditorGUILayout.PropertyField(primaryAspectRatioProp, new GUIContent("Primary Aspect Ratio", "Width and height values that define the primary aspect ratio (e.g. 4:3)"));
            int primaryIndex = EditorGUILayout.Popup("Select Aspect Ratio", 0, ratios);
            if (primaryIndex > 0)
            {
                primaryAspectRatioProp.vector2Value = LookupAspectRatio(primaryIndex);
            }
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(secondaryAspectRatioProp, new GUIContent("Secondary Aspect Ratio", "Width and height values that define the primary aspect ratio (e.g. 4:3)"));
            int secondaryIndex = EditorGUILayout.Popup("Select Aspect Ratio", 0, ratios);
            if (secondaryIndex > 0)
            {
                secondaryAspectRatioProp.vector2Value = LookupAspectRatio(secondaryIndex);
            }
            EditorGUILayout.Separator();

            if (EditorGUI.EndChangeCheck())
            {
                // Avoid divide by zero errors
                if (primaryAspectRatioProp.vector2Value.y == 0)
                {
                    primaryAspectRatioProp.vector2Value = new Vector2(primaryAspectRatioProp.vector2Value.x, 1f);
                }
                if (secondaryAspectRatioProp.vector2Value.y == 0)
                {
                    secondaryAspectRatioProp.vector2Value = new Vector2(secondaryAspectRatioProp.vector2Value.x, 1f);
                }

                SceneView.RepaintAll();
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        protected virtual void OnSceneGUI () 
        {
            View t = target as View;
            if (t.enabled)
            {
                EditViewBounds();
            }
        }
        
        protected virtual void EditViewBounds()
        {
            View view = target as View;

            DrawView(view, true);

            Vector3 pos = view.transform.position;

            float viewSize = CalculateLocalViewSize(view);

            Vector3[] handles = new Vector3[2];
            handles[0] = view.transform.TransformPoint(new Vector3(0, -viewSize, 0));
            handles[1] = view.transform.TransformPoint(new Vector3(0, viewSize, 0));

            Handles.color = Color.white;

            for (int i = 0; i < 2; ++i)
            {
                Vector3 newPos = Handles.FreeMoveHandle(handles[i],
                                                        Quaternion.identity,
                                                        HandleUtility.GetHandleSize(pos) * 0.1f,
                                                        Vector3.zero,
#if UNITY_5_6_OR_NEWER
                                                        Handles.CubeHandleCap);
#else
                                                        Handles.CubeCap);
#endif

                if (newPos != handles[i])
                {
                    Undo.RecordObject(view, "Set View Size");
                    view.ViewSize = (newPos - pos).magnitude;
                    EditorUtility.SetDirty(view);
                    break;
                }
            }
        }

        public static void DrawView(View view, bool drawInterior)
        {   
            float height = CalculateLocalViewSize(view);
            float widthA = height * (view.PrimaryAspectRatio.x / view.PrimaryAspectRatio.y);
            float widthB = height * (view.SecondaryAspectRatio.x / view.SecondaryAspectRatio.y);

            Color transparent = new Color(1,1,1,0f);
            Color fill = viewColor;
            Color outline = viewColor;

            bool highlight = Selection.activeGameObject == view.gameObject;

            var flowchart = FlowchartWindow.GetFlowchart();
            if (flowchart != null)
            {
                var selectedCommands = flowchart.SelectedCommands;
                foreach (var command in selectedCommands)
                {
                    MoveToView moveToViewCommand = command as MoveToView;
                    if (moveToViewCommand != null &&
                        moveToViewCommand.TargetView == view)
                    {
                        highlight = true;
                    }
                    else
                    {
                        FadeToView fadeToViewCommand = command as FadeToView;
                        if (fadeToViewCommand != null &&
                            fadeToViewCommand.TargetView == view)
                        {
                            highlight = true;
                        }
                    }
                }
            }

            if (highlight)
            {
                fill = outline = Color.green;
                fill.a = 0.1f;
                outline.a = 1f;
            }
            else
            {
                fill.a = 0.1f;
                outline.a = 0.5f;
            }

            if (drawInterior)
            {
                // Draw left box
                {
                    Vector3[] verts = new Vector3[4];
                    verts[0] = view.transform.TransformPoint(new Vector3(-widthB, -height, 0));
                    verts[1] = view.transform.TransformPoint(new Vector3(-widthB, height, 0));
                    verts[2] = view.transform.TransformPoint(new Vector3(-widthA, height, 0));
                    verts[3] = view.transform.TransformPoint(new Vector3(-widthA, -height, 0));

                    Handles.DrawSolidRectangleWithOutline(verts, fill, transparent);
                }

                // Draw right box
                {
                    Vector3[] verts = new Vector3[4];
                    verts[0] = view.transform.TransformPoint(new Vector3(widthA, -height, 0));
                    verts[1] = view.transform.TransformPoint(new Vector3(widthA, height, 0));
                    verts[2] = view.transform.TransformPoint(new Vector3(widthB, height, 0));
                    verts[3] = view.transform.TransformPoint(new Vector3(widthB, -height, 0));
                    
                    Handles.DrawSolidRectangleWithOutline(verts, fill, transparent);
                }

                // Draw inner box
                {
                    Vector3[] verts = new Vector3[4];
                    verts[0] = view.transform.TransformPoint(new Vector3(-widthA, -height, 0));
                    verts[1] = view.transform.TransformPoint(new Vector3(-widthA, height, 0));
                    verts[2] = view.transform.TransformPoint(new Vector3(widthA, height, 0));
                    verts[3] = view.transform.TransformPoint(new Vector3(widthA, -height, 0));
                    
                    Handles.DrawSolidRectangleWithOutline(verts, transparent, outline );
                }
            }

            // Draw outer box
            {
                Vector3[] verts = new Vector3[4];
                verts[0] = view.transform.TransformPoint(new Vector3(-widthB, -height, 0));
                verts[1] = view.transform.TransformPoint(new Vector3(-widthB, height, 0));
                verts[2] = view.transform.TransformPoint(new Vector3(widthB, height, 0));
                verts[3] = view.transform.TransformPoint(new Vector3(widthB, -height, 0));
                
                Handles.DrawSolidRectangleWithOutline(verts, transparent, outline );
            }
        }

        // Calculate view size in local coordinates
        // Kinda expensive, but accurate and only called in editor.
        static float CalculateLocalViewSize(View view)
        {
            return view.transform.InverseTransformPoint(view.transform.position + new Vector3(0, view.ViewSize, 0)).magnitude;
        }
    }
}