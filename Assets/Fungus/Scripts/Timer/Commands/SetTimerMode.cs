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
            TimerManager.SetModeOfTimerWithID(timer.Value, timerMode);
            Continue();
        }

        public override string GetSummary()
        {
            string summary = "";

            if (TimerInputIsSet)
            {
                string timerName = timer.Key;
                summary = string.Format(summaryFormat, timerName, timerMode);
            }

            return summary;
        }

        protected static string summaryFormat = "Set {0} to {1}";
    }
}