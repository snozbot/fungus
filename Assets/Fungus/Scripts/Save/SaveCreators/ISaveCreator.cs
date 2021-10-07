using Object = System.Object;

namespace Fungus
{
    /// <summary>
    /// For classes that create save data.
    /// </summary>
    public interface ISaveCreator
    {
        ISaveUnit CreateSaveFrom(Object input);
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSaveUnit">Specific type of save unit</typeparam>
    /// <typeparam name="TInput">Input used to make unit</typeparam>
    public interface ISaveCreator<TSaveUnit, TInput>: ISaveCreator where TSaveUnit: ISaveUnit
    {
        TSaveUnit CreateSaveFrom(TInput input);
    }
}