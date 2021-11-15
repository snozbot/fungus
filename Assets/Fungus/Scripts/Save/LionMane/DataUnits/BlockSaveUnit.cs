using System.Collections;
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

        [SerializeField]
        List<int> executingCommandIndexes;

        
    }
}