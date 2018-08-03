using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Searchable Popup Window for adding a command to a block
    /// 
    /// Inspired by https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SearchableEnum/Code/Editor/SearchablePopup.cs
    /// </summary>
    public class CommandSelectorPopupWindowContent : BasePopupWindowContent
    {
        private static readonly char[] SPLIT_INPUT_ON = new char[] { ' ', '/', '\\' };
        private static readonly int MAX_PREVIEW_GRID = 7;
        private static readonly string ELIPSIS = "...";


        static List<System.Type> commandTypes;

        static void CacheCommandTypes()
        {
            commandTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).Where(x => !x.IsAbstract).ToList();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            CacheCommandTypes();
        }

        static Block curBlock;
        static protected List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes;

        public CommandSelectorPopupWindowContent(string currentHandlerName, Block block, int width, int height)
            : base(currentHandlerName, block, width, height)
        {
        }

        protected override void SelectByOrigIndex(int index)
        {
            var commandType = (index >= 0 && index < commandTypes.Count) ? commandTypes[index] : null;
            AddCommandCallback(commandType);
        }

        protected override void PrepareAllItems()
        {
            if (commandTypes == null || commandTypes.Count == 0)
            {
                CacheCommandTypes();
            }


            filteredAttributes = GetFilteredSupportedCommands(block.GetFlowchart());

            int i = 0;
            foreach (var item in filteredAttributes)
            {
                allItems.Add(new FilteredListItem(i, (item.Value.Category.Length > 0 ? item.Value.Category + "/" : "") + item.Value.CommandName));

                i++;
            }
        }

        static public void ShowCommandMenu(Rect position, string currentHandlerName, Block block, int width, int height)
        {
            curBlock = block;

            var win = new CommandSelectorPopupWindowContent(currentHandlerName, block, width, height);
            PopupWindow.Show(position, win);


            //old method
            DoOlderMenu();
        }

        protected static List<KeyValuePair<System.Type, CommandInfoAttribute>> GetFilteredSupportedCommands(Flowchart flowchart)
        {
            List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = BlockEditor.GetFilteredCommandInfoAttribute(commandTypes);

            filteredAttributes.Sort(BlockEditor.CompareCommandAttributes);

            filteredAttributes = filteredAttributes.Where(x => flowchart.IsCommandSupported(x.Value)).ToList();

            return filteredAttributes;
        }

        


        static protected void DoOlderMenu()
        {
            var flowchart = (Flowchart)curBlock.GetFlowchart();

            GenericMenu commandMenu = new GenericMenu();

            // Build menu list

            foreach (var keyPair in filteredAttributes)
            {
                GUIContent menuItem;
                if (keyPair.Value.Category == "")
                {
                    menuItem = new GUIContent(keyPair.Value.CommandName);
                }
                else
                {
                    menuItem = new GUIContent(keyPair.Value.Category + "/" + keyPair.Value.CommandName);
                }

                commandMenu.AddItem(menuItem, false, AddCommandCallback, keyPair.Key);
            }

            commandMenu.ShowAsContext();
        }

        //Used by GenericMenu Delegate
        static protected void AddCommandCallback(object obj)
        {
            Type command = obj as Type;
            if (command != null)
            {
                AddCommandCallback(command);
            }
        }


        static protected void AddCommandCallback(Type commandType)
        {
            var block = curBlock;
            if (block == null || commandType == null)
            {
                return;
            }

            var flowchart = (Flowchart)block.GetFlowchart();

            // Use index of last selected command in list, or end of list if nothing selected.
            int index = -1;
            foreach (var command in flowchart.SelectedCommands)
            {
                if (command.CommandIndex + 1 > index)
                {
                    index = command.CommandIndex + 1;
                }
            }
            if (index == -1)
            {
                index = block.CommandList.Count;
            }

            var newCommand = Undo.AddComponent(block.gameObject, commandType) as Command;
            block.GetFlowchart().AddSelectedCommand(newCommand);
            newCommand.ParentBlock = block;
            newCommand.ItemId = flowchart.NextItemId();

            // Let command know it has just been added to the block
            newCommand.OnCommandAdded(block);

            Undo.RecordObject(block, "Set command type");
            if (index < block.CommandList.Count - 1)
            {
                block.CommandList.Insert(index, newCommand);
            }
            else
            {
                block.CommandList.Add(newCommand);
            }

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(block);

            flowchart.ClearSelectedCommands();

        }
    }
}