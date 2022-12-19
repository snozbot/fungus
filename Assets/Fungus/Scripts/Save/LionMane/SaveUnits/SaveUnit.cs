using UnityEngine;
using System.Collections.Generic;
using Object = System.Object;
using DateTime = System.DateTime;

namespace Fungus.LionManeSaveSys
{
    [System.Serializable]
    public abstract class SaveUnit: ISaveUnit, System.IEquatable<SaveUnit>
    {
        /// <summary>
        /// For more specific parts of the state this unit holds.
        /// </summary>
        public IList<ISaveUnit> Subunits { get { return subunits as IList<ISaveUnit>; } }
        protected List<SaveUnit> subunits = new List<SaveUnit>();

        public abstract string TypeName { get; set; }

        public virtual DateTime LastWritten
        {
            get { return savedLastWritten.LastWritten; }
            set { savedLastWritten.LastWritten = value; }
        }

        [SerializeField]
        protected SavedDateTime savedLastWritten = new SavedDateTime();

        public virtual void OnDeserialize()
        {
            savedLastWritten.OnDeserialize();
        }

        public virtual bool Equals(SaveUnit other)
        {
            bool sameTypeName = this.TypeName.Equals(other.TypeName);
            bool sameSubunits = SameSubunitsAs(other);
            bool sameWriteTime = LastWritten.Equals(other.LastWritten);
            bool whetherTheyreEqual = sameTypeName && sameSubunits && sameWriteTime;
            return whetherTheyreEqual;
        }

        protected virtual bool SameSubunitsAs(SaveUnit other)
        {
            if (this.subunits.Count != other.subunits.Count)
                return false;

            for (int i = 0; i < this.subunits.Count; i++)
            {
                var unitOfThis = this.subunits[i];
                var unitOfOther = other.subunits[i];
                if (!unitOfThis.Equals(unitOfOther))
                    return false;
            }

            return true;
        }

    }

}