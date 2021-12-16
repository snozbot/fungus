using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    public interface ICommandSaver : ISaveCreator
    {
        IList<ICommandSaveUnit> CreateSavesFrom(Block block);
    }

    public interface ICommandSaver<TSaveUnit, TInput> : ICommandSaver, ISaveCreator<TSaveUnit, TInput> 
        where TSaveUnit: ICommandSaveUnit
    {
    }
        

}