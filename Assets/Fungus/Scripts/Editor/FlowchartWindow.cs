// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Fungus.EditorUtils
{
    public class FlowchartWindow : EditorWindow
    {
        protected class ClipboardObject
        {
            internal SerializedObject serializedObject;
            internal Type type;

            internal ClipboardObject(Object obj)
            {
                serializedObject = new SerializedObject(obj);
                type = obj.GetType();
            }
        }

        protected class BlockCopy
        {
            private SerializedObject block = null;
            private List<ClipboardObject> commands = new List<ClipboardObject>();
            private ClipboardObject eventHandler = null;

            internal BlockCopy(Block block)
            {
                this.block = new SerializedObject(block);
                foreach (var command in block.CommandList)
                {
                    commands.Add(new ClipboardObject(command));
                }
                if (block._EventHandler != null)
                {
                    eventHandler = new ClipboardObject(block._EventHandler);
                }
            }

            private void CopyProperties(SerializedObject source, Object dest, params SerializedPropertyType[] excludeTypes)
            {
                var newSerializedObject = new SerializedObject(dest);
                var prop = source.GetIterator();
                while (prop.NextVisible(true))
                {
                    if (!excludeTypes.Contains(prop.propertyType))
                    {
                        newSerializedObject.CopyFromSerializedProperty(prop);
                    }
                }

                newSerializedObject.ApplyModifiedProperties();
            }

            internal Block PasteBlock(Flowchart flowchart)
            {
                var newBlock = FlowchartWindow.CreateBlock(flowchart, Vector2.zero);

                // Copy all command serialized properties
                // Copy references to match duplication behavior
                foreach (var command in commands)
                {
                    var newCommand = Undo.AddComponent(flowchart.gameObject, command.type) as Command;
                    CopyProperties(command.serializedObject, newCommand);
                    newCommand.ItemId = flowchart.NextItemId();
                    newBlock.CommandList.Add(newCommand);
                }

                // Copy event handler
                if (eventHandler != null)
                {
                    var newEventHandler = Undo.AddComponent(flowchart.gameObject, eventHandler.type) as EventHandler;
                    CopyProperties(eventHandler.serializedObject, newEventHandler);
                    newEventHandler.ParentBlock = newBlock;
                    newBlock._EventHandler = newEventHandler;     
                }

                // Copy block properties, but do not copy references because those were just assigned
                CopyProperties(
                    block,
                    newBlock,
                    SerializedPropertyType.ObjectReference,
                    SerializedPropertyType.Generic,
                    SerializedPropertyType.ArraySize
                );

                newBlock.BlockName = flowchart.GetUniqueBlockKey(block.FindProperty("blockName").stringValue + " (Copy)");

                return newBlock;
            }
        }

        protected struct BlockGraphics
        {
            internal Color tint;
            internal Texture2D onTexture;
            internal Texture2D offTexture;
        }

        protected List<BlockCopy> copyList = new List<BlockCopy>();
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
        protected Texture2D connectionPointTexture;

        protected Rect selectionBox;
        protected Vector2 startSelectionBoxPosition = -Vector2.one;
        protected List<Block> mouseDownSelectionState = new List<Block>();

        protected Color gridLineColor = Color.black;
        protected readonly Color connectionColor = new Color(0.65f, 0.65f, 0.65f, 1.0f);

        // Context Click occurs on MouseDown which interferes with panning
        // Track right click positions manually to show menus on MouseUp
        protected Vector2 rightClickDown = -Vector2.one;
        protected const float rightClickTolerance = 5f;

        protected const string searchFieldName = "search";
        private string searchString = string.Empty;
        protected Rect searchRect;
        protected Rect popupRect;
        protected Block[] filteredBlocks;
        protected int blockPopupSelection = -1;
        protected Vector2 popupScroll;
        protected bool mouseOverPopup;
        
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

            addTexture = FungusEditorResources.AddSmall;
            connectionPointTexture = FungusEditorResources.ConnectionPoint;
            gridLineColor.a = EditorGUIUtility.isProSkin ? 0.5f : 0.25f;

            copyList.Clear();

            wantsMouseMove = true; // For hover selection in block search popup  
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

        protected virtual void OnBecameVisible()
        {
            // Ensure that toolbar looks correct in both docked and undocked windows
            // The docked value doesn't always report correctly without the delayCall
            EditorApplication.delayCall += () => {
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                var isDockedMethod = typeof(EditorWindow).GetProperty("docked", flags).GetGetMethod(true);
                if ((bool) isDockedMethod.Invoke(this, null))
                {
                    EditorZoomArea.Offset = new Vector2(2.0f, 19.0f);
                }
                else
                {
                    EditorZoomArea.Offset = new Vector2(0.0f, 22.0f);
                }
            };
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

                if (deleteBlock._EventHandler != null)
                {
                    Undo.DestroyObjectImmediate(deleteBlock._EventHandler);
                }
                
                Undo.DestroyObjectImmediate((Block)deleteBlock);
                flowchart.ClearSelectedCommands();

                if (isSelected)
                {
                    // Deselect
                    flowchart.SelectedBlocks.Remove(deleteBlock);

                    // Revert to showing properties for the Flowchart
                    Selection.activeGameObject = flowchart.gameObject;
                }
            }
            deleteList.Clear();

            // Clear search filter focus
            if (Event.current.type == EventType.MouseDown && !searchRect.Contains(Event.current.mousePosition) &&
                !popupRect.Contains(Event.current.mousePosition))
            {
                CloseBlockPopup();
            }

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                if (GUI.GetNameOfFocusedControl() != searchFieldName && flowchart.SelectedBlocks.Count > 0)
                {
                    DeselectAll(flowchart);
                    Event.current.Use();
                }
            }

            DrawFlowchartView(flowchart);
            DrawOverlay(flowchart);

            // Handle selection box events after block and overlay events
            HandleSelectionBox(flowchart);

            ValidateCommands(flowchart);
            ExecuteCommands(flowchart);

            if (forceRepaintCount > 0)
            {
                // Redraw on next frame to get crisp refresh rate
                Repaint();
            }
        }

        protected virtual void DrawOverlay(Flowchart flowchart)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            GUILayout.Space(2);

            if (GUILayout.Button(new GUIContent(addTexture, "Add a new block"), EditorStyles.toolbarButton))
            {
                Vector2 newNodePosition = new Vector2(50 / flowchart.Zoom - flowchart.ScrollPos.x, 
                                                    50 / flowchart.Zoom - flowchart.ScrollPos.y);
                CreateBlock(flowchart, newNodePosition);
            }
            
            // Separator
            GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(8));

            GUILayout.Label("Scale", EditorStyles.miniLabel);
            var newZoom = GUILayout.HorizontalSlider(
                flowchart.Zoom, minZoomValue, maxZoomValue, GUILayout.MinWidth(40), GUILayout.MaxWidth(100)
            );
            GUILayout.Label(flowchart.Zoom.ToString("0.0#x"), EditorStyles.miniLabel, GUILayout.Width(30));

            if (newZoom != flowchart.Zoom)
            {
                DoZoom(flowchart, newZoom - flowchart.Zoom, Vector2.one * 0.5f);
            }

            if (GUILayout.Button("Center", EditorStyles.toolbarButton))
            {
                flowchart.ScrollPos = flowchart.CenterPosition;
            }

            GUILayout.FlexibleSpace();

            var blocks = flowchart.GetComponents<Block>();

            // Intercept mouse and keyboard events before search field uses them
            if (GUI.GetNameOfFocusedControl() == searchFieldName)
            {
                if (Event.current.type == EventType.KeyDown)
                {
                    var centerBlock = false;
                    var selectBlock = false;
                    var closePopup = false;
                    var useEvent = false;

                    switch (Event.current.keyCode)
                    {
                    case KeyCode.DownArrow:
                        ++blockPopupSelection;
                        centerBlock = true;
                        useEvent = true;
                        break;

                    case KeyCode.UpArrow:
                        --blockPopupSelection;
                        centerBlock = true;
                        useEvent = true;
                        break;

                    case KeyCode.Return:
                        centerBlock = true;
                        selectBlock = true;
                        closePopup = true;
                        useEvent = true;
                        break;
                        
                    case KeyCode.Escape:
                        closePopup = true;
                        useEvent = true;
                        break;
                    }

                    blockPopupSelection = Mathf.Clamp(blockPopupSelection, 0, filteredBlocks.Length - 1);

                    if (centerBlock && filteredBlocks.Length > 0)
                    {
                        var block = filteredBlocks[blockPopupSelection];
                        CenterBlock(flowchart, block);

                        if (selectBlock)
                        {
                            SelectBlock(flowchart, block);
                        }
                    }

                    if (closePopup)
                    {
                        CloseBlockPopup();
                    }

                    if (useEvent)
                    {
                        Event.current.Use();
                    }
                }
            }
            else if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                searchRect.Contains(Event.current.mousePosition))
            {
                blockPopupSelection = 0;
            }

            GUI.SetNextControlName(searchFieldName);
            var newString = EditorGUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(150));
            if (newString != searchString)
            {
                searchString = newString;
            }

            // Update this every frame in case of redo/undo while popup is open
            filteredBlocks = blocks.Where(block => block.BlockName.ToLower().Contains(searchString.ToLower())).ToArray();
            blockPopupSelection = Mathf.Clamp(blockPopupSelection, 0, filteredBlocks.Length - 1);

            if (Event.current.type == EventType.Repaint)
            {
                searchRect = GUILayoutUtility.GetLastRect();
                popupRect = searchRect;
                popupRect.width += 12;
                popupRect.y += popupRect.height;
                popupRect.height = Mathf.Min(filteredBlocks.Length * 16, position.height - 22);
            }

            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                CloseBlockPopup();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Label(flowchart.name, EditorStyles.whiteBoldLabel);
            
            GUILayout.Space(2);
            
            if (flowchart.Description.Length > 0)
            {
                GUILayout.Label(flowchart.Description, EditorStyles.helpBox);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(440));
        
            GUILayout.FlexibleSpace();

            var rawMousePosition = Event.current.mousePosition; // mouse position outside of scrollview to test against toolbar rect

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
                Rect toolbarRect = new Rect(0, 0, position.width, 18);
                mouseOverPopup = (GUI.GetNameOfFocusedControl() == searchFieldName && popupRect.Contains(rawMousePosition));
                mouseOverVariables = variableWindowRect.Contains(Event.current.mousePosition) ||
                                     toolbarRect.Contains(rawMousePosition) || mouseOverPopup; 
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            // Draw block search popup on top of other controls
            if (GUI.GetNameOfFocusedControl() == searchFieldName && filteredBlocks.Length > 0)
            {
                DrawBlockPopup(flowchart);
            }
        }

        protected virtual void DrawBlockPopup(Flowchart flowchart)
        {            
            blockPopupSelection = Mathf.Clamp(blockPopupSelection, 0, filteredBlocks.Length - 1);

            GUI.Box(popupRect, "", GUI.skin.FindStyle("sv_iconselector_back"));

            if (Event.current.type == EventType.MouseMove)
            {
                if (popupRect.Contains(Event.current.mousePosition))
                {
                    var relativeY = Event.current.mousePosition.y - popupRect.yMin + popupScroll.y;
                    blockPopupSelection = (int) (relativeY / 16);
                }

                Event.current.Use();
            }

            GUILayout.BeginArea(popupRect);
            popupScroll = EditorGUILayout.BeginScrollView(popupScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

            var normalStyle = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
            normalStyle.padding = new RectOffset(8, 0, 0, 0);
            normalStyle.imagePosition = ImagePosition.ImageLeft;
            var selectedStyle = new GUIStyle(normalStyle);
            selectedStyle.normal = selectedStyle.hover;
            normalStyle.hover = normalStyle.normal;

            for (int i = 0; i < filteredBlocks.Length; ++i)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(16));

                var block = filteredBlocks[i];
                var style = i == blockPopupSelection ? selectedStyle : normalStyle;

                GUI.contentColor = GetBlockGraphics(block).tint;

                var buttonPressed = false;
                if (GUILayout.Button(FungusEditorResources.BulletPoint, style, GUILayout.Width(16)))
                {
                    buttonPressed = true;
                }

                GUI.contentColor = Color.white;

                if (GUILayout.Button(block.BlockName, style))
                {
                    buttonPressed = true;
                }

                if (buttonPressed)
                {
                    CenterBlock(flowchart, block);
                    SelectBlock(flowchart, block);
                    CloseBlockPopup();
                }

                EditorGUILayout.EndHorizontal();       
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
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

            // Draw background color / drop shadow
            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Graphs.Styles.graphBackground.Draw(
                    new Rect(0, 17, position.width, position.height - 17), false, false, false, false
                );            
            }
            // Calc rect for script view
            Rect scriptViewRect = new Rect(0, 0, this.position.width / flowchart.Zoom, this.position.height / flowchart.Zoom);

            // Update right click start outside of EditorZoomArea
            if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    rightClickDown = Event.current.mousePosition;
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    if (Vector2.Distance(rightClickDown, Event.current.mousePosition) > rightClickTolerance)
                    {
                        rightClickDown = -Vector2.one;
                    }
                }
            }

            EditorZoomArea.Begin(flowchart.Zoom, scriptViewRect);

            if (Event.current.type == EventType.Repaint)
            {
                DrawGrid(flowchart);
            }
            
            // The center of the Flowchart depends on the block positions and window dimensions, so we calculate it 
            // here in the FlowchartWindow class and store it on the Flowchart object for use later.
            if (flowchart != null && blocks.Length > 0)
            {
                CalcFlowchartCenter(flowchart, blocks);
            }

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

                        if (GUI.Button(rect, FungusEditorResources.PlayBig, new GUIStyle()))
                        {
                            SelectBlock(flowchart, b);
                        }

                        GUI.color = Color.white;
                    }
                }
            }

            PanAndZoom(flowchart);

            EditorZoomArea.End();
            
            // Handle right click up outside of EditorZoomArea to avoid strange offsets
            if (Event.current.type == EventType.MouseUp && Event.current.button == 1 &&
                rightClickDown != -Vector2.one && !mouseOverVariables)
            {
                var menu = new GenericMenu();
                var mousePosition = rightClickDown;

                Block hitBlock = null;
                foreach (var block in blocks)
                {
                    if (block._NodeRect.Contains(rightClickDown / flowchart.Zoom - flowchart.ScrollPos))
                    {
                        hitBlock = block;
                        break;
                    }
                }
                // Clicked on a block
                if (hitBlock != null)
                {
                    flowchart.AddSelectedBlock(hitBlock);

                    // Use a copy because flowchart.SelectedBlocks gets modified
                    var blockList = new List<Block>(flowchart.SelectedBlocks);
                    menu.AddItem(new GUIContent ("Copy"), false, () => Copy(flowchart));
                    menu.AddItem(new GUIContent ("Cut"), false, () => Cut(flowchart));
                    menu.AddItem(new GUIContent ("Duplicate"), false, () => Duplicate(flowchart));
                    menu.AddItem(new GUIContent ("Delete"), false, DeleteBlocks, blockList);
                }
                // Clicked on empty space in grid
                else
                {
                    DeselectAll(flowchart);

                    menu.AddItem(new GUIContent("Add Block"), false, () => CreateBlock(flowchart, mousePosition / flowchart.Zoom - flowchart.ScrollPos));

                    if (copyList.Count > 0)
                    {
                        menu.AddItem(new GUIContent("Paste"), false, () => Paste(flowchart, mousePosition));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    }
                }

                var menuRect = new Rect();
                menuRect.position = new Vector2(mousePosition.x, mousePosition.y - 12f);
                menu.DropDown(menuRect);
                Event.current.Use();               
            }

            // If event has yet to be used and user isn't multiselecting or panning, clear selection
            bool validModifier = Event.current.alt || GetAppendModifierDown();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !validModifier)
            {
                DeselectAll(flowchart);
            }

            // Draw selection box
            if (startSelectionBoxPosition.x >= 0 && startSelectionBoxPosition.y >= 0)
            {
                GUI.Box(selectionBox, "", (GUIStyle) "SelectionRect");
                forceRepaintCount = 6;
            }
        }

        public virtual Vector2 GetBlockCenter(Flowchart flowchart, Block[] blocks)
        {
            if (blocks.Length == 0)
            {
                return Vector2.zero;
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

            return (min + max) * 0.5f;
        }

        public virtual void CalcFlowchartCenter(Flowchart flowchart, Block[] blocks)
        {
            var center = -GetBlockCenter(flowchart, blocks);
            center.x += position.width * 0.5f / flowchart.Zoom;
            center.y += position.height * 0.5f / flowchart.Zoom;

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
                    if (!mouseOverVariables)
                    {
                        startSelectionBoxPosition = Event.current.mousePosition;
                        mouseDownSelectionState = new List<Block>(flowchart.SelectedBlocks);
                        Event.current.Use();
                    }
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
                    selectionBox.position = -Vector2.one;
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
            if (Event.current.type == EventType.ScrollWheel && !mouseOverPopup)
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
                Vector2 zoomCenter;
                zoomCenter.x = Event.current.mousePosition.x / position.width;
                zoomCenter.y = Event.current.mousePosition.y / position.height;
                zoomCenter *= flowchart.Zoom;

                DoZoom(flowchart, -Event.current.delta.y * 0.01f, zoomCenter);
            }
        }

        protected virtual void DoZoom(Flowchart flowchart, float delta, Vector2 center)
        {
            var prevZoom = flowchart.Zoom;
            flowchart.Zoom += delta;
            flowchart.Zoom = Mathf.Clamp(flowchart.Zoom, minZoomValue, maxZoomValue);
            var deltaSize = position.size / prevZoom - position.size / flowchart.Zoom;
            var offset = -Vector2.Scale(deltaSize, center);
            flowchart.ScrollPos += offset;
            forceRepaintCount = 6;
        }

        protected virtual void DrawGrid(Flowchart flowchart)
        {
            float width = this.position.width / flowchart.Zoom;
            float height = this.position.height / flowchart.Zoom;

            Handles.color = gridLineColor;

            float gridSize = 128f;
            
            float x = flowchart.ScrollPos.x % gridSize;
            while (x < width)
            {
                Handles.DrawLine(new Vector2(x, 0), new Vector2(x, height));
                x += gridSize;
            }
            
            float y = (flowchart.ScrollPos.y % gridSize);
            while (y < height)
            {
                if (y >= 0)
                {
                    Handles.DrawLine(new Vector2(0, y), new Vector2(width, y));
                }
                y += gridSize;
            }

            Handles.color = Color.white;
        }

        protected virtual void SelectBlock(Flowchart flowchart, Block block)
        {
            // Select the block and also select currently executing command
            flowchart.SelectedBlock = block;
            SetBlockForInspector(flowchart, block);
        }

        protected virtual void DeselectAll(Flowchart flowchart)
        {
            Undo.RecordObject(flowchart, "Deselect");
            flowchart.ClearSelectedCommands();
            flowchart.ClearSelectedBlocks();
            Selection.activeGameObject = flowchart.gameObject;
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
            var graphics = GetBlockGraphics(block);

            // Make sure node is wide enough to fit the node name text
            var n = block as Node;
            float width = nodeStyleCopy.CalcSize(new GUIContent(block.BlockName)).x;
            Rect tempRect = n._NodeRect;
            tempRect.width = Mathf.Max (n._NodeRect.width, width);
            n._NodeRect = tempRect;

            Rect boxRect = GUILayoutUtility.GetRect(n._NodeRect.width, n._NodeRect.height);

            // Draw untinted highlight
            if (selected)
            {
                GUI.backgroundColor = Color.white;
                nodeStyleCopy.normal.background = graphics.onTexture;
                GUI.Box(boxRect, "", nodeStyleCopy);
            }

            // Draw tinted block; ensure text is readable
            var brightness = graphics.tint.r * 0.3 + graphics.tint.g * 0.59 + graphics.tint.b * 0.11;
            nodeStyleCopy.normal.textColor = brightness >= 0.5 ? Color.black : Color.white;

            if (GUI.GetNameOfFocusedControl() == searchFieldName && !filteredBlocks.Contains(block))
            {
                graphics.tint.a *= 0.2f;
            }

            nodeStyleCopy.normal.background = graphics.offTexture;
            GUI.backgroundColor = graphics.tint;
            GUI.Box(boxRect, block.BlockName, nodeStyleCopy);

            GUI.backgroundColor = Color.white;

            if (block.Description.Length > 0)
            {
                GUIStyle descriptionStyle = new GUIStyle(EditorStyles.helpBox);
                descriptionStyle.wordWrap = true;
                GUILayout.Label(block.Description, descriptionStyle);
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
                new Vector2(rectA.xMin, rectA.center.y),
                new Vector2(rectA.xMin + rectA.width / 2, rectA.yMin),
                new Vector2(rectA.xMin + rectA.width / 2, rectA.yMax),
                new Vector2(rectA.xMax, rectA.center.y) 
            };

            Vector2[] pointsB = new Vector2[] {
                new Vector2(rectB.xMin, rectB.center.y),
                new Vector2(rectB.xMin + rectB.width / 2, rectB.yMin),
                new Vector2(rectB.xMin + rectB.width / 2, rectB.yMax),
                new Vector2(rectB.xMax, rectB.center.y)
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

            Color color = connectionColor;
            if (highlight)
            {
                color = Color.green;
            }

            Handles.color = color;

            // Place control based on distance between points
            // Weight the min component more so things don't get overly curvy
            var diff = pointA - pointB;
            diff.x = Mathf.Abs(diff.x);
            diff.y = Mathf.Abs(diff.y);
            var min = Mathf.Min(diff.x, diff.y);
            var max = Mathf.Max(diff.x, diff.y);
            var mod = min * 0.75f + max * 0.25f;

            // Draw bezier curve connecting blocks
            var directionA = (rectA.center - pointA).normalized;
            var directionB = (rectB.center - pointB).normalized;
            var controlA = pointA - directionA * mod * 0.67f;
            var controlB = pointB - directionB * mod * 0.67f;            
            Handles.DrawBezier(pointA, pointB, controlA, controlB, color, null, 3f);

            // Draw arrow on curve
            var point = GetPointOnCurve(pointA, controlA, pointB, controlB, 0.7f);
            var direction = (GetPointOnCurve(pointA, controlA, pointB, controlB, 0.6f) - point).normalized;
            var perp = new Vector2(direction.y, -direction.x);
            Handles.DrawAAConvexPolygon(
                point, point + direction * 10 + perp * 5, point + direction * 10 - perp * 5
            );

            var connectionPointA = pointA + directionA * 4f;
            var connectionRectA = new Rect(connectionPointA.x - 4f, connectionPointA.y - 4f, 8f, 8f);
            var connectionPointB = pointB + directionB * 4f;
            var connectionRectB = new Rect(connectionPointB.x - 4f, connectionPointB.y - 4f, 8f, 8f);

            GUI.DrawTexture(connectionRectA, connectionPointTexture, ScaleMode.ScaleToFit);
            GUI.DrawTexture(connectionRectB, connectionPointTexture, ScaleMode.ScaleToFit);

            Handles.color = Color.white;
        }

        private static Vector2 GetPointOnCurve(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
        {
            float rt = 1 - t;
            float rtt = rt * t;
            return rt * rt * rt * s + 3 * rt * rtt * st + 3 * rtt * t * et + t * t * t * e;
        }

        public static void DeleteBlocks(object obj)
        {
            var blocks = obj as List<Block>;
            blocks.ForEach(block => FlowchartWindow.deleteList.Add(block));
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

        protected virtual void Copy(Flowchart flowchart)
        {
            copyList.Clear();

            foreach (var block in flowchart.SelectedBlocks)
            {
                copyList.Add(new BlockCopy(block));
            }
        }

        protected virtual void Cut(Flowchart flowchart)
        {
            Copy(flowchart);
            Undo.RecordObject(flowchart, "Cut");
            DeleteBlocks(flowchart.SelectedBlocks);
        }

        // Center is position in unscaled window space
        protected virtual void Paste(Flowchart flowchart, Vector2 center, bool relative = false)
        {
            Undo.RecordObject(flowchart, "Deselect");
            DeselectAll(flowchart);

            var pasteList = new List<Block>();

            foreach (var copy in copyList)
            {
                pasteList.Add(copy.PasteBlock(flowchart));
            }

            var copiedCenter = GetBlockCenter(flowchart, pasteList.ToArray()) + flowchart.ScrollPos;
            var delta = relative ? center : (center / flowchart.Zoom - copiedCenter);
            
            foreach (var block in pasteList)
            {
                var tempRect = block._NodeRect;
                tempRect.position += delta;
                block._NodeRect = tempRect;
            }
        }

        protected virtual void Duplicate(Flowchart flowchart)
        {
            var tempCopyList = new List<BlockCopy>(copyList);
            Copy(flowchart);
            Paste(flowchart, new Vector2(20, 0), true);
            copyList = tempCopyList;
        }

        protected virtual void ValidateCommands(Flowchart flowchart)
        {
            if (Event.current.type == EventType.ValidateCommand)
            {
                var c = Event.current.commandName;
                if (c == "Copy" || c == "Cut" || c == "Delete" || c == "Duplicate")
                {
                    if (flowchart.SelectedBlocks.Count > 0)
                    {
                        Event.current.Use();
                    }
                }
                else if (c == "Paste")
                {
                    if (copyList.Count > 0)
                    {
                        Event.current.Use();
                    }
                }
                else if (c == "SelectAll" || c == "Find")
                {
                    Event.current.Use();
                }
            }
        }

        protected virtual void ExecuteCommands(Flowchart flowchart)
        {
            if (Event.current.type == EventType.ExecuteCommand)
            {
                switch (Event.current.commandName)
                {
                case "Copy":
                    Copy(flowchart);
                    Event.current.Use();
                    break;
                
                case "Cut":
                    Cut(flowchart);
                    Event.current.Use();
                    break;

                case "Paste":
                    Paste(flowchart, position.center - position.position);
                    Event.current.Use();
                    break;

                case "Delete":
                    DeleteBlocks(flowchart.SelectedBlocks);
                    Event.current.Use();
                    break;

                case "Duplicate":
                    Duplicate(flowchart);
                    Event.current.Use();
                    break;

                case "SelectAll":
                    Undo.RecordObject(flowchart, "Selection");
                    flowchart.ClearSelectedBlocks();
                    foreach (var block in flowchart.GetComponents<Block>())
                    {
                        flowchart.AddSelectedBlock(block);
                    }
                    Event.current.Use();
                    break;

                case "Find":
                    blockPopupSelection = 0;
                    EditorGUI.FocusTextInControl(searchFieldName);
                    Event.current.Use();
                    break;
                }
            }
        }

        protected virtual void CenterBlock(Flowchart flowchart, Block block)
        {
            if (flowchart.Zoom < 1)
            {
                DoZoom(flowchart, 1 - flowchart.Zoom, Vector2.one * 0.5f);
            }

            flowchart.ScrollPos = -block._NodeRect.center + position.size * 0.5f / flowchart.Zoom;
        }

        protected virtual void CloseBlockPopup()
        {
            GUIUtility.keyboardControl = 0;
            searchString = string.Empty;
        }

        protected virtual BlockGraphics GetBlockGraphics(Block block)
        {
            var graphics = new BlockGraphics();

            Color defaultTint;
            if (block._EventHandler != null)
            {
                graphics.offTexture = FungusEditorResources.EventNodeOff;
                graphics.onTexture = FungusEditorResources.EventNodeOn;
                defaultTint = FungusConstants.DefaultEventBlockTint;
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
                    graphics.offTexture = FungusEditorResources.ChoiceNodeOff;
                    graphics.onTexture = FungusEditorResources.ChoiceNodeOn;
                    defaultTint = FungusConstants.DefaultChoiceBlockTint;
                }
                else
                {
                    graphics.offTexture = FungusEditorResources.ProcessNodeOff;
                    graphics.onTexture = FungusEditorResources.ProcessNodeOn;
                    defaultTint = FungusConstants.DefaultProcessBlockTint;
                }
            }

            graphics.tint = block.UseCustomTint ? block.Tint : defaultTint;

            return graphics;
        }
    }
}