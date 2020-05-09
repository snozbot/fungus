// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
#if UNITY_2019_2_OR_NEWER

    public static class GenerateEverythingMenuItem
    {
        [MenuItem("Tools/Fungus/Utilities/Generate Everything Flowchart")]
        public static void GenerateEverythingFlowchart()
        {
            var newGO = new GameObject("Flowchart w/ EVERYTHING");
            var flow = newGO.AddComponent<Flowchart>();

            var blockPos = Vector2.zero;
            var blockPosStep = new Vector2(0, 60);

            //adding a block for all event handlers
            foreach (var eventHandlerType in TypeCache.GetTypesWithAttribute<EventHandlerInfoAttribute>())
            {
                var block = flow.CreateBlock(blockPos);
                blockPos += blockPosStep;

                block.BlockName = eventHandlerType.Name;

                EventHandler newHandler = newGO.AddComponent(eventHandlerType) as EventHandler;
                newHandler.ParentBlock = block;
                block._EventHandler = newHandler;
            }

            //reset head
            blockPos = new Vector2(200, 0);

            //adding a block for each category, fill it with its commands
            var blockComCats = new Dictionary<string, Block>();
            foreach (var commandType in TypeCache.GetTypesWithAttribute<CommandInfoAttribute>())
            {
                var commandTypeAttr = commandType.GetCustomAttributes(typeof(CommandInfoAttribute), false)[0] as CommandInfoAttribute;

                blockComCats.TryGetValue(commandTypeAttr.Category, out Block targetBlock);
                if (targetBlock == null)
                {
                    targetBlock = flow.CreateBlock(blockPos);
                    blockPos += blockPosStep;

                    targetBlock.BlockName = commandTypeAttr.Category;
                    blockComCats[commandTypeAttr.Category] = targetBlock;
                }


                var newCommand = newGO.AddComponent(commandType) as Command;
                newCommand.ParentBlock = targetBlock;
                newCommand.ItemId = flow.NextItemId();

                // Let command know it has just been added to the block
                newCommand.OnCommandAdded(targetBlock);
                targetBlock.CommandList.Add(newCommand);
            }

            //add all variable types
            foreach (var varType in TypeCache.GetTypesWithAttribute<VariableInfoAttribute>())
            {
                Variable newVariable = newGO.AddComponent(varType) as Variable;
                newVariable.Key = flow.GetUniqueVariableKey("");
                flow.Variables.Add(newVariable);
            }
        }
    }

#endif
}