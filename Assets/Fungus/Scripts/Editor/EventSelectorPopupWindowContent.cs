using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Rotorz.ReorderableList;
using System.IO;
using System.Reflection;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Searchable Popup Window for selecting Event type, used by block editor
    /// 
    /// Inspired by https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SearchableEnum/Code/Editor/SearchablePopup.cs
    /// </summary>
    public class EventSelectorPopupWindowContent : PopupWindowContent
    {
        protected class SetEventHandlerOperation
        {
            public Block block;
            public Type eventHandlerType;
        }

        private string currentHandlerName;
        private Block block;
        private List<Type> eventHandlerTypes;
        private int hoverIndex;
        private readonly string SEARCH_CONTROL_NAME = "PopupSearchControlName";
        private readonly float ROW_HEIGHT = EditorGUIUtility.singleLineHeight;
        private List<string> allItems = new List<string>(), visibleItems = new List<string>();
        private string currentFilter;
        private Vector2 scroll;
        private int scrollToIndex;
        private float scrollOffset;
        private int currentIndex;

        public EventSelectorPopupWindowContent(string currentHandlerName, Block block, List<Type> eventHandlerTypes)
        {
            this.currentHandlerName = currentHandlerName;
            this.block = block;
            this.eventHandlerTypes = eventHandlerTypes;

            foreach (System.Type type in eventHandlerTypes)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
                if (info != null)
                {
                    allItems.Add(info.Category + "/" + info.EventHandlerName);
                }
                else
                {
                    allItems.Add(type.Name);
                }
            }
            visibleItems.AddRange(allItems);
        }

        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }

        private void DrawSearch(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
                EditorStyles.toolbar.Draw(rect, false, false, false, false);

            Rect searchRect = new Rect(rect);
            searchRect.xMin += 6;
            searchRect.xMax -= 6;
            searchRect.y += 2;
            //searchRect.width -= CancelButton.fixedWidth;

            GUI.FocusControl(SEARCH_CONTROL_NAME);
            GUI.SetNextControlName(SEARCH_CONTROL_NAME);
            string newText = GUI.TextField(searchRect, "FilterBy");

            //if (list.UpdateFilter(newText))
            //{
            //    hoverIndex = 0;
            //    scroll = Vector2.zero;
            //}

            //searchRect.x = searchRect.xMax;
            //searchRect.width = CancelButton.fixedWidth;

            //if (string.IsNullOrEmpty(list.Filter))
            //    GUI.Box(searchRect, GUIContent.none, DisabledCancelButton);
            //else if (GUI.Button(searchRect, "x", CancelButton))
            //{
            //    list.UpdateFilter("");
            //    scroll = Vector2.zero;
            //}
        }

        private void DrawSelectionArea(Rect scrollRect)
        {
            Rect contentRect = new Rect(0, 0,
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                visibleItems.Count * ROW_HEIGHT);

            scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

            Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);

            for (int i = 0; i < visibleItems.Count; i++)
            {
                if (scrollToIndex == i &&
                    (Event.current.type == EventType.Repaint
                     || Event.current.type == EventType.Layout))
                {
                    Rect r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    scroll.x = 0;
                }

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseMove ||
                        Event.current.type == EventType.ScrollWheel)
                        hoverIndex = i;
                    if (Event.current.type == EventType.MouseDown)
                    {
                        //onSelectionMade(list.Entries[i].Index);
                        SelectByName(visibleItems[i]);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }

        private static void DrawBox(Rect rect, Color tint)
        {
            Color c = GUI.color;
            GUI.color = tint;
            GUI.Box(rect, "");
            GUI.color = c;
        }

        private void DrawRow(Rect rowRect, int i)
        {
            if (i == currentIndex)
                DrawBox(rowRect, Color.cyan);
            else if (i == hoverIndex)
                DrawBox(rowRect, Color.white);

            Rect labelRect = new Rect(rowRect);
            //labelRect.xMin += ROW_INDENT;

            GUI.Label(labelRect, visibleItems[i]);
        }

        /// <summary>
        /// Process keyboard input to navigate the choices or make a selection.
        /// </summary>
        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    hoverIndex = Mathf.Min(visibleItems.Count - 1, hoverIndex + 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = -ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (hoverIndex >= 0 && hoverIndex < visibleItems.Count)
                    {
                        SelectByName(visibleItems[hoverIndex]);
                        //onSelectionMade(list.Entries[hoverIndex].Index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    EditorWindow.focusedWindow.Close();
                }
            }
        }

        static public void DoEventHandlerPopUp(Rect rect, string currentHandlerName, Block block, List<Type> eventHandlerTypes)
        {
            //new method
            EventSelectorPopupWindowContent win = new EventSelectorPopupWindowContent(currentHandlerName, block, eventHandlerTypes);
            PopupWindow.Show(rect, win);

            //old method
            
            SetEventHandlerOperation noneOperation = new SetEventHandlerOperation();
            noneOperation.block = block;
            noneOperation.eventHandlerType = null;

            GenericMenu eventHandlerMenu = new GenericMenu();
            eventHandlerMenu.AddItem(new GUIContent("None"), false, OnSelectEventHandler, noneOperation);

            // Add event handlers with no category first
            foreach (System.Type type in eventHandlerTypes)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
                if (info != null &&
                    info.Category.Length == 0)
                {
                    SetEventHandlerOperation operation = new SetEventHandlerOperation();
                    operation.block = block;
                    operation.eventHandlerType = type;

                    eventHandlerMenu.AddItem(new GUIContent(info.EventHandlerName), false, OnSelectEventHandler, operation);
                }
            }

            // Add event handlers with a category afterwards
            foreach (System.Type type in eventHandlerTypes)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
                if (info != null &&
                    info.Category.Length > 0)
                {
                    SetEventHandlerOperation operation = new SetEventHandlerOperation();
                    operation.block = block;
                    operation.eventHandlerType = type;
                    string typeName = info.Category + "/" + info.EventHandlerName;
                    eventHandlerMenu.AddItem(new GUIContent(typeName), false, OnSelectEventHandler, operation);
                }
            }


            eventHandlerMenu.ShowAsContext();
            

        }


        static protected void OnSelectEventHandler(object obj)
        {
            SetEventHandlerOperation operation = obj as SetEventHandlerOperation;
            Block block = operation.block;
            System.Type selectedType = operation.eventHandlerType;
            if (block == null)
            {
                return;
            }

            Undo.RecordObject(block, "Set Event Handler");

            if (block._EventHandler != null)
            {
                Undo.DestroyObjectImmediate(block._EventHandler);
            }

            if (selectedType != null)
            {
                EventHandler newHandler = Undo.AddComponent(block.gameObject, selectedType) as EventHandler;
                newHandler.ParentBlock = block;
                block._EventHandler = newHandler;
            }

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(block);
        }

        protected void SelectByName(string name)
        {
            var loc = allItems.IndexOf(name);
            SetEventHandlerOperation operation = new SetEventHandlerOperation();
            operation.block = block;
            operation.eventHandlerType = eventHandlerTypes[loc];
            OnSelectEventHandler(operation);
        }
    }
}