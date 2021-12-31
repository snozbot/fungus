using UnityEngine;

namespace Fungus.TimeSys
{
    
    public abstract class TimerEventHandler : EventHandler
    {
        [SerializeField]
        [VariableProperty(typeof(IntegerVariable))]
        [Tooltip("If this has nothing, this responds to any Timer as appropriate for this event handler")]
        protected IntegerVariable[] timerIDs;

        protected virtual void OnEnable()
        {
            ListenForTheTimerEvent();
        }

        protected virtual void ListenForTheTimerEvent()
        {
            TimerEvent += OnTimerEventExecuted;
        }

        protected abstract System.Action<Timer> TimerEvent { get; set; }

        protected virtual void OnTimerEventExecuted(Timer timerThatStarted)
        {
            if (this.ShouldExecuteFor(timerThatStarted))
                ExecuteBlock();
        }

        protected virtual bool ShouldExecuteFor(Timer timer)
        {
            bool executeForAnyTimer = timerIDs.Length == 0;

            if (executeForAnyTimer)
                return true;

            return ThisHasIDOf(timer);
        }

        protected virtual bool ThisHasIDOf(Timer timer)
        {
            foreach (var intVar in this.timerIDs)
            {
                bool validVar = intVar != null;

                if (!validVar)
                    continue;

                int id = intVar.Value;
                if (timer.ID == id)
                    return true;
            }

            return false;
        }

        protected virtual void OnDisable()
        {
            UnlistenForTheTimerEvent();
        }

        protected virtual void UnlistenForTheTimerEvent()
        {
            TimerEvent -= OnTimerEventExecuted;
        }

        

    }
}