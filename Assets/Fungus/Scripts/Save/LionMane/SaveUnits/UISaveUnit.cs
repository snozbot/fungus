using UnityEngine;
using TimeSpan = System.TimeSpan;

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

        public TimeSpan Playtime
        {
            get { return savedPlaytime.TimeSpan; }
            set { savedPlaytime.TimeSpan = value; }
        }

        [SerializeField]
        protected SavedTimeSpan savedPlaytime = new SavedTimeSpan();

        public override void OnDeserialize()
        {
            base.OnDeserialize();
            savedPlaytime.OnDeserialize();
        }
    }
}