using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Common base for PopupWindowContent that is a search filterable list a la AddComponent
    /// 
    /// Inspired by https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SearchableEnum/Code/Editor/SearchablePopup.cs
    /// </summary>
    public abstract class BasePopupWindowContent : PopupWindowContent
    {
        /// <summary>
        /// Called when the user has confirmed an item from the menu.
        /// </summary>
        /// <param name="index">Index of into the original list of items to show given to the popupcontent</param>
        abstract protected void SelectByOrigIndex(int index);

        /// <summary>
        /// Called during Base Ctor, so that all 
        /// </summary>
        abstract protected void PrepareAllItems();

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

        protected Block block;
        protected int hoverIndex;
        protected readonly string SEARCH_CONTROL_NAME = "PopupSearchControlName";
        protected readonly float ROW_HEIGHT = EditorGUIUtility.singleLineHeight;
        protected List<FilteredListItem> allItems = new List<FilteredListItem>(), visibleItems = new List<FilteredListItem>();
        protected string currentFilter = string.Empty;
        protected Vector2 scroll;
        protected int scrollToIndex;
        protected float scrollOffset;
        protected int currentIndex;
        protected Vector2 size;

        static readonly char[] SEARCH_SPLITS = new char[]{ '/', ' ' };

        public BasePopupWindowContent(string currentHandlerName, Block block, int width, int height)
        {
            this.block = block;
            this.size = new Vector2(width, height);

            PrepareAllItems();

            allItems.Sort((lhs, rhs) => lhs.name.CompareTo(rhs.name));
            UpdateFilter();
            currentIndex = Mathf.Max(0, visibleItems.FindIndex(x=>x.name.Contains(currentHandlerName)));
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
                        EditorWindow.focusedWindow.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    EditorWindow.focusedWindow.Close();
                }
            }
        }

       
        protected void SelectByName(string name)
        {
            var loc = allItems.FindIndex(x => x.name == name);
            SelectByOrigIndex(loc);
        }

    }
}