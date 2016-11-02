// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    public class FlowchartWindow : EditorWindow
    {
        public static List<Block> deleteList = new List<Block>();

        protected List<Block> windowBlockMap = new List<Block>();

        // The ReorderableList control doesn't drag properly when used with GUI.DragWindow(),
        // so we just implement dragging ourselves.
        protected int dragWindowId = -1;
        protected Vector2 startDragPosition;

        public const float minZoomValue = 0.25f;
        public const float maxZoomValue = 1f;

        protected GUIStyle nodeStyle = new GUIStyle();

        protected static BlockInspector blockInspector;

        protected bool mouseOverVariables = false;

        protected int forceRepaintCount;

        protected Texture2D addTexture;

        protected Rect selectionBox;
        protected Vector2 startSelectionBoxPosition;
        protected List<Block> mouseDownSelectionState;
        
        [MenuItem("Tools/Fungus/Flowchart Window")]
        static void Init()
        {
            GetWindow(typeof(FlowchartWindow), false, "Flowchart");
        }

        protected virtual void OnEnable()
        {
            // All block nodes use the same GUIStyle, but with a different background
            nodeStyle.border.left = 20;
            nodeStyle.border.right = 20;
            nodeStyle.border.top = 5;
            nodeStyle.border.bottom = 5;
            nodeStyle.padding.left = 20;
            nodeStyle.padding.right = 20;
            nodeStyle.padding.top = 5;
            nodeStyle.padding.bottom = 5;
            nodeStyle.contentOffset = Vector2.zero;
            nodeStyle.alignment = TextAnchor.MiddleCenter;
            nodeStyle.wordWrap = true;

            addTexture = Resources.Load("Icons/add_small") as Texture2D;
        }

        protected virtual void OnInspectorUpdate()
        {
            // Ensure the Block Inspector is always showing the currently selected block
            var flowchart = GetFlowchart();
            if (flowchart == null)
            {
                return;
            }

            if (Selection.activeGameObject == null &&
                flowchart.SelectedBlock != null &&
                flowchart.IsActive())
            {
                if (blockInspector == null)
                {
                    ShowBlockInspector(flowchart);
                }
                blockInspector.block = (Block)flowchart.SelectedBlock;
            }

            forceRepaintCount--;
            forceRepaintCount = Math.Max(0, forceRepaintCount);

            Repaint();
        }

        public static Flowchart GetFlowchart()
        {
            // Using a temp hidden object to track the active Flowchart across 
            // serialization / deserialization when playing the game in the editor.
            FungusState fungusState = GameObject.FindObjectOfType<FungusState>();
            if (fungusState == null)
            {
                GameObject go = new GameObject("_FungusState");
                go.hideFlags = HideFlags.HideInHierarchy;
                fungusState = go.AddComponent<FungusState>();
            }

            if (Selection.activeGameObject != null)
            {
                var fs = Selection.activeGameObject.GetComponent<Flowchart>();
                if (fs != null)
                {
                    fungusState.SelectedFlowchart = fs;
                }
            }

            return fungusState.SelectedFlowchart;
        }

        protected virtual void OnGUI()
        {
            var flowchart = GetFlowchart();
            if (flowchart == null)
            {
                GUILayout.Label("No Flowchart scene object selected");
                return;
            }

            // Delete any scheduled objects
            foreach (var deleteBlock in deleteList)
            {
                bool isSelected = (flowchart.SelectedBlocks.Contains(deleteBlock));

                var commandList = deleteBlock.CommandList;
                foreach (var command in commandList)
                {
                    Undo.DestroyObjectImmediate(command);
                }
                
                Undo.DestroyObjectImmediate((Block)deleteBlock);
                flowchart.ClearSelectedCommands();

                if (isSelected)
                {
                    // Revert to showing properties for the Flowchart
                    Selection.activeGameObject = flowchart.gameObject;
                }
            }
            deleteList.Clear();

            DrawFlowchartView(flowchart);
            DrawOverlay(flowchart);

            // Handle selection box events after block and overlay events
            HandleSelectionBox(flowchart);

            if (forceRepaintCount > 0)
            {
                // Redraw on next frame to get crisp refresh rate
                Repaint();
            }
        }

        protected virtual void DrawOverlay(Flowchart flowchart)
        {
            GUILayout.Space(8);
            
            GUILayout.BeginHorizontal();
            
            GUILayout.Space(8);

            if (GUILayout.Button(new GUIContent(addTexture, "Add a new block")))
            {
                Vector2 newNodePosition = new Vector2(50 - flowchart.ScrollPos.x, 
                                                      50 - flowchart.ScrollPos.y);
                CreateBlock(flowchart, newNodePosition);
            }

            GUILayout.Space(8);

            flowchart.Zoom = GUILayout.HorizontalSlider(flowchart.Zoom, minZoomValue, maxZoomValue, GUILayout.Width(100));

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Label(flowchart.name, EditorStyles.whiteBoldLabel);
            if (flowchart.Description.Length > 0)
            {
                GUILayout.Label(flowchart.Description, EditorStyles.helpBox);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(440));
        
            GUILayout.FlexibleSpace();

            flowchart.VariablesScrollPos = GUILayout.BeginScrollView(flowchart.VariablesScrollPos, GUILayout.MaxHeight(position.height * 0.75f));

            GUILayout.FlexibleSpace();

            GUILayout.Space(8);

            FlowchartEditor flowchartEditor = Editor.CreateEditor (flowchart) as FlowchartEditor;
            flowchartEditor.DrawVariablesGUI();
            DestroyImmediate(flowchartEditor);

            Rect variableWindowRect = GUILayoutUtility.GetLastRect();
            if (flowchart.VariablesExpanded &&
                flowchart.Variables.Count > 0)
            {
                variableWindowRect.y -= 20;
                variableWindowRect.height += 20;
            }
            if (Event.current.type == EventType.Repaint)
            {
                mouseOverVariables = variableWindowRect.Contains(Event.current.mousePosition); 
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
        
        protected virtual void DrawFlowchartView(Flowchart flowchart)
        {
            Block[] blocks = flowchart.GetComponents<Block>();

            foreach (var block in blocks)
            {
                var node = block as Node;
                if (node == null)
                {
                    continue;
                }

                var newRect = new Rect();
                newRect.xMin = Mathf.Min(flowchart.ScrollViewRect.xMin, node._NodeRect.xMin - 400);
                newRect.xMax = Mathf.Max(flowchart.ScrollViewRect.xMax, node._NodeRect.xMax + 400);
                newRect.yMin = Mathf.Min(flowchart.ScrollViewRect.yMin, node._NodeRect.yMin - 400);
                newRect.yMax = Mathf.Max(flowchart.ScrollViewRect.yMax, node._NodeRect.yMax + 400);
                flowchart.ScrollViewRect = newRect;
            }

            // Calc rect for script view
            Rect scriptViewRect = new Rect(0, 0, this.position.width / flowchart.Zoom, this.position.height / flowchart.Zoom);

            EditorZoomArea.Begin(flowchart.Zoom, scriptViewRect);

            DrawGrid(flowchart);
            
            GLDraw.BeginGroup(scriptViewRect);

            // The center of the Flowchart depends on the block positions and window dimensions, so we calculate it 
            // here in the FlowchartWindow class and store it on the Flowchart object for use later.
            CalcFlowchartCenter(flowchart, blocks);

            // Draw connections
            foreach (var block in blocks)
            {
                DrawConnections(flowchart, block, false);
            }
            foreach (var block in blocks)
            {
                DrawConnections(flowchart, block, true);
            }

            GUIStyle windowStyle = new GUIStyle();
            windowStyle.stretchHeight = true;

            BeginWindows();

            windowBlockMap.Clear();
            bool useEvent = false;
            bool endDrag = false;
            for (int i = 0; i < blocks.Length; ++i)
            {
                var block = blocks[i];

                float nodeWidthA = nodeStyle.CalcSize(new GUIContent(block.BlockName)).x + 10;
                float nodeWidthB = 0f;
                if (block._EventHandler != null)
                {
                    nodeWidthB = nodeStyle.CalcSize(new GUIContent(block._EventHandler.GetSummary())).x + 10;
                }

                if (Event.current.button == 0)
                {
                    Rect tempRect = block._NodeRect;
                    tempRect.width = Mathf.Max(Mathf.Max(nodeWidthA, nodeWidthB), 120);
                    tempRect.height = 40;

                    if (dragWindowId > -1 && flowchart.SelectedBlocks.Contains(block))
                    {
                        if (Event.current.type == EventType.MouseDrag)
                        {
                            tempRect.x += Event.current.delta.x;
                            tempRect.y += Event.current.delta.y;

                            forceRepaintCount = 6;
                            useEvent = true;
                        }
                        else if (Event.current.rawType == EventType.MouseUp)
                        {
                            Vector2 newPos = new Vector2(tempRect.x, tempRect.y);
                            tempRect.x = startDragPosition.x + (newPos.x - blocks[dragWindowId]._NodeRect.position.x);
                            tempRect.y = startDragPosition.y + (newPos.y - blocks[dragWindowId]._NodeRect.position.y);

                            block._NodeRect = tempRect;
                            
                            Undo.RecordObject(block, "Node Position");
                            
                            tempRect.x = newPos.x;
                            tempRect.y = newPos.y;

                            forceRepaintCount = 6;
                            useEvent = true;
                            endDrag = true;
                        }
                    }

                    block._NodeRect = tempRect;
                }

                Rect windowRect = new Rect(block._NodeRect);
                windowRect.x += flowchart.ScrollPos.x;
                windowRect.y += flowchart.ScrollPos.y;

                GUILayout.Window(i, windowRect, DrawWindow, "", windowStyle);

                GUI.backgroundColor = Color.white;

                windowBlockMap.Add(block);
            }

            dragWindowId = endDrag ? -1 : dragWindowId;

            if (useEvent)
            {
                Event.current.Use();
            }

            EndWindows();

            // Draw Event Handler labels
            foreach (var block in blocks)
            {
                if (block._EventHandler != null)
                {
                    string handlerLabel = "";
                    EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(block._EventHandler.GetType());
                    if (info != null)
                    {
                        handlerLabel = "<" + info.EventHandlerName + "> ";
                    }

                    GUIStyle handlerStyle = new GUIStyle(EditorStyles.whiteLabel);
                    handlerStyle.wordWrap = true;
                    handlerStyle.margin.top = 0;
                    handlerStyle.margin.bottom = 0;
                    handlerStyle.alignment = TextAnchor.MiddleCenter;

                    Rect rect = new Rect(block._NodeRect);
                    rect.height = handlerStyle.CalcHeight(new GUIContent(handlerLabel), block._NodeRect.width);
                    rect.x += flowchart.ScrollPos.x;
                    rect.y += flowchart.ScrollPos.y - rect.height;

                    GUI.Label(rect, handlerLabel, handlerStyle);
                }
            }
                
            // Draw play icons beside all executing blocks
            if (Application.isPlaying)
            {
                foreach (var b in blocks)
                {
                    if (b.IsExecuting())
                    {
                        b.ExecutingIconTimer = Time.realtimeSinceStartup + FungusConstants.ExecutingIconFadeTime;
                        b.ActiveCommand.ExecutingIconTimer = Time.realtimeSinceStartup + FungusConstants.ExecutingIconFadeTime;
                        forceRepaintCount = 6;
                    }

                    if (b.ExecutingIconTimer > Time.realtimeSinceStartup)
                    {
                        Rect rect = new Rect(b._NodeRect);

                        rect.x += flowchart.ScrollPos.x - 37;
                        rect.y += flowchart.ScrollPos.y + 3;
                        rect.width = 34;
                        rect.height = 34;

                        if (!b.IsExecuting())
                        {
                            float alpha = (b.ExecutingIconTimer - Time.realtimeSinceStartup) / FungusConstants.ExecutingIconFadeTime;
                            alpha = Mathf.Clamp01(alpha);
                            GUI.color = new Color(1f, 1f, 1f, alpha); 
                        }

                        if (GUI.Button(rect, FungusEditorResources.texPlayBig as Texture, new GUIStyle()))
                        {
                            SelectBlock(flowchart, b);
                        }

                        GUI.color = Color.white;
                    }
                }
            }

            PanAndZoom(flowchart);

            GLDraw.EndGroup();

            EditorZoomArea.End();

            // If event has yet to be used and user isn't multiselecting or panning, clear selection
            bool validModifier = Event.current.alt || GetAppendModifierDown();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !validModifier)
            {
                Undo.RecordObject(flowchart, "Deselect");
                flowchart.ClearSelectedCommands();
                flowchart.ClearSelectedBlocks();
                Selection.activeGameObject = flowchart.gameObject;                
            }

            // Draw selection box
            if (startSelectionBoxPosition.x >= 0 && startSelectionBoxPosition.y >= 0)
			{
				GUI.Box(selectionBox, "", (GUIStyle) "SelectionRect");
                forceRepaintCount = 6;
			}
        }

        public virtual void CalcFlowchartCenter(Flowchart flowchart, Block[] blocks)
        {
            if (flowchart == null ||
                blocks.Count() == 0)
            {
                return;
            }

            Vector2 min = blocks[0]._NodeRect.min;
            Vector2 max = blocks[0]._NodeRect.max;

            foreach (var block in blocks)
            {
                min.x = Mathf.Min(min.x, block._NodeRect.center.x);
                min.y = Mathf.Min(min.y, block._NodeRect.center.y);
                max.x = Mathf.Max(max.x, block._NodeRect.center.x);
                max.y = Mathf.Max(max.y, block._NodeRect.center.y);
            }

            Vector2 center = (min + max) * -0.5f;

            center.x += position.width * 0.5f;
            center.y += position.height * 0.5f;

            flowchart.CenterPosition = center;
        }

        protected virtual void HandleSelectionBox(Flowchart flowchart)
        {
            if (Event.current.button == 0 && Event.current.modifiers != EventModifiers.Alt &&
                !(UnityEditor.Tools.current == Tool.View && UnityEditor.Tools.viewTool == ViewTool.Pan))
            {
                switch (Event.current.type)
                {
                case EventType.MouseDown:
                    startSelectionBoxPosition = Event.current.mousePosition;
                    mouseDownSelectionState = new List<Block>(flowchart.SelectedBlocks);
                    Event.current.Use();
                    break;

                case EventType.MouseDrag:
                    if (startSelectionBoxPosition.x >= 0 && startSelectionBoxPosition.y >= 0)
                    {
                        var topLeft = Vector2.Min(startSelectionBoxPosition, Event.current.mousePosition);
					    var bottomRight = Vector2.Max(startSelectionBoxPosition, Event.current.mousePosition);
                        selectionBox = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

                        Rect zoomSelectionBox = selectionBox;
                        zoomSelectionBox.position -= flowchart.ScrollPos * flowchart.Zoom;
                        zoomSelectionBox.position /= flowchart.Zoom;
                        zoomSelectionBox.size /= flowchart.Zoom;

                        foreach (var block in flowchart.GetComponents<Block>())
                        {
                            if (zoomSelectionBox.Overlaps(block._NodeRect))
                            {
                                if (mouseDownSelectionState.Contains(block))
                                {
                                    flowchart.SelectedBlocks.Remove(block);
                                }
                                else
                                {
                                    flowchart.AddSelectedBlock(block);
                                }
                            }
                            else if (mouseDownSelectionState.Contains(block))
                            {
                                flowchart.AddSelectedBlock(block);
                            }
                            else
                            {
                                flowchart.SelectedBlocks.Remove(block);
                            }
                        }
                    }
                    Event.current.Use();
                    break;
                }

                if (Event.current.rawType == EventType.MouseUp)
                {
                    selectionBox.size = Vector2.zero;
                    selectionBox.position = Vector2.one * -1;
                    startSelectionBoxPosition = selectionBox.position;

                    var tempList = new List<Block>(flowchart.SelectedBlocks);
                    flowchart.SelectedBlocks = mouseDownSelectionState;
                    Undo.RecordObject(flowchart, "Select");
                    flowchart.SelectedBlocks = tempList;

                    if (flowchart.SelectedBlock != null)
                    {
                        SetBlockForInspector(flowchart, flowchart.SelectedBlock);
                    }
                }
            }
        }

        protected virtual void PanAndZoom(Flowchart flowchart)
        {
            // Right click to drag view
            bool drag = false;
            
            // Pan tool
            if (UnityEditor.Tools.current == Tool.View && UnityEditor.Tools.viewTool == ViewTool.Pan &&
                Event.current.button == 0 && Event.current.type == EventType.MouseDrag)
            {
                drag = true;
            }
            
            // Right or middle button drag
            if (Event.current.button > 0 && Event.current.type == EventType.MouseDrag)
            {
                drag = true;
            }
            
            // Alt + left mouse drag
            if (Event.current.alt &&
                Event.current.button == 0 && Event.current.type == EventType.MouseDrag)
            {
                drag = true;
            }
            
            if (drag)
            {
                flowchart.ScrollPos += Event.current.delta;
                forceRepaintCount = 6;
            }
            
            bool zoom = false;
            
            // Scroll wheel
            if (Event.current.type == EventType.ScrollWheel)
            {
                zoom = true;
            }
            
            // Zoom tool
            if (UnityEditor.Tools.current == Tool.View && UnityEditor.Tools.viewTool == ViewTool.Zoom &&
                Event.current.button == 0 && Event.current.type == EventType.MouseDrag)
            {
                zoom = true;
            }
            
            if (zoom && selectionBox.size == Vector2.zero)
            {
                flowchart.Zoom -= Event.current.delta.y * 0.01f;
                flowchart.Zoom = Mathf.Clamp(flowchart.Zoom, minZoomValue, maxZoomValue);
                forceRepaintCount = 6;
            }
        }

        protected virtual void DrawGrid(Flowchart flowchart)
        {
            float width = this.position.width / flowchart.Zoom;
            float height = this.position.height / flowchart.Zoom;

            // Match background color of scene view
            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = new Color32(71, 71, 71, 255); 
            }
            else
            {
                GUI.color = new Color32(86, 86, 86, 255); 
            }
            GUI.DrawTexture( new Rect(0,0, width, height), EditorGUIUtility.whiteTexture );

            GUI.color = Color.white;
            Color color = new Color32(96, 96, 96, 255);

            float gridSize = 128f;
            
            float x = flowchart.ScrollPos.x % gridSize;
            while (x < width)
            {
                GLDraw.DrawLine(new Vector2(x, 0), new Vector2(x, height), color, 1f);
                x += gridSize;
            }
            
            float y = (flowchart.ScrollPos.y % gridSize);
            while (y < height)
            {
                if (y >= 0)
                {
                    GLDraw.DrawLine(new Vector2(0, y), new Vector2(width, y), color, 1f);
                }
                y += gridSize;
            }
        }

        protected virtual void SelectBlock(Flowchart flowchart, Block block)
        {
            // Select the block and also select currently executing command
            flowchart.SelectedBlock = block;
            SetBlockForInspector(flowchart, block);
        }
        
        public static Block CreateBlock(Flowchart flowchart, Vector2 position)
        {
            Block newBlock = flowchart.CreateBlock(position);
            Undo.RegisterCreatedObjectUndo(newBlock, "New Block");

            // Use AddSelected instead of Select for when multiple blocks are duplicated
            flowchart.AddSelectedBlock(newBlock);
            SetBlockForInspector(flowchart, newBlock);

            return newBlock;
        }

        protected virtual void DeleteBlock(Flowchart flowchart, Block block)
        {
            var commandList = block.CommandList;
            foreach (var command in commandList)
            {
                Undo.DestroyObjectImmediate(command);
            }
            
            Undo.DestroyObjectImmediate((Block)block);
            flowchart.ClearSelectedCommands();
        }

        protected virtual void DrawWindow(int windowId)
        {
            var block = windowBlockMap[windowId];
            var flowchart = (Flowchart)block.GetFlowchart();
                            
            if (flowchart == null)
            {
                return;
            }

            // Select block when node is clicked
            if (Event.current.button == 0 && 
                Event.current.type == EventType.MouseDown &&
                !mouseOverVariables)
            {
                // Check if might be start of a window drag
                if (Event.current.button == 0 &&
                    Event.current.alt == false)
                {
                    if (!GetAppendModifierDown())
                    {
                        dragWindowId = windowId;

                        startDragPosition.x = block._NodeRect.x;
                        startDragPosition.y = block._NodeRect.y;
                    }

                    Event.current.Use();
                }

                if (windowId < windowBlockMap.Count)
                {
                    Undo.RecordObject(flowchart, "Select");

                    if (GetAppendModifierDown())
                    {
                        if (flowchart.SelectedBlocks.Contains(block))
                        {
                            flowchart.SelectedBlocks.Remove(block);
                        }
                        else
                        {
                            flowchart.AddSelectedBlock(block);
                        }
                    }
                    else
                    {
                        if (flowchart.SelectedBlocks.Contains(block))
                        {
                            SetBlockForInspector(flowchart, block);
                        }
                        else
                        {
                            SelectBlock(flowchart, block);
                        }
                    }

                    GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
                }
            }

            bool selected = false;
            if (flowchart.SelectedBlocks.Contains(block))
            {
                selected = true;
            }

            GUIStyle nodeStyleCopy = new GUIStyle(nodeStyle);
            Texture2D offTex;
            Texture2D onTex;
            Color defaultColor;

            if (block._EventHandler != null)
            {
                offTex = FungusEditorResources.texEventNodeOff;
                onTex = FungusEditorResources.texEventNodeOn;
                defaultColor = FungusConstants.DefaultEventBlockTint;
            }
            else
            {
                // Count the number of unique connections (excluding self references)
                var uniqueList = new List<Block>();
                var connectedBlocks = block.GetConnectedBlocks();
                foreach (var connectedBlock in connectedBlocks)
                {
                    if (connectedBlock == block ||
                        uniqueList.Contains(connectedBlock))
                    {
                        continue;
                    }
                    uniqueList.Add(connectedBlock);
                }

                if (uniqueList.Count > 1)
                {
                    offTex = FungusEditorResources.texChoiceNodeOff;
                    onTex = FungusEditorResources.texChoiceNodeOn;
                    defaultColor = FungusConstants.DefaultChoiceBlockTint;
                }
                else
                {
                    offTex = FungusEditorResources.texProcessNodeOff;
                    onTex = FungusEditorResources.texProcessNodeOn;
                    defaultColor = FungusConstants.DefaultProcessBlockTint;
                }
            }

            // Make sure node is wide enough to fit the node name text
            var n = block as Node;
            float width = nodeStyleCopy.CalcSize(new GUIContent(block.BlockName)).x;
            Rect tempRect = n._NodeRect;
            tempRect.width = Mathf.Max (n._NodeRect.width, width);
            n._NodeRect = tempRect;

            Rect boxRect = GUILayoutUtility.GetRect(n._NodeRect.width, n._NodeRect.height);
            var tintColor = n.UseCustomTint ? n.Tint : defaultColor;

            // Draw untinted highlight
            if (selected)
            {
                GUI.backgroundColor = Color.white;
                nodeStyleCopy.normal.background = onTex;
                GUI.Box(boxRect, "", nodeStyleCopy);
            }

            // Draw tinted block; ensure text is readable
            var brightness = tintColor.r * 0.3 + tintColor.g * 0.59 + tintColor.b * 0.11;
            nodeStyleCopy.normal.textColor = brightness >= 0.5 ? Color.black : Color.white;

            nodeStyleCopy.normal.background = offTex;
            GUI.backgroundColor = tintColor;
            GUI.Box(boxRect, block.BlockName, nodeStyleCopy);

            GUI.backgroundColor = Color.white;

            if (block.Description.Length > 0)
            {
                GUIStyle descriptionStyle = new GUIStyle(EditorStyles.helpBox);
                descriptionStyle.wordWrap = true;
                GUILayout.Label(block.Description, descriptionStyle);
            }

            if (Event.current.type == EventType.ContextClick)
            {
                flowchart.AddSelectedBlock(block);

                GenericMenu menu = new GenericMenu ();
                
                // Use a copy because flowchart.SelectedBlocks gets modified
                var blockList = new List<Block>(flowchart.SelectedBlocks);
                menu.AddItem(new GUIContent ("Duplicate"), false, DuplicateBlocks, blockList);
                menu.AddItem(new GUIContent ("Delete"), false, DeleteBlocks, blockList);

                menu.ShowAsContext();           
            }
        }

        protected virtual void DrawConnections(Flowchart flowchart, Block block, bool highlightedOnly)
        {
            if (block == null)
            {
                return;
            }

            var connectedBlocks = new List<Block>();

            bool blockIsSelected = (flowchart.SelectedBlock != block);

            var commandList = block.CommandList;
            foreach (var command in commandList)
            {
                if (command == null)
                {
                    continue;
                }

                bool commandIsSelected = false;
                var selectedCommands = flowchart.SelectedCommands;
                foreach (var selectedCommand in selectedCommands)
                {
                    if (selectedCommand == command)
                    {
                        commandIsSelected = true;
                        break;
                    }
                }

                bool highlight = command.IsExecuting || (blockIsSelected && commandIsSelected);

                if (highlightedOnly && !highlight ||
                    !highlightedOnly && highlight)
                {
                    continue;
                }

                connectedBlocks.Clear();
                command.GetConnectedBlocks(ref connectedBlocks);

                foreach (var blockB in connectedBlocks)
                {
                    if (blockB == null ||
                        block == blockB ||
                        !blockB.GetFlowchart().Equals(flowchart))
                    {
                        continue;
                    }

                    Rect startRect = new Rect(block._NodeRect);
                    startRect.x += flowchart.ScrollPos.x;
                    startRect.y += flowchart.ScrollPos.y;

                    Rect endRect = new Rect(blockB._NodeRect);
                    endRect.x += flowchart.ScrollPos.x;
                    endRect.y += flowchart.ScrollPos.y;

                    DrawRectConnection(startRect, endRect, highlight);
                }
            }
        }

        protected virtual void DrawRectConnection(Rect rectA, Rect rectB, bool highlight)
        {
            Vector2[] pointsA = new Vector2[] {
                new Vector2(rectA.xMin + 5, rectA.center.y),
                new Vector2(rectA.xMin + rectA.width / 2, rectA.yMin + 2),
                new Vector2(rectA.xMin + rectA.width / 2, rectA.yMax - 2),
                new Vector2(rectA.xMax - 5, rectA.center.y) 
            };

            Vector2[] pointsB = new Vector2[] {
                new Vector2(rectB.xMin + 5, rectB.center.y),
                new Vector2(rectB.xMin + rectB.width / 2, rectB.yMin + 2),
                new Vector2(rectB.xMin + rectB.width / 2, rectB.yMax - 2),
                new Vector2(rectB.xMax - 5, rectB.center.y)
            };

            Vector2 pointA = Vector2.zero;
            Vector2 pointB = Vector2.zero;
            float minDist = float.MaxValue;

            foreach (var a in pointsA)
            {
                foreach (var b in pointsB)
                {
                    float d = Vector2.Distance(a, b);
                    if (d < minDist)
                    {
                        pointA = a;
                        pointB = b;
                        minDist = d;
                    }
                }
            }

            Color color = Color.grey;
            if (highlight)
            {
                color = Color.green;
            }

            GLDraw.DrawConnectingCurve(pointA, pointB, color, 1.025f);

            Rect dotARect = new Rect(pointA.x - 5, pointA.y - 5, 10, 10);
            GUI.Label(dotARect, "", new GUIStyle("U2D.dragDotActive"));

            Rect dotBRect = new Rect(pointB.x - 5, pointB.y - 5, 10, 10);
            GUI.Label(dotBRect, "", new GUIStyle("U2D.dragDotActive"));
        }

        public static void DeleteBlocks(object obj)
        {
            var blocks = obj as List<Block>;
            blocks.ForEach(block => FlowchartWindow.deleteList.Add(block));
        }
        
        protected static void DuplicateBlocks(object obj)
        {
            var flowchart = GetFlowchart();

            Undo.RecordObject(flowchart, "Select");
            flowchart.ClearSelectedBlocks();

            var blocks = obj as List<Block>;

            foreach (var block in blocks)
            {
                Vector2 newPosition = new Vector2(block._NodeRect.position.x + 
                                              block._NodeRect.width + 20, 
                                              block._NodeRect.y);

                Block oldBlock = block;

                Block newBlock = FlowchartWindow.CreateBlock(flowchart, newPosition);
                newBlock.BlockName = flowchart.GetUniqueBlockKey(oldBlock.BlockName + " (Copy)");

                Undo.RecordObject(newBlock, "Duplicate Block");

                var commandList = oldBlock.CommandList;
                foreach (var command in commandList)
                {
                    if (ComponentUtility.CopyComponent(command))
                    {
                        if (ComponentUtility.PasteComponentAsNew(flowchart.gameObject))
                        {
                            Command[] commands = flowchart.GetComponents<Command>();
                            Command pastedCommand = commands.Last<Command>();
                            if (pastedCommand != null)
                            {
                                pastedCommand.ItemId = flowchart.NextItemId();
                                newBlock.CommandList.Add(pastedCommand);
                            }
                        }
                        
                        // This stops the user pasting the command manually into another game object.
                        ComponentUtility.CopyComponent(flowchart.transform);
                    }
                }

                if (oldBlock._EventHandler != null)
                {
                    if (ComponentUtility.CopyComponent(oldBlock._EventHandler))
                    {
                        if (ComponentUtility.PasteComponentAsNew(flowchart.gameObject))
                        {
                            EventHandler[] eventHandlers = flowchart.GetComponents<EventHandler>();
                            EventHandler pastedEventHandler = eventHandlers.Last<EventHandler>();
                            if (pastedEventHandler != null)
                            {
                                pastedEventHandler.ParentBlock = newBlock;
                                newBlock._EventHandler = pastedEventHandler;
                            }
                        }
                    }
                }
            }
        }

        protected static void ShowBlockInspector(Flowchart flowchart)
        {
            if (blockInspector == null)
            {
                // Create a Scriptable Object with a custom editor which we can use to inspect the selected block.
                // Editors for Scriptable Objects display using the full height of the inspector window.
                blockInspector = ScriptableObject.CreateInstance<BlockInspector>() as BlockInspector;
                blockInspector.hideFlags = HideFlags.DontSave;
            }

            Selection.activeObject = blockInspector;

            EditorUtility.SetDirty(blockInspector);
        }

        protected static void SetBlockForInspector(Flowchart flowchart, Block block)
        {
            ShowBlockInspector(flowchart);
            flowchart.ClearSelectedCommands();
            if (block.ActiveCommand != null)
            {
                flowchart.AddSelectedCommand(block.ActiveCommand);
            }
        }

        /// <summary>
        /// Displays a temporary text alert in the center of the Flowchart window.
        /// </summary>
        public static void ShowNotification(string notificationText)
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(FlowchartWindow), false, "Flowchart");
            if (window != null)
            {
                window.ShowNotification(new GUIContent(notificationText));
            }
        }

         protected virtual bool GetAppendModifierDown()
        {
            return Event.current.shift || EditorGUI.actionKey;
        }
    }
}