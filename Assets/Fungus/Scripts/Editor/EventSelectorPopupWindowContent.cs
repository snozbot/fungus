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

        public class FilteredListItem
        {
            public FilteredListItem(int index, string str)
            {
                origIndex = index;
                name = str;
                lowerName = str.ToLowerInvariant();
            }
            public int origIndex;
            public string name, lowerName;
        }

        private string currentHandlerName;
        private Block block;
        private List<Type> eventHandlerTypes;
        private int hoverIndex;
        private readonly string SEARCH_CONTROL_NAME = "PopupSearchControlName";
        private readonly float ROW_HEIGHT = EditorGUIUtility.singleLineHeight;
        private List<FilteredListItem> allItems = new List<FilteredListItem>(), visibleItems = new List<FilteredListItem>();
        private string currentFilter = string.Empty;
        private Vector2 scroll;
        private int scrollToIndex;
        private float scrollOffset;
        private int currentIndex;
        private Vector2 size;

        static readonly char[] SEARCH_SPLITS = new char[]{ '/', ' ' };

        public EventSelectorPopupWindowContent(string currentHandlerName, Block block, List<Type> eventHandlerTypes, int width, int height)
        {
            this.currentHandlerName = currentHandlerName;
            this.block = block;
            this.eventHandlerTypes = eventHandlerTypes;
            this.size = new Vector2(width, height);

            int i = 0;
            foreach (System.Type type in eventHandlerTypes)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
                if (info != null)
                {
                    allItems.Add(new FilteredListItem(i, (info.Category.Length > 0 ? info.Category + "/" : "") + info.EventHandlerName));
                }
                else
                {
                    allItems.Add(new FilteredListItem(i, type.Name));
                }

                i++;
            }
            allItems.Sort((lhs, rhs) => lhs.name.CompareTo(rhs.name));
            UpdateFilter();
        }

        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }

        public override Vector2 GetWindowSize()
        {
            return size;
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
            var prevFilter = currentFilter;
            currentFilter = GUI.TextField(searchRect, currentFilter);

            if (prevFilter != currentFilter)
            {
                UpdateFilter();
            }
        }

        private void UpdateFilter()
        {
            var curlower = currentFilter.ToLowerInvariant();
            var lowers = curlower.Split(SEARCH_SPLITS);
            lowers = lowers.Where(x => x.Length > 0).ToArray();

            if (lowers == null || lowers.Length == 0)
            {
                visibleItems.AddRange(allItems);
            }
            else
            {
                visibleItems = allItems.Where(x =>
                {
                    foreach (var item in lowers)
                    {
                        if (x.lowerName.Contains(currentFilter))
                            return true;
                    }
                    return false;
                }).ToList();
            }

            hoverIndex = 0;
            scroll = Vector2.zero;
            visibleItems.Insert(0, new FilteredListItem(-1, "None"));
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
                        SelectByOrigIndex(visibleItems[i].origIndex);
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

            GUI.Label(labelRect, visibleItems[i].name);
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
                        SelectByOrigIndex(visibleItems[hoverIndex].origIndex);
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

        static public void DoEventHandlerPopUp(Rect position, string currentHandlerName, Block block, List<Type> eventHandlerTypes, int width, int height)
        {
            //new method
            EventSelectorPopupWindowContent win = new EventSelectorPopupWindowContent(currentHandlerName, block, eventHandlerTypes, width, height);
            PopupWindow.Show(position, win);

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
            var loc = allItems.First(x => x.name == name);
            SetEventHandlerOperation operation = new SetEventHandlerOperation();
            operation.block = block;
            operation.eventHandlerType = eventHandlerTypes[loc.origIndex];
            OnSelectEventHandler(operation);
        }

        protected void SelectByOrigIndex(int index)
        {
            SetEventHandlerOperation operation = new SetEventHandlerOperation();
            operation.block = block;
            operation.eventHandlerType = (index >= 0 && index<eventHandlerTypes.Count) ? eventHandlerTypes[index] : null;
            OnSelectEventHandler(operation);
        }
    }
}