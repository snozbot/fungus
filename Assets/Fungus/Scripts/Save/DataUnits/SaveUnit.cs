using Object = System.Object;

namespace Fungus
{
    public abstract class SaveUnit: ISaveUnit
    {
        protected Object contents;
        public Object Contents
        {
            get { return contents; }
            set { contents = value; }
        }
    }

    /// <summary>
    /// Has a default implementation for units of save data made to work with 
    /// Fungus's save system.
    /// </summary>
    /// <typeparam name="TSave"></typeparam>
    public abstract class SaveUnit<TSave> : SaveUnit, ISaveUnit<TSave>
    {
        protected new TSave contents;
        public new virtual TSave Contents
        {
            get { return contents; }
            set
            {
                this.contents = value;
                base.Contents = value;
                // ^So that when being treated as a non-specific SaveUnit, the right stuff is passed
                // when calling its Contents property
            }
        }

    }
}