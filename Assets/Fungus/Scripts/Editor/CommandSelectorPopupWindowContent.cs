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
    public class CommandSelectorPopupWindowContent //: PopupWindowContent
    {
        private static readonly char[] SPLIT_INPUT_ON = new char[] { ' ', '/', '\\' };
        private static readonly int MAX_PREVIEW_GRID = 7;
        private static readonly string ELIPSIS = "...";


        static List<System.Type> commandTypes;

        static void CacheEventHandlerTypes()
        {
            commandTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).Where(x => !x.IsAbstract).ToList();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            CacheEventHandlerTypes();
        }

        static Block curBlock;

        static public void ShowCommandMenu(Block block)
        {
            curBlock = block;

            var flowchart = (Flowchart)block.GetFlowchart();

            GenericMenu commandMenu = new GenericMenu();

            // Build menu list
            var filteredAttributes = GetFilteredSupportedCommands(flowchart);

            foreach (var keyPair in filteredAttributes)
            {
                AddCommandOperation commandOperation = new AddCommandOperation();

                commandOperation.commandType = keyPair.Key;

                GUIContent menuItem;
                if (keyPair.Value.Category == "")
                {
                    menuItem = new GUIContent(keyPair.Value.CommandName);
                }
                else
                {
                    menuItem = new GUIContent(keyPair.Value.Category + "/" + keyPair.Value.CommandName);
                }

                commandMenu.AddItem(menuItem, false, AddCommandCallback, commandOperation);
            }

            commandMenu.ShowAsContext();
        }

        protected static List<KeyValuePair<System.Type, CommandInfoAttribute>> GetFilteredSupportedCommands(Flowchart flowchart)
        {
            List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = BlockEditor.GetFilteredCommandInfoAttribute(commandTypes);

            filteredAttributes.Sort(BlockEditor.CompareCommandAttributes);

            filteredAttributes = filteredAttributes.Where(x => flowchart.IsCommandSupported(x.Value)).ToList();

            return filteredAttributes;
        }

        

        //Used by GenericMenu Delegate
        static protected void AddCommandCallback(object obj)
        {
            AddCommandOperation commandOperation = obj as AddCommandOperation;
            if (commandOperation != null)
            {
                AddCommandCallback(commandOperation.commandType);
            }
        }


        static protected void AddCommandCallback(Type commandType)
        {
            var block = curBlock;
            if (block == null)
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

            //commandTextFieldContents = string.Empty;
        }


        protected class AddCommandOperation
        {
            public Type commandType;
        }

        //Handles showing partial matches against the text input next to the AddCommand button
        // Splits and matches and can use up down arrows and return/enter/numenter to confirm
        //  TODO add sorting of results so we get best match at the not just just a match
        //      e.g. "if" should show Flow/If at the top not Flow/Else If
        //private void ShowPartialMatches()
        //{
        //    var block = curBlock;

        //    var flowchart = (Flowchart)block.GetFlowchart();

        //    //TODO this could be cached if input hasn't changed to avoid thrashing
        //    var filteredAttributes = GetFilteredSupportedCommands(flowchart);

        //    var upperCommandText = commandTextFieldContents.ToUpper().Trim();

        //    if (upperCommandText.Length == 0)
        //        return;

        //    var tokens = upperCommandText.Split(SPLIT_INPUT_ON);

        //    //we want commands that have all the elements you have typed
        //    filteredAttributes = filteredAttributes.Where((x) =>
        //    {
        //        bool catAny = tokens.Any(x.Value.Category.ToUpper().Contains);
        //        bool comAny = tokens.Any(x.Value.CommandName.ToUpper().Contains);
        //        bool catAll = tokens.All(x.Value.Category.ToUpper().Contains);
        //        bool comAll = tokens.All(x.Value.CommandName.ToUpper().Contains);

        //        //so if both category and command found something, then there are multiple tokens and they line up with category and command
        //        if (catAny && comAny)
        //            return true;
        //        //or its a single token or a complex token that matches entirely in cat or com
        //        else if (catAll || comAll)
        //            return true;
        //        //this setup avoids multiple bad suggestions due to a complex category name that gives many false matches on complex partials

        //        return false;

        //    }).ToList();

        //    if (filteredAttributes == null || filteredAttributes.Count == 0)
        //        return;

        //    //show results
        //    GUILayout.Space(5);

        //    GUILayout.BeginHorizontal();

        //    filteredCommandPreviewSelectedItem = Mathf.Clamp(filteredCommandPreviewSelectedItem, 0, filteredAttributes.Count - 1);

        //    var toShow = filteredAttributes.Select(x => x.Value.Category + "/" + x.Value.CommandName).ToArray();

        //    //show the first x max that match our filters
        //    if (toShow.Length > MAX_PREVIEW_GRID)
        //    {
        //        toShow = toShow.Take(MAX_PREVIEW_GRID).ToArray();
        //        toShow[MAX_PREVIEW_GRID - 1] = ELIPSIS;
        //    }

        //    filteredCommandPreviewSelectedItem = GUILayout.SelectionGrid(filteredCommandPreviewSelectedItem, toShow, 1);

        //    if (toShow[filteredCommandPreviewSelectedItem] != ELIPSIS)
        //    {
        //        commandSelectedByTextInput = filteredAttributes[filteredCommandPreviewSelectedItem].Key;
        //    }
        //    else
        //    {
        //        commandSelectedByTextInput = null;
        //    }

        //    GUILayout.EndHorizontal();

        //    GUILayout.Space(5);
        //}
    }

}