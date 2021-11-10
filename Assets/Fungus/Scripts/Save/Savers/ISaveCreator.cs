using System.Collections.Generic;
using Object = System.Object;

namespace Fungus.LionManeSaveSys
{
    /// <summary>
    /// For classes that create save data to be stored in RAM as opposed to saves stored on disk.
    /// </summary>
    public interface ISaveCreator
    {
        ISaveUnit CreateSaveFrom(Object input);
        IList<ISaveUnit> CreateSavesFrom(IList<Object> inputs);
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSaveUnit">Specific type of save unit</typeparam>
    /// <typeparam name="TInput">Input used to make unit</typeparam>
    public interface ISaveCreator<TSaveUnit, TInput>: ISaveCreator where TSaveUnit: ISaveUnit
    {
        TSaveUnit CreateSaveFrom(TInput input);
        IList<TSaveUnit> CreateSavesFrom(IList<TInput> inputs);
    }
}