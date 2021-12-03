using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    /// <summary>
    /// Stores the states of Flowchart Blocks
    /// </summary>
    public struct BlockSaveUnit : ISaveUnit<BlockSaveUnit>
    {
        public BlockSaveUnit Contents => this;
        object ISaveUnit.Contents => this;

        /// <summary>
        /// Unique identifier for the Block this stores the state of.
        /// </summary>
        public int ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        [SerializeField]
        int itemId;

        /// <summary>
        /// Name of the Block this unit was storing the state of.
        /// </summary>
        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }

        [SerializeField]
        string blockName;

        /// <summary>
        /// States of the Commands that were executing at the time this Save Unit was made.
        /// </summary>
        public IList<CommandSaveUnit> ExecutingCommands
        {
            get { return executingCommands; }
        }

        [SerializeField]
        List<CommandSaveUnit> executingCommands;

        public int ExecutionCount
        {
            get { return executionCount; }
        }

        [SerializeField]
        int executionCount;

        public static IList<BlockSaveUnit> From(IList<Block> blocks)
        {
            BlockSaveUnit[] results = new BlockSaveUnit[blocks.Count];

            for (int i = 0; i < blocks.Count; i++)
            {
                Block currentBlock = blocks[i];
                BlockSaveUnit newUnit = From(currentBlock);
                results[i] = newUnit;
            }

            return results;
        }

        public static BlockSaveUnit From(Block block)
        {
            BlockSaveUnit newUnit = new BlockSaveUnit();
            newUnit.executionCount = block.GetExecutionCount();
            newUnit.BlockName = block.BlockName;

            // TODO: set up executing commands

            return newUnit;
        }

    }
}