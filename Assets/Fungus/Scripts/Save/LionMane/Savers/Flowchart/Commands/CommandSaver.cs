using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    // Need this so that in the inspector we can be sure that the BlockSaver is only passed savers
    // that make save units for specific commands

    public abstract class CommandSaver : DataSaver, ICommandSaver
    {
        /// <summary>
        /// Returns a list of the appropriate command save units based on the contents of the passed block.
        /// </summary>
        public abstract IList<ICommandSaveUnit> CreateSavesFrom(Block block);
    }
}