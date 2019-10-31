using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// A simple struct wrapping a reference to a Fungus Block. Allows for BlockReferenceDrawer. 
    /// This is the recommended way to directly reference a fungus block in external c# scripts,
    /// as it will give you an inspector field that gives a drop down of all the blocks on a 
    /// flowchart, in a similar way to what you would expect from selecting a block on a command.
    /// 
    /// If you want to showup in the Callers section of the block, ensure your monobehaviours 
    /// that have these also implement IBlockCaller.
    /// </summary>
    [System.Serializable]
    public struct BlockReference
    {
        public Block block;

        public void Execute()
        {
            if (block != null)
                block.StartExecution();
        }
    }
}