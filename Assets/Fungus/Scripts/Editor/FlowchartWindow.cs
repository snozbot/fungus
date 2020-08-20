// This code is part of the Fungus library (https://github.com/snozbot/fungus)
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
    public class FlowchartWindow : EventWindow
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

            internal Block PasteBlock(FlowchartWindow flowWind ,Flowchart flowchart)
            {
                var newBlock = flowWind.CreateBlock(flowchart, Vector2.zero);

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

        /// <summary>
        /// Helper class to maintain list of blocks that are currently executing when the game is running in editor
        /// </summary>
        protected class ExecutingBlocks
        {
            internal List<Block> areExecuting = new List<Block>(),
                                 wereExecuting = new List<Block>(),
                                 workspace = new List<Block>();

            internal bool isChangeDetected { get; set; }

            private float lastFade;

            internal void ProcessAllBlocks(Block[] blocks)
            {
                isChangeDetected = false;
                workspace.Clear();
                //cache these once as they can end up being called thousands of times per frame otherwise
                var curRealTime = Time.realtimeSinceStartup;
                var fadeTimer = curRealTime + FungusConstants.ExecutingIconFadeTime;
                for (int i = 0; i < blocks.Length; ++i)
                {
                    var b = blocks[i];
                    var bIsExec = b.IsExecuting();
                    if (bIsExec)
                    {
                        b.ExecutingIconTimer = fadeTimer;
                        b.ActiveCommand.ExecutingIconTimer = fadeTimer;
                        workspace.Add(b);
                    }
                }

                if(areExecuting.Count != workspace.Count || !WorkspaceMatchesExeucting())
                {
                    wereExecuting.Clear();
                    wereExecuting.AddRange(areExecuting);
                    areExecuting.Clear();
                    areExecuting.AddRange(workspace);
                    isChangeDetected = true;
                    lastFade = fadeTimer;
                }
            }

            internal bool WorkspaceMatchesExeucting()
            {
                for (int i = 0; i < areExecuting.Count; i++)
                {
                    if (areExecuting[i] != workspace[i])
                        return false;
                }
                return true;
            }

            internal bool IsAnimFadeoutNeed()
            {
                return (lastFade - Time.realtimeSinceStartup) >= 0;
            }

            internal void ClearAll()
            {
                areExecuting.Clear();
                wereExecuting.Clear();
                workspace.Clear();
                isChangeDetected = true;
                lastFade = 0;
            }
        }



        public const float GridLineSpacingSize = 120;
        public const float GridObjectSnap = 20;
        public const float DefaultBlockHeight = 40;
        public const float BlockMinWidth = 60;
        public const float BlockMaxWidth = 240;
        public const float MinZoomValue = 0.25f;
        public const float MaxZoomValue = 1f;
        public const int HorizontalPad = 20;
        public const int VerticalPad = 5;
        //defines the distance between a down and up for a right click to be a click rather than a drag
        public const float RightClickTolerance = 5f;   
        public const string SearchFieldName = "search";


        protected readonly Color connectionColor = new Color(0.65f, 0.65f, 0.65f, 1.0f);


        protected List<BlockCopy> copyList = new List<BlockCopy>();
        public static List<Block> deleteList = new List<Block>();
        protected Vector2 startDragPosition;
        protected GUIStyle nodeStyle, descriptionStyle, handlerStyle, blockSearchPopupNormalStyle, blockSearchPopupSelectedStyle;
        protected static BlockInspector blockInspector;
        protected int forceRepaintCount;
        protected Texture2D addTexture;
        protected GUIContent addButtonContent;
        protected Texture2D connectionPointTexture;
        protected Rect selectionBox;
        protected Vector2 startSelectionBoxPosition = -Vector2.one;
        protected List<Block> mouseDownSelectionState = new List<Block>();
        protected Color gridLineColor = Color.black;
        // Context Click occurs on MouseDown which interferes with panning
        // Track right click positions manually to show menus on MouseUp
        protected Vector2 rightClickDown = -Vector2.one;
        private string searchString = string.Empty;
        protected Rect searchRect;
        protected Rect popupRect;
        protected List<Block> filteredBlocks = new List<Block>();
        protected int blockPopupSelection = -1;
        protected Vector2 popupScroll;
        protected Flowchart flowchart, prevFlowchart;
        protected int prevVarCount;
        protected Block[] blocks = new Block[0];
        protected Block dragBlock;
        protected bool hasDraggedSelected = false;
        protected static FungusState fungusState;

        static protected VariableListAdaptor variableListAdaptor;

        private bool filterStale = true;
        private bool wasControl;
        private ExecutingBlocks executingBlocks = new ExecutingBlocks();

        private GUIStyle toolbarSeachTextFieldStyle;
        protected GUIStyle ToolbarSeachTextFieldStyle
        {
            get
            {
                if(toolbarSeachTextFieldStyle == null)
                    toolbarSeachTextFieldStyle = GUI.skin.FindStyle("ToolbarSeachTextField");

                return toolbarSeachTextFieldStyle;
            }
        }
        private GUIStyle toolbarSeachCancelButtonStyle;
        private bool didDoubleClick;

        protected GUIStyle ToolbarSeachCancelButtonStyle
        {
            get
            {
                if(toolbarSeachCancelButtonStyle == null)
                    toolbarSeachCancelButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");

                return toolbarSeachCancelButtonStyle;
            }
        }

        [MenuItem("Tools/Fungus/Flowchart Window")]
        static void Init()
        {
            GetWindow(typeof(FlowchartWindow), false, "Flowchart");
        }

        protected virtual void OnEnable()
        {
            addTexture = FungusEditorResources.AddSmall;
            addButtonContent = new GUIContent(addTexture, "Add a new block");
            connectionPointTexture = FungusEditorResources.ConnectionPoint;
            gridLineColor.a = EditorGUIUtility.isProSkin ? 0.5f : 0.25f;

            copyList.Clear();

            wantsMouseMove = true; // For hover selection in block search popup  

            UpdateBlockCollection();


            EditorApplication.update += OnEditorUpdate;
            Undo.undoRedoPerformed += Undo_ForceRepaint;

#if UNITY_2017_4_OR_NEWER
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif
        }

        //cache styles here, rather than duping them for every block we may ever draw,
        // does mean any modifications made to the style when drawing must be undone as you go
        protected void InitStyles()
        {
            if (nodeStyle == null)
            {
                nodeStyle = new GUIStyle();
                // All block nodes use the same GUIStyle, but with a different background
                nodeStyle.border = new RectOffset(HorizontalPad, HorizontalPad, VerticalPad, VerticalPad);
                nodeStyle.padding = nodeStyle.border;
                nodeStyle.contentOffset = Vector2.zero;
                nodeStyle.alignment = TextAnchor.MiddleCenter;
                nodeStyle.wordWrap = true;
            }

            if (EditorStyles.helpBox != null && descriptionStyle == null)
            {
                descriptionStyle = new GUIStyle(EditorStyles.helpBox);
                descriptionStyle.wordWrap = true;
            }

            if (EditorStyles.whiteLabel != null && handlerStyle == null)
            {
                handlerStyle = new GUIStyle(EditorStyles.label);
                handlerStyle.wordWrap = true;
                handlerStyle.margin.top = 0;
                handlerStyle.margin.bottom = 0;
                handlerStyle.alignment = TextAnchor.MiddleCenter;
            }

            if(blockSearchPopupNormalStyle == null || blockSearchPopupSelectedStyle == null)
            {
                blockSearchPopupNormalStyle = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
                blockSearchPopupNormalStyle.padding = new RectOffset(8, 0, 0, 0);
                blockSearchPopupNormalStyle.imagePosition = ImagePosition.ImageLeft;
                blockSearchPopupSelectedStyle = new GUIStyle(blockSearchPopupNormalStyle);
                blockSearchPopupSelectedStyle.normal = blockSearchPopupSelectedStyle.hover;
                blockSearchPopupNormalStyle.hover = blockSearchPopupNormalStyle.normal;
            }
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            Undo.undoRedoPerformed -= Undo_ForceRepaint;
#if UNITY_2017_4_OR_NEWER
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
#endif
        }

#if UNITY_2017_4_OR_NEWER
        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            //force null so it can refresh context on the other side of the context
            flowchart = null;
            prevFlowchart = null;
            blockInspector = null;
        }
#endif

        protected void Undo_ForceRepaint()
        {
            //an undo redo may have added or removed blocks so
            UpdateBlockCollection();
            flowchart.UpdateSelectedCache();
            Repaint();
        }

        protected void OnEditorUpdate()
        {
            HandleFlowchartSelectionChange();

            if(flowchart != null)
            {
                var varcount = flowchart.VariableCount;
                if (varcount != prevVarCount)
                {
                    prevVarCount = varcount;
                    Repaint();
                }

                if(flowchart.SelectedCommandsStale)
                {
                    flowchart.SelectedCommandsStale = false;
                    Repaint();
                }

                if(CommandEditor.SelectedCommandDataStale)
                {
                    CommandEditor.SelectedCommandDataStale = false;
                    Repaint();
                }

                if(BlockEditor.SelectedBlockDataStale)
                {
                    BlockEditor.SelectedBlockDataStale = false;
                    Repaint();
                }

                if (FlowchartEditor.FlowchartDataStale)
                {
                    FlowchartEditor.FlowchartDataStale = false;
                    Repaint();
                }
            }
            else
            {
                prevVarCount = 0;
            }

            if (Application.isPlaying)
            {
                executingBlocks.ProcessAllBlocks(blocks);
                if (executingBlocks.isChangeDetected || executingBlocks.IsAnimFadeoutNeed())
                    Repaint();
            }
        }

        protected void UpdateBlockCollection()
        {
            flowchart = GetFlowchart();
            if (flowchart == null)
            {
                blocks = new Block[0];
                filteredBlocks = new List<Block>();
            }
            else
            {
                blocks = flowchart.GetComponents<Block>();
            }
            filterStale = true;
            UpdateFilteredBlocks();
        }

        protected virtual void OnInspectorUpdate()
        {
            if (HandleFlowchartSelectionChange()) return;

            // Ensure the Block Inspector is always showing the currently selected block
            var flowchart = GetFlowchart();
            if (flowchart == null || AnyNullBLocks())
            {
                UpdateBlockCollection();
                Repaint();
                return;
            }

            if (Selection.activeGameObject == null &&
                flowchart.SelectedBlock != null)
            {
                if (blockInspector == null)
                {
                    ShowBlockInspector(flowchart);
                }
                blockInspector.block = (Block)flowchart.SelectedBlock;
            }

            if (forceRepaintCount != 0)
            {
                forceRepaintCount--;
                forceRepaintCount = Math.Max(0, forceRepaintCount);

                Repaint();
            }
        }

        private bool AnyNullBLocks()
        {
            if (blocks == null)
                return true;

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null)
                    return true;
            }

            return false;
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
            if (fungusState == null)
            {
                fungusState = GameObject.FindObjectOfType<FungusState>();
                if (fungusState == null)
                {
                    GameObject go = new GameObject("_FungusState");
                    go.hideFlags = HideFlags.HideInHierarchy;
                    fungusState = go.AddComponent<FungusState>();
                }
            }

            if (Selection.activeGameObject != null)
            {
                var fs = Selection.activeGameObject.GetComponent<Flowchart>();
                if (fs != null)
                {
                    fungusState.SelectedFlowchart = fs;
                }
            }

            if (fungusState.SelectedFlowchart == null)
            {
                variableListAdaptor = null;
            }
            else if (variableListAdaptor == null || variableListAdaptor.TargetFlowchart != fungusState.SelectedFlowchart)
            {
                var fsSO = new SerializedObject(fungusState.SelectedFlowchart);
                variableListAdaptor = new VariableListAdaptor(fsSO.FindProperty("variables"), fungusState.SelectedFlowchart);
            }

            return fungusState.SelectedFlowchart;
        }

        protected void UpdateFilteredBlocks()
        {
            if (filterStale)
            {
                filterStale = false;
                //reset all
                for (int i = 0; filteredBlocks != null && i < filteredBlocks.Count; i++)
                {
                    if (filteredBlocks[i] != null)
                    {
                        filteredBlocks[i].FilterState = Block.FilteredState.None;
                    }
                }

                var nullCount = filteredBlocks.Count(x => x == null);
                if (nullCount > 0 && nullCount != filteredBlocks.Count)
                {
                    Debug.LogWarning("Null block found in filteredBlocks. May be a symptom of an underlying issue");
                }

                filteredBlocks.Clear();
                
                for (int i = 0; i < blocks.Length; i++)
                {
                    var item = blocks[i];
                    if (item != null)
                    {
                        if(IsBlockNameMatch(item))
                        {
                            filteredBlocks.Add(item);
                            item.FilterState = Block.FilteredState.Full;
                        }
                        else if (IsCommandContentMatch(item))
                        {
                            filteredBlocks.Add(item);
                            item.FilterState = Block.FilteredState.Partial;
                        }
                    }
                }

                blockPopupSelection = Mathf.Clamp(blockPopupSelection, 0, filteredBlocks.Count - 1);
            }
        }

        private bool IsBlockNameMatch(Block block)
        {
            return block.BlockName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsCommandContentMatch(Block block)
        {
            try
            {
                return block.CommandList.Any(command => command.GetSearchableContent().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            catch (Exception)
            {
                return false;
            }           
        }

        protected virtual void HandleEarlyEvents(Event e) 
        {
            switch (e.type)
            {
            case EventType.MouseDown:
                // Clear search filter focus
                if (!searchRect.Contains(e.mousePosition) && !popupRect.Contains(e.mousePosition))
                {
                    CloseBlockPopup();
                }

                if (e.button == 0 && searchRect.Contains(e.mousePosition))
                {
                    blockPopupSelection = 0;
                    popupScroll = Vector2.zero;
                }

                rightClickDown = -Vector2.one;
                break;

            case EventType.KeyDown:
                if (GUI.GetNameOfFocusedControl() == SearchFieldName)
                {
                    var centerBlock = false;
                    var selectBlock = false;
                    var closePopup = false;
                    var useEvent = false;

                    switch (e.keyCode)
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

                    blockPopupSelection = Mathf.Clamp(blockPopupSelection, 0, filteredBlocks.Count - 1);

                    if (centerBlock && filteredBlocks.Count > 0)
                    {
                        var block = filteredBlocks[blockPopupSelection];
                        CenterBlock(block);

                        if (selectBlock)
                        {
                            SelectBlock(block);
                        }
                    }

                    if (closePopup)
                    {
                        CloseBlockPopup();
                    }

                    if (useEvent)
                    {
                        e.Use();
                    }
                }
                else if (e.keyCode == KeyCode.Escape)
                {
                    DeselectAll();
                    e.Use();
                }
                else if (e.control && !wasControl)
                {
                    StartControlSelection();
                    Repaint();
                    wasControl = true;
                }
                break;
                case EventType.KeyUp:
                if (!e.control && wasControl)
                {
                    wasControl = false;
                    EndControlSelection();
                    Repaint();
                }
                break;
            }
        }

        private void StartControlSelection()
        {
            mouseDownSelectionState.AddRange(flowchart.SelectedBlocks);
            flowchart.ClearSelectedBlocks();
            for (int i = 0; i < mouseDownSelectionState.Count; i++)
            {
                if (mouseDownSelectionState[i] != null)
                {
                    mouseDownSelectionState[i].IsControlSelected = true;
                }
                else
                {
                    Debug.LogWarning("Null block found in mouseDownSelectionState. May be a symptom of an underlying issue");
                }
            }
        }

        private void AddMouseDownSelectionState(Block item)
        {
            mouseDownSelectionState.Add(item);
            item.IsControlSelected = true;
        }

        private void RemoveMouseDownSelectionState(Block item)
        {
            mouseDownSelectionState.Remove(item);
            item.IsControlSelected = false;
        }

        private void EndControlSelection()
        {
            //we can be called either by mouse up with control still held or because ctrl was released
            if (GetAppendModifierDown())
            {
                //remove items selected from the mouse down and then move the mouse down to the selection
                for (int i = mouseDownSelectionState.Count - 1; i >= 0; i--)
                {
                    var item = mouseDownSelectionState[i];

                    if (item.IsSelected)
                    {
                        flowchart.DeselectBlockNoCheck(item);
                        RemoveMouseDownSelectionState(item);
                    }
                    else
                    {
                        flowchart.AddSelectedBlock(item);
                    }
                }
            }
            else
            {
                //ctrl released moves all back to selection
                for (int i = mouseDownSelectionState.Count - 1; i >= 0; i--)
                {
                    var item = mouseDownSelectionState[i];
                    flowchart.AddSelectedBlock(item);
                    RemoveMouseDownSelectionState(item);
                }
            }
        }

        internal bool HandleFlowchartSelectionChange()
        {
            flowchart = GetFlowchart();
            //target has changed, so clear the blockinspector
            if (flowchart != prevFlowchart)
            {
                blockInspector = null;
                prevFlowchart = flowchart;
                executingBlocks.ClearAll();

                //attempt to defilter previous, if due to scene change these will be null
                //  the regular filter updates will still occur within UpdateBlockCollection
                for (int i = 0; i < filteredBlocks.Count; i++)
                {
                    if (filteredBlocks[i] != null)
                    {
                        filteredBlocks[i].FilterState = Block.FilteredState.None;
                    }
                }

                UpdateBlockCollection();

                if(flowchart != null)
                    flowchart.ReverseUpdateSelectedCache();//becomes reverse restore selected cache

                Repaint();
                return true;
            }
            return false;
        }

        protected virtual void OnGUI()
        {
            // TODO: avoid calling some of these methods in OnGUI because it should be possible
            // to only call them when the window is initialized or a new flowchart is selected, etc.
            if (HandleFlowchartSelectionChange()) return;

            if (flowchart == null)
            {
                GUILayout.Label("No Flowchart scene object selected");
                return;
            }

            InitStyles();
            
            DeleteBlocks();

            UpdateFilteredBlocks();

            HandleEarlyEvents(Event.current);

            // Draw background color / drop shadow
            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Graphs.Styles.graphBackground.Draw(
                    new Rect(0, 17, position.width, position.height - 17), false, false, false, false
                );
            }

            // Draw blocks and connections
            DrawFlowchartView(Event.current);

            // Draw selection box
            if (Event.current.type == EventType.Repaint)
            {
                if (startSelectionBoxPosition.x >= 0 && startSelectionBoxPosition.y >= 0)
                {
                    GUI.Box(selectionBox, "", GUI.skin.FindStyle("SelectionRect"));
                }
            }

            // Draw toolbar, search popup, and variables window
            //  need try catch here as we are now invalidating the drawer if the target flowchart
            //      has changed which makes unity GUILayouts upset and this function appears to 
            //      actually get called partially outside our control
            try
            {
                DrawOverlay(Event.current);
            }
            catch (Exception)
            {
                //Debug.Log("Failed to draw overlay in some way");
            }

            // Handle events for custom GUI
            base.HandleEvents(Event.current);

            if (forceRepaintCount > 0)
            {
                // Redraw on next frame to get crisp refresh rate
                Repaint();
            }
        }

        protected virtual void DrawOverlay(Event e)
        {
            // Main toolbar group
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.Space(2);

                // Draw add block button
                if (GUILayout.Button(addButtonContent, EditorStyles.toolbarButton))
                {
                    DeselectAll();
                    Vector2 newNodePosition = new Vector2(
                        50 / flowchart.Zoom - flowchart.ScrollPos.x, 50 / flowchart.Zoom - flowchart.ScrollPos.y
                    );
                    CreateBlock(flowchart, newNodePosition);
                    UpdateBlockCollection();
                }

                GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(8)); // Separator

                // Draw scale bar and labels
                GUILayout.Label("Scale", EditorStyles.miniLabel);
                var newZoom = GUILayout.HorizontalSlider(
                    flowchart.Zoom, MinZoomValue, MaxZoomValue, GUILayout.MinWidth(40), GUILayout.MaxWidth(100)
                );
                GUILayout.Label(flowchart.Zoom.ToString("0.0#x"), EditorStyles.miniLabel, GUILayout.Width(30));

                if (newZoom != flowchart.Zoom)
                {
                    DoZoom(newZoom - flowchart.Zoom, Vector2.one * 0.5f);
                }

                // Draw center button
                if (GUILayout.Button("Center", EditorStyles.toolbarButton))
                {
                    CenterFlowchart();
                }

                GUILayout.FlexibleSpace();

                // Draw search bar
                GUI.SetNextControlName(SearchFieldName);
                var newString = EditorGUILayout.TextField(searchString, ToolbarSeachTextFieldStyle, GUILayout.Width(150));
                if (newString != searchString)
                {
                    searchString = newString;
                    filterStale = true;
                }
                GUI.SetNextControlName(string.Empty);

                if (e.type == EventType.Repaint)
                {
                    searchRect = GUILayoutUtility.GetLastRect();
                    popupRect = searchRect;
                    popupRect.width += 12;
                    popupRect.y += popupRect.height;
                    popupRect.height = Mathf.Min(Mathf.Max(1,filteredBlocks.Count) * 16, position.height - 22);
                }

                if (GUILayout.Button("", ToolbarSeachCancelButtonStyle))
                {
                    CloseBlockPopup();
                }

                // Eat all click events on toolbar
                if (e.type == EventType.MouseDown)
                {
                    if (e.mousePosition.y < searchRect.height)
                    {
                        e.Use();
                    }
                }
            }
            GUILayout.EndHorizontal();

            // Name and description group
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                {
                    GUILayout.Label(flowchart.name, EditorStyles.boldLabel);

                    GUILayout.Space(2);

                    if (flowchart.Description.Length > 0)
                    {
                        GUILayout.Label(flowchart.Description, EditorStyles.helpBox);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();


            // Draw block search popup on top of other controls
            if (!string.IsNullOrEmpty(searchString))
            {
                DrawBlockPopup(e);
            }

            DrawVariablesBlock(e);
        }

        protected virtual void DrawVariablesBlock(Event e)
        {
            // Variables group
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.Width(440));
                {
                    GUILayout.FlexibleSpace();

                    flowchart.VariablesScrollPos = GUILayout.BeginScrollView(flowchart.VariablesScrollPos);
                    {
                        GUILayout.Space(8);

                        EditorGUI.BeginChangeCheck();

                        if (variableListAdaptor != null)
                        {
                            if (variableListAdaptor.TargetFlowchart != null)
                            {
                                //440 - space for scrollbar
                                variableListAdaptor.DrawVarList(400);
                            }
                            else
                            {
                                variableListAdaptor = null;
                            }
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(flowchart);
                        }
                    }
                    GUILayout.EndScrollView();


                    // Eat mouse events
                    if (e.type == EventType.MouseDown)
                    {
                        Rect variableWindowRect = GUILayoutUtility.GetLastRect();
                        if (flowchart.VariablesExpanded && flowchart.Variables.Count > 0)
                        {
                            variableWindowRect.y -= 20;
                            variableWindowRect.height += 20;
                        }

                        if (variableWindowRect.Contains(e.mousePosition))
                        {
                            e.Use();
                        }
                    }
                }
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        protected virtual void DrawBlockPopup(Event e)
        {            
            blockPopupSelection = Mathf.Clamp(blockPopupSelection, 0, Mathf.Max(filteredBlocks.Count - 1,0));

            GUI.Box(popupRect, "", GUI.skin.FindStyle("sv_iconselector_back"));

            if (e.type == EventType.MouseMove)
            {
                if (popupRect.Contains(e.mousePosition))
                {
                    var relativeY = e.mousePosition.y - popupRect.yMin + popupScroll.y;
                    blockPopupSelection = (int) (relativeY / 16);
                }

                e.Use();
            }

            GUILayout.BeginArea(popupRect);
            {
                popupScroll = EditorGUILayout.BeginScrollView(popupScroll, GUIStyle.none, GUI.skin.verticalScrollbar);
                {
                    for (int i = 0; i < filteredBlocks.Count; ++i)
                    {
                        DrawBlockSearchPopUpItem(filteredBlocks[i], i == blockPopupSelection);
                    }

                    if(filteredBlocks.Count == 0)
                    {
                        DrawBlockSearchPopUpItem(null, false);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        protected void DrawBlockSearchPopUpItem(Block block, bool selected)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(16));

            var style = selected ? blockSearchPopupSelectedStyle : blockSearchPopupNormalStyle;

            GUI.contentColor = block != null ? GetBlockGraphics(block).tint : Color.white;

            var buttonPressed = false;
            if (GUILayout.Button(FungusEditorResources.BulletPoint, style, GUILayout.Width(16)))
            {
                buttonPressed = true;
            }

            GUI.contentColor = Color.white;

            if (GUILayout.Button(block != null ? block.BlockName : "No Matches", style))
            {
                buttonPressed = true;
            }

            if (buttonPressed)
            {
                CenterBlock(block);
                SelectBlock(block);
                CloseBlockPopup();
            }

            EditorGUILayout.EndHorizontal();
        }

        protected Block GetBlockAtPoint(Vector2 point)
        {
            for (int i = blocks.Length - 1; i > -1; --i)
            {
                var block = blocks[i];
                var rect = block._NodeRect;
                rect.position += flowchart.ScrollPos;

                if (rect.Contains(point / flowchart.Zoom))
                {
                    return block;
                }
            }

            return null;
        }

        protected override void OnMouseDown(Event e)
        {
            var hitBlock = GetBlockAtPoint(e.mousePosition);

            // Convert Ctrl+Left click to a right click on mac
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (e.button == MouseButton.Left &&
                    e.control)
                {
                    e.button = MouseButton.Right;
                }
            }

            switch(e.button)
            {
            case MouseButton.Left:
                if (!e.alt)
                {
                    if (hitBlock != null)
                    {
                        if(e.clickCount == 2)
                        {
                            CenterBlock(hitBlock);

                            e.Use();
                            didDoubleClick = true;
                            return;
                        }

                        startDragPosition = e.mousePosition / flowchart.Zoom - flowchart.ScrollPos;
                        Undo.RecordObject(flowchart, "Select");

                        if (GetAppendModifierDown())
                        {
                            //ctrl clicking blocks toggles between
                            if (mouseDownSelectionState.Contains(hitBlock))
                            {
                                RemoveMouseDownSelectionState(hitBlock);
                            }
                            else
                            {
                                AddMouseDownSelectionState(hitBlock);
                            }
                        }
                        else
                        {
                            if (flowchart.SelectedBlocks.Contains(hitBlock))
                            {
                                SetBlockForInspector(flowchart, hitBlock);
                            }
                            else
                            {
                                SelectBlock(hitBlock);
                            }

                            dragBlock = hitBlock;
                            hasDraggedSelected = false;
                        }

                        e.Use();
                        GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
                    }
                    else if (!(UnityEditor.Tools.current == Tool.View && UnityEditor.Tools.viewTool == ViewTool.Zoom))
                    {
                        startSelectionBoxPosition = e.mousePosition;
                        selectionBox = Rect.MinMaxRect(selectionBox.x, selectionBox.y, selectionBox.x, selectionBox.y);
                        e.Use();
                    }
                }
                break;

            case MouseButton.Right:
                rightClickDown = e.mousePosition;
                e.Use();
                break;    
            }
        }

        protected override void OnMouseDrag(Event e)
        {
            var draggingWindow = false;
            switch (e.button)
            {
            case MouseButton.Left:
                // Block dragging
                if (dragBlock != null)
                {
                    for (int i = 0; i < flowchart.SelectedBlocks.Count; ++i)
                    {
                        var block = flowchart.SelectedBlocks[i];
                        var tempRect = block._NodeRect;
                        tempRect.position += e.delta / flowchart.Zoom;
                        block._NodeRect = tempRect;
                    }

                    hasDraggedSelected = true;
                    e.Use();
                }
                // Pan tool or alt + left click
                else if (UnityEditor.Tools.current == Tool.View && UnityEditor.Tools.viewTool == ViewTool.Pan || e.alt)
                {
                    draggingWindow = true;
                }
                else if (UnityEditor.Tools.current == Tool.View && UnityEditor.Tools.viewTool == ViewTool.Zoom)
                {
                    DoZoom(-e.delta.y * 0.01f, Vector2.one * 0.5f);
                    e.Use();
                }
                // Selection box
                else if (startSelectionBoxPosition.x >= 0 && startSelectionBoxPosition.y >= 0)
                {
                    if (Mathf.Approximately(e.delta.magnitude, 0))
                        break;
                    
                    var topLeft = Vector2.Min(startSelectionBoxPosition, e.mousePosition);
                    var bottomRight = Vector2.Max(startSelectionBoxPosition, e.mousePosition);
                    selectionBox = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

                    Rect zoomSelectionBox = selectionBox;
                    zoomSelectionBox.position -= flowchart.ScrollPos * flowchart.Zoom;
                    zoomSelectionBox.position /= flowchart.Zoom;
                    zoomSelectionBox.size /= flowchart.Zoom;


                    for (int i = 0; i < blocks.Length; ++i)
                    {
                        var block = blocks[i];
                        var doesMarqueOverlap = zoomSelectionBox.Overlaps(block._NodeRect);
                        if (doesMarqueOverlap)
                        {
                            flowchart.AddSelectedBlock(block);
                        }
                        else
                        { 
                            flowchart.DeselectBlockNoCheck(block);
                        }
                    }

                    e.Use();
                }
                break;

            case MouseButton.Right:
                if (Vector2.Distance(rightClickDown, e.mousePosition) > RightClickTolerance)
                {
                    rightClickDown = -Vector2.one;
                }
                draggingWindow = true;
                break;

            case MouseButton.Middle:
                draggingWindow = true;
                break;
            }

            if (draggingWindow)
            {
                flowchart.ScrollPos += e.delta / flowchart.Zoom;
                e.Use();
            }
        }

        protected override void OnRawMouseUp(Event e)
        {
            var hitBlock = GetBlockAtPoint(e.mousePosition);

            // Convert Ctrl+Left click to a right click on mac
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (e.button == MouseButton.Left &&
                    e.control)
                {
                    e.button = MouseButton.Right;
                }
            }

            switch (e.button)
            {
            case MouseButton.Left:
                if(didDoubleClick)
                {
                    didDoubleClick = false;
                    return;
                }


                if (dragBlock != null)
                {
                    for (int i = 0; i < flowchart.SelectedBlocks.Count; ++i)
                    {
                        var block = flowchart.SelectedBlocks[i];
                        var tempRect = block._NodeRect;
                        var distance = e.mousePosition / flowchart.Zoom - flowchart.ScrollPos - startDragPosition;
                        tempRect.position -= distance;
                        block._NodeRect = tempRect;
                        Undo.RecordObject(block, "Block Position");
                        tempRect.position += distance;
                        block._NodeRect = tempRect;
                        if (FungusEditorPreferences.useGridSnap)
                        {
                            block._NodeRect = block._NodeRect.SnapPosition(GridObjectSnap);
                        }
                        Repaint();
                    }

                    dragBlock = null;
                }

                // Check to see if selection actually changed?
                if (selectionBox.size.x > 0 && selectionBox.size.y > 0)
                {
                    Undo.RecordObject(flowchart, "Select");
                    flowchart.UpdateSelectedCache();

                    EndControlSelection();
                    //if ctrl down push them immediately back into mouse down
                    if (GetAppendModifierDown())
                        StartControlSelection();

                    Repaint();

                    if (flowchart.SelectedBlock != null)
                    {
                        SetBlockForInspector(flowchart, flowchart.SelectedBlock);
                    }
                    Repaint();
                }
                else
                {
                    if (!GetAppendModifierDown() && !hasDraggedSelected)
                    {
                        DeselectAll();

                        if (hitBlock != null)
                        {
                            SelectBlock(hitBlock);
                        }
                    }
                }

                hasDraggedSelected = false;
                break;

            case MouseButton.Right:
                if (rightClickDown != -Vector2.one)
                {
                    var menu = new GenericMenu();
                    var mousePosition = rightClickDown;

                    // Clicked on a block
                    if (hitBlock != null)
                    {
                        flowchart.AddSelectedBlock(hitBlock);

                        // Use a copy because flowchart.SelectedBlocks gets modified
                        var blockList = new List<Block>(flowchart.SelectedBlocks);
                        menu.AddItem(new GUIContent ("Copy"), false, () => Copy());
                        menu.AddItem(new GUIContent ("Cut"), false, () => Cut());
                        menu.AddItem(new GUIContent ("Duplicate"), false, () => Duplicate());
                        menu.AddItem(new GUIContent ("Delete"), false, () => AddToDeleteList(blockList));
                        menu.AddSeparator("");
                        if(Application.isPlaying)
                        {
                            menu.AddItem(new GUIContent("StopAll"), false, () => StopAllBlocks());
                            menu.AddItem(new GUIContent("Stop"), false, () => StopThisBlock(hitBlock));
                            menu.AddItem(new GUIContent("Execute"), false, () => ExecuteThisBlock(hitBlock, false));
                            menu.AddItem(new GUIContent("Execute (Stop All First)"), false, () => ExecuteThisBlock(hitBlock, true));
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("StopAll"));//, false), () => StopAllBlocks());
                            menu.AddDisabledItem(new GUIContent("Stop"));//, false);, () => StopThisBlock(hitBlock));
                            menu.AddDisabledItem(new GUIContent("Execute"));//, false);, () => ExecuteThisBlock(hitBlock, false));
                            menu.AddDisabledItem(new GUIContent("Execute (Stop All First)"));//, false);, () => ExecuteThisBlock(hitBlock, true));
                        }
                    }
                    else
                    {
                        DeselectAll();

                        menu.AddItem(new GUIContent("Add Block"), false, () => CreateBlock(flowchart, mousePosition / flowchart.Zoom - flowchart.ScrollPos));

                        if (copyList.Count > 0)
                        {
                            menu.AddItem(new GUIContent("Paste"), false, () => Paste(mousePosition));
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Paste"));
                        }

                        menu.AddSeparator("");
                        if (Application.isPlaying)
                        {
                            menu.AddItem(new GUIContent("StopAll"), false, () => StopAllBlocks());
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("StopAll"));//, false);, () => StopAllBlocks());
                        }
                    }

                    var menuRect = new Rect();
                    menuRect.position = new Vector2(mousePosition.x, mousePosition.y - 12f);
                    menu.DropDown(menuRect);
                    e.Use();               
                }
                break;
            }

            // Selection box
            selectionBox.size = Vector2.zero;
            selectionBox.position = -Vector2.one;
            startSelectionBoxPosition = selectionBox.position;
        }

        protected override void OnScrollWheel(Event e)
        {
            if (selectionBox.size == Vector2.zero)
            {
                Vector2 zoomCenter;
                zoomCenter.x = e.mousePosition.x / flowchart.Zoom / position.width;
                zoomCenter.y = e.mousePosition.y / flowchart.Zoom / position.height;
                zoomCenter *= flowchart.Zoom;

                DoZoom(-e.delta.y * 0.01f, zoomCenter);
                e.Use();
            }
        }

        protected virtual void DrawFlowchartView(Event e)
        {
            // Calc rect for script view
            Rect scriptViewRect = CalcFlowchartWindowViewRect();

            EditorZoomArea.Begin(flowchart.Zoom, scriptViewRect);

            if (e.type == EventType.Repaint)
            {
                DrawGrid();

                //draw all non selected
                for (int i = 0; i < blocks.Length; ++i)
                {
                    var block = blocks[i];
                    if(!block.IsSelected && ! block.IsControlSelected)
                        DrawBlock(block, scriptViewRect);
                }
                
                //draw all held
                for (int i = 0; i < blocks.Length; ++i)
                {
                    var block = blocks[i];
                    if (block.IsControlSelected)
                        DrawBlock(block, scriptViewRect);
                }

                //draw all selected
                for (int i = 0; i < blocks.Length; ++i)
                {
                    var block = blocks[i];
                    if (block.IsSelected)
                        DrawBlock(block, scriptViewRect);
                }
            }

            // Draw play icons beside all executing blocks
            if (Application.isPlaying)
            {
                var emptyStyle = new GUIStyle();

                //cache these once as they can end up being called thousands of times per frame otherwise
                var curRealTime = Time.realtimeSinceStartup;

                for (int i = 0; i < blocks.Length; ++i)
                {
                    var b = blocks[i];
                    DrawExecutingBlockIcon(b,
                        scriptViewRect,
                        (b.ExecutingIconTimer - curRealTime) / FungusConstants.ExecutingIconFadeTime,
                        emptyStyle);
                }
                GUI.color = Color.white;
            }

            EditorZoomArea.End();
        }

        private void DrawExecutingBlockIcon(Block b, Rect scriptViewRect, float alpha, GUIStyle style)
        {
            if (alpha <= 0)
                return;

            Rect rect = new Rect(b._NodeRect);

            rect.x += flowchart.ScrollPos.x - 37;
            rect.y += flowchart.ScrollPos.y + 3;
            rect.width = 34;
            rect.height = 34;

            if (scriptViewRect.Overlaps(rect))
            {
                GUI.color = new Color(1f, 1f, 1f, alpha);

                if (GUI.Button(rect, FungusEditorResources.PlayBig, style))
                {
                    SelectBlock(b);
                }

                GUI.color = Color.white;
            }
        }

        private Rect CalcFlowchartWindowViewRect()
        {
            return new Rect(0, 0, this.position.width / flowchart.Zoom, this.position.height / flowchart.Zoom);
        }

        public virtual Vector2 GetBlockCenter(Block[] blocks)
        {
            if (blocks.Length == 0)
            {
                return Vector2.zero;
            }

            Vector2 min = blocks[0]._NodeRect.min;
            Vector2 max = blocks[0]._NodeRect.max;

            for (int i = 0; i < blocks.Length; ++i)
            {
                var block = blocks[i];
                min.x = Mathf.Min(min.x, block._NodeRect.center.x);
                min.y = Mathf.Min(min.y, block._NodeRect.center.y);
                max.x = Mathf.Max(max.x, block._NodeRect.center.x);
                max.y = Mathf.Max(max.y, block._NodeRect.center.y);
            }

            return (min + max) * 0.5f;
        }

        protected virtual void CenterFlowchart()
        {
            UpdateBlockCollection();

            if (blocks.Length > 0)
            {
                var center = -GetBlockCenter(blocks);
                center.x += position.width * 0.5f / flowchart.Zoom;
                center.y += position.height * 0.5f / flowchart.Zoom;

                flowchart.CenterPosition = center;
                flowchart.ScrollPos = flowchart.CenterPosition;
            }
        }

        protected virtual void DoZoom(float delta, Vector2 center)
        {
            var prevZoom = flowchart.Zoom;
            flowchart.Zoom += delta;
            flowchart.Zoom = Mathf.Clamp(flowchart.Zoom, MinZoomValue, MaxZoomValue);
            var deltaSize = position.size / prevZoom - position.size / flowchart.Zoom;
            var offset = -Vector2.Scale(deltaSize, center);
            flowchart.ScrollPos += offset;
            forceRepaintCount = 1;
        }

        //Potentially could be faster using https://forum.unity.com/threads/how-do-i-access-the-background-image-used-for-the-animator.501876/
        protected virtual void DrawGrid()
        {
            float width = this.position.width / flowchart.Zoom;
            float height = this.position.height / flowchart.Zoom;

            Handles.color = gridLineColor;

            
            float x = flowchart.ScrollPos.x % GridLineSpacingSize;
            while (x < width)
            {
                Handles.DrawLine(new Vector2(x, 0), new Vector2(x, height));
                x += GridLineSpacingSize;
            }
            
            float y = (flowchart.ScrollPos.y % GridLineSpacingSize);
            while (y < height)
            {
                if (y >= 0)
                {
                    Handles.DrawLine(new Vector2(0, y), new Vector2(width, y));
                }
                y += GridLineSpacingSize;
            }

            Handles.color = Color.white;
        }

        protected virtual void SelectBlock(Block block)
        {
            // Select the block and also select currently executing command
            flowchart.SelectedBlock = block;
            SetBlockForInspector(flowchart, block);
        }

        protected virtual void DeselectAll()
        {
            Undo.RecordObject(flowchart, "Deselect");
            flowchart.ClearSelectedCommands();
            EndControlSelection();
            flowchart.ClearSelectedBlocks();
            Selection.activeGameObject = flowchart.gameObject;
        }
        
        public Block CreateBlock(Flowchart flowchart, Vector2 position)
        {
            Block newBlock = flowchart.CreateBlock(position);
            UpdateBlockCollection();
            Undo.RegisterCreatedObjectUndo(newBlock, "New Block");

            // Use AddSelected instead of Select for when multiple blocks are duplicated
            flowchart.AddSelectedBlock(newBlock);
            SetBlockForInspector(flowchart, newBlock);

            return newBlock;
        }

        public Block CreateBlockSuppressSelect(Flowchart flowchart, Vector2 position)
        {
            Block newBlock = flowchart.CreateBlock(position);
            UpdateBlockCollection();
            Undo.RegisterCreatedObjectUndo(newBlock, "New Block");

            return newBlock;
        }

        //prevent every DrawConnections from allocating a new list for all of its connections
        protected List<Block> connectedBlocksWorkSpace = new List<Block>();

        protected virtual void DrawConnections(Block block)
        {
            if (block == null)
            {
                return;
            }


            bool blockIsSelected = flowchart.SelectedBlock == block;


            Rect scriptViewRect = CalcFlowchartWindowViewRect();

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

                connectedBlocksWorkSpace.Clear();
                command.GetConnectedBlocks(ref connectedBlocksWorkSpace);

                foreach (var blockB in connectedBlocksWorkSpace)
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

                    Rect boundRect = new Rect();
                    boundRect.xMin = Mathf.Min(startRect.xMin, endRect.xMin);
                    boundRect.xMax = Mathf.Max(startRect.xMax, endRect.xMax);
                    boundRect.yMin = Mathf.Min(startRect.yMin, endRect.yMin);
                    boundRect.yMax = Mathf.Max(startRect.yMax, endRect.yMax);

                    if (boundRect.Overlaps(scriptViewRect))
                        DrawRectConnection(startRect, endRect, highlight);
                }
            }
        }

        static readonly Vector2[] pointsA = new Vector2[4];
        static readonly Vector2[] pointsB = new Vector2[4];

        //we only connect mids on sides to matching opposing middle side on other block
        private struct IndexPair { public int a, b;  public IndexPair(int a, int b) { this.a = a;this.b = b; } }
        static readonly IndexPair[] closestCornerIndexPairs = new IndexPair[]
        {
            new IndexPair(){a=0,b=3 },
            new IndexPair(){a=3,b=0 },
            new IndexPair(){a=1,b=2 },
            new IndexPair(){a=2,b=1 },
        };

        //prevent alloc in DrawAAConvexPolygon
        static readonly Vector3[] beizerWorkSpace = new Vector3[3];

        protected virtual void DrawRectConnection(Rect rectA, Rect rectB, bool highlight)
        {
            //previous method made a lot of garbage so now we reuse the same array
            pointsA[0] = new Vector2(rectA.xMin, rectA.center.y);
            pointsA[1] = new Vector2(rectA.xMin + rectA.width / 2, rectA.yMin);
            pointsA[2] = new Vector2(rectA.xMin + rectA.width / 2, rectA.yMax);
            pointsA[3] = new Vector2(rectA.xMax, rectA.center.y);

            pointsB[0] = new Vector2(rectB.xMin, rectB.center.y);
            pointsB[1] = new Vector2(rectB.xMin + rectB.width / 2, rectB.yMin);
            pointsB[2] = new Vector2(rectB.xMin + rectB.width / 2, rectB.yMax);
            pointsB[3] = new Vector2(rectB.xMax, rectB.center.y);

            Vector2 pointA = Vector2.zero;
            Vector2 pointB = Vector2.zero;
            float minDist = float.MaxValue;

            //previous method compared every point to every point
            //  we only check mathcing opposing mids
            for (int i = 0; i < closestCornerIndexPairs.Length; i++)
            {
                var a = pointsA[closestCornerIndexPairs[i].a];
                var b = pointsB[closestCornerIndexPairs[i].b];
                float d = Vector2.Distance(a, b);
                if (d < minDist)
                {
                    pointA = a;
                    pointB = b;
                    minDist = d;
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
            //reuse same array to avoid the auto alloced one in DrawAAConvexPolygon
            beizerWorkSpace[0] = point;
            beizerWorkSpace[1] = point + direction * 10 + perp * 5;
            beizerWorkSpace[2] = point + direction * 10 - perp * 5;
            Handles.DrawAAConvexPolygon(beizerWorkSpace);

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

        protected void AddToDeleteList(List<Block> blocks)
        {
            for (int i = 0; i < blocks.Count; ++i)
            {
                FlowchartWindow.deleteList.Add(blocks[i]);
            }
        }

        internal void StopThisBlock(Block block)
        {
            block.Stop();
        }

        internal void StopAllBlocks()
        {
            flowchart.StopAllBlocks();
        }

        internal void ExecuteThisBlock(Block block, bool stopRunningBlocks)
        {
            if (stopRunningBlocks)
                StopAllBlocks();

            block.StartExecution();
        }

        public void DeleteBlocks()
        {
             // Delete any scheduled objects
            for (int i = 0; i < deleteList.Count; ++i)
            {
                var deleteBlock = deleteList[i];

                var commandList = deleteBlock.CommandList;
                for (int j = 0; j < commandList.Count; ++j)
                {
                    Undo.DestroyObjectImmediate(commandList[j]);
                }

                if (deleteBlock._EventHandler != null)
                {
                    Undo.DestroyObjectImmediate(deleteBlock._EventHandler);
                }
                
                if (deleteBlock.IsSelected)
                {
                    // Deselect
                    flowchart.DeselectBlockNoCheck(deleteBlock);
                }

                Undo.DestroyObjectImmediate(deleteBlock);
            }

            if (deleteList.Count > 0)
            {
                UpdateBlockCollection();
                // Revert to showing properties for the Flowchart
                Selection.activeGameObject = flowchart.gameObject;
                flowchart.ClearSelectedCommands();
                Repaint();
            }

            deleteList.Clear();
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
            return (Event.current != null && Event.current.shift) || EditorGUI.actionKey;
        }

        protected virtual void Copy()
        {
            copyList.Clear();

            foreach (var block in flowchart.SelectedBlocks)
            {
                copyList.Add(new BlockCopy(block));
            }
            foreach (var block in mouseDownSelectionState)
            {
                copyList.Add(new BlockCopy(block));
            }
        }

        protected virtual void Cut()
        {
            Copy();
            Undo.RecordObject(flowchart, "Cut");
            AddToDeleteList(flowchart.SelectedBlocks);
        }

        // Center is position in unscaled window space
        protected virtual void Paste(Vector2 center, bool relative = false)
        {
            Undo.RecordObject(flowchart, "Deselect");
            DeselectAll();

            var pasteList = new List<Block>();

            foreach (var copy in copyList)
            {
                pasteList.Add(copy.PasteBlock(this, flowchart));
            }

            var copiedCenter = GetBlockCenter(pasteList.ToArray()) + flowchart.ScrollPos;
            var delta = relative ? center : (center / flowchart.Zoom - copiedCenter);
            
            foreach (var block in pasteList)
            {
                var tempRect = block._NodeRect;
                tempRect.position += delta;
                block._NodeRect = tempRect;
            }

            UpdateBlockCollection();
        }

        protected virtual void Duplicate()
        {
            var tempCopyList = new List<BlockCopy>(copyList);
            Copy();
            Paste(new Vector2(20, 0), true);
            copyList = tempCopyList;
        }

        protected override void OnValidateCommand(Event e)
        {
            if (e.type == EventType.ValidateCommand)
            {
                var c = e.commandName;
                if (c == "Copy" || c == "Cut" || c == "SoftDelete" || c == "Delete" || c == "Duplicate")
                {
                    if (flowchart.SelectedBlocks.Count > 0 || mouseDownSelectionState.Count > 0)
                    {
                        e.Use();
                    }
                }
                else if (c == "Paste")
                {
                    if (copyList.Count > 0)
                    {
                        e.Use();
                    }
                }
                else if (c == "SelectAll" || c == "Find")
                {
                    e.Use();
                }
            }
        }

        protected override void OnExecuteCommand(Event e)
        {
            switch (e.commandName)
            {
            case "Copy":
                Copy();
                e.Use();
                break;
            
            case "Cut":
                Cut();
                e.Use();
                break;

            case "Paste":
                Paste(position.center - position.position);
                e.Use();
                break;

            case "Delete":
                AddToDeleteList(flowchart.SelectedBlocks);
                e.Use();
                break;

            case "SoftDelete":
                AddToDeleteList(flowchart.SelectedBlocks);
                e.Use();
                break;

            case "Duplicate":
                Duplicate();
                e.Use();
                break;

            case "SelectAll":
                Undo.RecordObject(flowchart, "Selection");
                flowchart.ClearSelectedBlocks();
                for (int i = 0; i < blocks.Length; ++i)
                {
                    flowchart.AddSelectedBlock(blocks[i]);
                }
                e.Use();
                break;

            case "Find":
                blockPopupSelection = 0;
                popupScroll = Vector2.zero;
                EditorGUI.FocusTextInControl(SearchFieldName);
                e.Use();
                break;
            }
        }

        protected virtual void CenterBlock(Block block)
        {
            if (flowchart.Zoom < 1)
            {
                DoZoom(1 - flowchart.Zoom, Vector2.one * 0.5f);
            }

            flowchart.ScrollPos = -block._NodeRect.center + position.size * 0.5f / flowchart.Zoom;
        }

        protected virtual void CloseBlockPopup()
        {
            GUIUtility.keyboardControl = 0;
            searchString = string.Empty;
            filterStale = true;
        }

        static protected List<Block> blockGraphicsUniqueListWorkSpace = new List<Block>();
        static protected List<Block> blockGraphicsConnectedWorkSpace = new List<Block>();
        protected virtual BlockGraphics GetBlockGraphics(Block block)
        {
            var graphics = new BlockGraphics();

            blockGraphicsUniqueListWorkSpace.Clear();
            blockGraphicsConnectedWorkSpace.Clear();
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
                block.GetConnectedBlocks(ref blockGraphicsConnectedWorkSpace);
                foreach (var connectedBlock in blockGraphicsConnectedWorkSpace)
                {
                    if (connectedBlock == block ||
                        blockGraphicsUniqueListWorkSpace.Contains(connectedBlock))
                    {
                        continue;
                    }
                    blockGraphicsUniqueListWorkSpace.Add(connectedBlock);
                }

                if (blockGraphicsUniqueListWorkSpace.Count > 1)
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

        private void DrawBlock(Block block, Rect scriptViewRect)
        {
            float nodeWidthA = nodeStyle.CalcSize(new GUIContent(block.BlockName)).x + 10;
            
            Rect tempRect = block._NodeRect;
            tempRect.width = Mathf.Clamp(nodeWidthA, BlockMinWidth, BlockMaxWidth);
            tempRect.height = DefaultBlockHeight;
            if (FungusEditorPreferences.useGridSnap)
            {
                tempRect = tempRect.SnapWidth(GridObjectSnap);
            }
            block._NodeRect = tempRect;

            // Draw blocks
            var graphics = GetBlockGraphics(block);

            Rect windowRelativeRect = new Rect(block._NodeRect);
            if (FungusEditorPreferences.useGridSnap)
            {
                windowRelativeRect = windowRelativeRect.SnapPosition(GridObjectSnap);
            }
            windowRelativeRect.position += flowchart.ScrollPos;

            //skip if outside of view
            if (scriptViewRect.Overlaps(windowRelativeRect))
            {

                var tmpNormBg = nodeStyle.normal.background;

                // Draw untinted highlight
                if (block.IsSelected && !block.IsControlSelected)
                {
                    GUI.backgroundColor = Color.white;
                    nodeStyle.normal.background = graphics.onTexture;
                    GUI.Box(windowRelativeRect, "", nodeStyle);
                    nodeStyle.normal.background = tmpNormBg;
                }

                if (block.IsControlSelected && !block.IsSelected)
                {
                    GUI.backgroundColor = Color.white;
                    nodeStyle.normal.background = graphics.onTexture;
                    var c = GUI.backgroundColor;
                    c.a = 0.5f;
                    GUI.backgroundColor = c;
                    GUI.Box(windowRelativeRect, "", nodeStyle);
                    nodeStyle.normal.background = tmpNormBg;
                }

                // Draw tinted block; ensure text is readable
                var brightness = graphics.tint.r * 0.3 + graphics.tint.g * 0.59 + graphics.tint.b * 0.11;
                var tmpNormTxtCol = nodeStyle.normal.textColor;
                nodeStyle.normal.textColor = brightness >= 0.5 ? Color.black : Color.white;

                switch (block.FilterState)
                {
                case Block.FilteredState.Full:
                    break;
                case Block.FilteredState.Partial:
                    graphics.tint.a *= 0.65f;
                    break;
                case Block.FilteredState.None:
                    graphics.tint.a *= 0.2f;
                    break;
                default:
                    break;
                }

                nodeStyle.normal.background = graphics.offTexture;
                GUI.backgroundColor = graphics.tint;
                GUI.Box(windowRelativeRect, block.BlockName, nodeStyle);

                GUI.backgroundColor = Color.white;

                if (block.Description.Length > 0)
                {
                    var content = new GUIContent(block.Description);
                    windowRelativeRect.y += windowRelativeRect.height;
                    windowRelativeRect.height = descriptionStyle.CalcHeight(content, windowRelativeRect.width);
                    GUI.Label(windowRelativeRect, content, descriptionStyle);
                }

                GUI.backgroundColor = Color.white;

                nodeStyle.normal.textColor = tmpNormTxtCol;
                nodeStyle.normal.background = tmpNormBg;

                // Draw Event Handler labels
                if (block._EventHandler != null)
                {
                    string handlerLabel = "";
                    EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(block._EventHandler.GetType());
                    if (info != null)
                    {
                        handlerLabel = "<" + info.EventHandlerName + "> ";
                    }

                    Rect rect = new Rect(block._NodeRect);
                    rect.height = handlerStyle.CalcHeight(new GUIContent(handlerLabel), block._NodeRect.width);
                    rect.x += flowchart.ScrollPos.x;
                    rect.y += flowchart.ScrollPos.y - rect.height;

                    GUI.Label(rect, handlerLabel, handlerStyle);
                }
            }

            DrawConnections(block);
        }
    }
}