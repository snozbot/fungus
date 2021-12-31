using UnityEngine;

namespace Fungus.TimeSys
{
    public abstract class TimerCommand : Command
    {
        [SerializeField]
        protected IntegerData timerID = new IntegerData(0);

        public override void OnEnter()
        {
            base.OnEnter();
            FetchTimerToActOn();
        }

        protected virtual void FetchTimerToActOn()
        {
            TimerManager.EnsureTimerExistsWithID(timerID);
            timer = TimerManager.Timers[timerID];
        }

        protected TimerManager TimerManager
        {
            get { return FungusManager.Instance.TimerManager; }
        }

        protected Timer timer;
    }
}