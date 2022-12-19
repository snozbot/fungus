using System.Collections.Generic;
using UnityEngine;
using Fungus.LionManeSaveSys;

namespace Fungus
{
    /** Contains the stuff meant to be shown in the Save UI. 
     * Implements the stuff most often seen in such user interfaces.**/
    public class UISaveUnit : ISaveUnit<UISaveUnit>
    {
        public object Contents => this;

        public string TypeName => "UISaveUnit";

        UISaveUnit ISaveUnit<UISaveUnit>.Contents => this;

        public virtual bool HasSlotNumberAssigned {  get { return SlotNumber >= 0; } }
        public virtual int SlotNumber
        {
            get { return slotNumber; }
            set { slotNumber = value; }
        }

        [SerializeField]
        protected int slotNumber = -1;

    }
}