// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Searchable Popup Window for adding a command to a block
    /// </summary>
    public class CommandSelectorPopupWindowContent : BasePopupWindowContent
    {
        static List<System.Type> _commandTypes;
        static List<System.Type> CommandTypes
        {
            get
            {
                if (_commandTypes == null || _commandTypes.Count == 0)
                    CacheCommandTypes();

                return _commandTypes;
            }
        }

        static void CacheCommandTypes()
        {
            _commandTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).Where(x => !x.IsAbstract).ToList();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            CacheCommandTypes();
        }

        static Block curBlock;
        static protected List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes;

        public CommandSelectorPopupWindowContent(string currentHandlerName, int width, int height)
            : base(currentHandlerName, width, height)
        {
        }

        protected override void SelectByOrigIndex(int index)
        {
            var commandType = (index >= 0 && index < CommandTypes.Count) ? CommandTypes[index] : null;
            AddCommandCallback(commandType);
        }

        protected override void PrepareAllItems()
        {
            filteredAttributes = GetFilteredSupportedCommands(curBlock.GetFlowchart());

            foreach (var item in filteredAttributes)
            {
                //force lookup to orig index here to account for commmand lists being filtered by users
                var newFilteredItem = new FilteredListItem(CommandTypes.IndexOf(item.Key), (item.Value.Category.Length > 0 ? item.Value.Category + CATEGORY_CHAR : "") + item.Value.CommandName, item.Value.HelpText);
                allItems.Add(newFilteredItem);
            }
        }

        static public void ShowCommandMenu(Rect position, string currentHandlerName, Block block, int width, int height)
        {
            curBlock = block;


            if (!FungusEditorPreferences.useLegacyMenus)
            {
                var win = new CommandSelectorPopupWindowContent(currentHandlerName,
                    width, (int)(height - EditorGUIUtility.singleLineHeight * 3));
                PopupWindow.Show(position, win);
            }
            else
            {
                //need to ensure we have filtered data 
                filteredAttributes = GetFilteredSupportedCommands(curBlock.GetFlowchart());
            }

            //old method
            DoOlderMenu();
        }

        protected static List<KeyValuePair<System.Type, CommandInfoAttribute>> GetFilteredSupportedCommands(Flowchart flowchart)
        {
            List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = BlockEditor.GetFilteredCommandInfoAttribute(CommandTypes);

            filteredAttributes.Sort(BlockEditor.CompareCommandAttributes);

            filteredAttributes = filteredAttributes.Where(x => flowchart.IsCommandSupported(x.Value)).ToList();

            return filteredAttributes;
        }
        

        static protected void DoOlderMenu()
        {
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
                    menuItem = new GUIContent(keyPair.Value.Category + CATEGORY_CHAR + keyPair.Value.CommandName);
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