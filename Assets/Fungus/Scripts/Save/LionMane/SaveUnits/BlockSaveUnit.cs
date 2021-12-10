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
        /// Keys are the exact types of the commands' states, values are the json representations of 
        /// said states.
        /// </summary>
        public IList<StringPair> ExecutingCommands
        {
            get { return executingCommands; }
            set 
            {
                executingCommands.Clear();
                executingCommands.AddRange(value);
            }
        }

        [SerializeField]
        List<StringPair> executingCommands;

        public int ExecutionCount
        {
            get { return executionCount; }
            set { executionCount = value; }
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

        /// <summary>
        /// Note that this does not create a BlockSaveUnit with the executingCommands populated, given
        /// how command save states will have to be made on a case-by-case basis.
        /// </summary>
        public static BlockSaveUnit From(Block block)
        {
            BlockSaveUnit newUnit = new BlockSaveUnit();
            newUnit.ItemId = block.ItemId;
            newUnit.executionCount = block.GetExecutionCount();
            newUnit.BlockName = block.BlockName;
            newUnit.executingCommands = new List<StringPair>();

            return newUnit;
        }


    }
}