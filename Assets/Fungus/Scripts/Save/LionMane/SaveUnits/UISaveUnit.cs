using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;

namespace Fungus.LionManeSaveSys
{
    /** Contains the stuff meant to be shown in the Save UI. 
     * Implements the stuff most often seen in such user interfaces.**/
    [System.Serializable]
    public class UISaveUnit : SaveUnit
    {
        public override string TypeName { get; set; } = "UI";

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