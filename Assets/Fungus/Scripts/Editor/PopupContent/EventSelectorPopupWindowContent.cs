using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Searchable Popup Window for selecting Event type, used by block editor
    /// </summary>
    public class EventSelectorPopupWindowContent : BasePopupWindowContent
    {
        static List<System.Type> _eventHandlerTypes;
        static List<System.Type> EventHandlerTypes
        {
            get
            {
                if (_eventHandlerTypes == null || _eventHandlerTypes.Count == 0)
                    CacheEventHandlerTypes();

                return _eventHandlerTypes;
            }
        }

        static void CacheEventHandlerTypes()
        {
            _eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).Where(x => !x.IsAbstract).ToList();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            CacheEventHandlerTypes();
        }

        protected class SetEventHandlerOperation
        {
            public Block block;
            public Type eventHandlerType;
        }

        protected Block block;
        public EventSelectorPopupWindowContent(string currentHandlerName, Block block, int width, int height)
            :base(currentHandlerName, width, height, true)
        {
            this.block = block;
        }

        protected override void PrepareAllItems()
        {
            int i = 0;
            foreach (System.Type type in EventHandlerTypes)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
                if (info != null)
                {
                    allItems.Add(new FilteredListItem(i, (info.Category.Length > 0 ? info.Category + CATEGORY_CHAR : "") + info.EventHandlerName, info.HelpText));
                }
                else
                {
                    allItems.Add(new FilteredListItem(i, type.Name, info.HelpText));
                }

                i++;
            }
        }
        
        override protected void SelectByOrigIndex(int index)
        {
            SetEventHandlerOperation operation = new SetEventHandlerOperation();
            operation.block = block;
            operation.eventHandlerType = (index >= 0 && index < EventHandlerTypes.Count) ? EventHandlerTypes[index] : null;
            OnSelectEventHandler(operation);
        }


        static public void DoEventHandlerPopUp(Rect position, string currentHandlerName, Block block, int width, int height)
        {
            if (FungusEditorPreferences.useExperimentalMenus)
            {
                //new method
                EventSelectorPopupWindowContent win = new EventSelectorPopupWindowContent(currentHandlerName, block, width, height);
                PopupWindow.Show(position, win);
            }
            //old method
            DoOlderMenu(block);
        }

        static protected void DoOlderMenu(Block block)
        {

            SetEventHandlerOperation noneOperation = new SetEventHandlerOperation();
            noneOperation.block = block;
            noneOperation.eventHandlerType = null;

            GenericMenu eventHandlerMenu = new GenericMenu();
            eventHandlerMenu.AddItem(new GUIContent("None"), false, OnSelectEventHandler, noneOperation);

            // Add event handlers with no category first
            foreach (System.Type type in EventHandlerTypes)
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
            foreach (System.Type type in EventHandlerTypes)
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

            BlockEditor.SelectedBlockDataStale = true;

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(block);
        }
    }
}