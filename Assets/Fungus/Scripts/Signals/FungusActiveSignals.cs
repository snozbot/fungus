// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Fungus Priority event signalling system.
    /// A common point for Fungus core systems and user Commands to signal to external code that a
    /// Fungus system is currently doing something important.
    /// 
    /// One intended use case for this system is to have your code listen to this to know when to 
    /// stop player movement or camera movement etc. when the player is engaged in a conversation 
    /// with an NPC.
    /// </summary>
    public static class FungusPrioritySignals
    {
        #region Public members
        /// <summary>
        /// used by increase and decrease active depth functions.
        /// </summary>
        private static int activeDepth;

        public static int CurrentPriorityDepth
        {
            get
            {
                return activeDepth;
            } 
        }

        public static event FungusPriorityStartHandler OnFungusPriorityStart;
        public delegate void FungusPriorityStartHandler();

        public static event FungusPriorityEndHandler OnFungusPriorityEnd;
        public delegate void FungusPriorityEndHandler();


        public static event FungusPriorityChangeHandler OnFungusPriorityChange;
        public delegate void FungusPriorityChangeHandler(int previousActiveDepth, int newActiveDepth);
        
        /// <summary>
        /// Adds 1 to the theactiveDepth. If it was zero causes the OnFungusPriorityStart
        /// </summary>
        public static void DoIncreasePriorityDepth()
        {
            if(activeDepth == 0)
            {
                if (OnFungusPriorityStart != null)
                {
                    OnFungusPriorityStart();
                }
            }
            if(OnFungusPriorityChange != null)
            {
                OnFungusPriorityChange(activeDepth, activeDepth + 1);
            }
            activeDepth++;
        }

        /// <summary>
        /// Subtracts 1 to the theactiveDepth. If it reaches zero causes the OnFungusPriorityEnd
        /// </summary>
        public static void DoDecreasePriorityDepth()
        {
            if (OnFungusPriorityChange != null)
            {
                OnFungusPriorityChange(activeDepth, activeDepth - 1);
            }
            if(activeDepth == 1)
            {
                if(OnFungusPriorityEnd != null)
                {
                    OnFungusPriorityEnd();
                }
            }
            activeDepth--;
        }

        /// <summary>
        /// Forces active depth back to 0. If already 0 fires no signals.
        /// </summary>
        public static void DoResetPriority()
        {
            if (activeDepth == 0)
                return;

            if (OnFungusPriorityChange != null)
            {
                OnFungusPriorityChange(activeDepth, 0);
            }
            if (OnFungusPriorityEnd != null)
            {
                OnFungusPriorityEnd();
            }
            activeDepth = 0;
        }
        #endregion
    }
}
