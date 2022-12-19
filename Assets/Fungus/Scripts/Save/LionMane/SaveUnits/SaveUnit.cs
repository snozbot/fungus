using UnityEngine;
using System.Collections.Generic;
using Object = System.Object;
using DateTime = System.DateTime;

namespace Fungus.LionManeSaveSys
{
    [System.Serializable]
    public abstract class SaveUnit: ISaveUnit
    {
        /*** For more specific parts of the state that this unit holds. ***/

        /*
         * sfs
         * */
        public IList<SaveUnit> Subunits { get { return subunits; } }
        protected List<SaveUnit> subunits = new List<SaveUnit>();

        protected Object contents;
        public Object Contents
        {
            get { return contents; }
            set { contents = value; }
        }

        public abstract string TypeName { get; }

        public virtual DateTime LastWritten
        {
            get { return savedDateTime.LastWritten; }
            set { savedDateTime.LastWritten = value; }
        }

        protected SavedDateTime savedDateTime = new SavedDateTime();

        public virtual void OnDeserialize()
        {
            savedDateTime.OnDeserialize();
        }

    }

    /// <summary>
    /// Has a default implementation for units of save data made to work with 
    /// Fungus's save system.
    /// </summary>
    /// <typeparam name="TContents"></typeparam>
    public abstract class SaveUnit<TContents> : SaveUnit, ISaveUnit<TContents>
    {
        protected new TContents contents;
        public new virtual TContents Contents
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