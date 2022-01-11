using UnityEngine;
using TimeSpan = System.TimeSpan;

namespace Fungus.TimeSys
{
    public abstract class TimerCommand : Command
    {
        [Tooltip("Has the ID of the timer you want to work with.")]
        [SerializeField]
        [VariableProperty(typeof(IntegerVariable))]
        protected IntegerVariable timer;

        public override void OnEnter()
        {
            base.OnEnter();

            bool noTimerSet = timer == null;
            if (noTimerSet)
            {
                AlertNoTimerSet();
                return;
            }

            FetchTimerObjectToActOn();
            timeRecorded = timerObj.TimeRecorded;
        }

        protected virtual void AlertNoTimerSet()
        {
            string messageFormat = "A Timer Command in Flowchart {0}, Block {1} has no timer set to work with.";
            string message = string.Format(messageFormat, GetFlowchart().name, ParentBlock.BlockName);
            throw new System.ArgumentException(message);
        }

        protected virtual void FetchTimerObjectToActOn()
        {
            TimerManager.EnsureTimerExistsWithID(timer.Value);
            timerObj = TimerManager.Timers[timer.Value];
        }

        protected TimerManager TimerManager
        {
            get { return FungusManager.Instance.TimerManager; }
        }

        protected Timer timerObj;
        protected TimeSpan timeRecorded;
        
        public override Color GetButtonColor()
        {
            return buttonColor;
        }

        protected static Color32 buttonColor = new Color32(200, 222, 255, 255);

        public override string GetSummary()
        {
            string summary = "";

            if (TimerInputIsSet)
            {
                string timerName = timer.Key;
                summary = timerName;
            }

            return summary;
        }

        protected virtual bool TimerInputIsSet { get { return timer != null; } }
    }
}