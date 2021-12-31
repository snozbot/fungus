using UnityEngine;

namespace Fungus.TimeSys
{
    [CommandInfo("Timer", "Set Timer Mode", "Sets the type of the timer with the given ID")]
    public class SetTimerMode : TimerCommand
    {
        [SerializeField]
        TimerMode timerMode;

        public override void OnEnter()
        {
            base.OnEnter();
            TimerManager.SetModeOfTimerWithID(timerID, timerMode);
            Continue();
        }
    }
}