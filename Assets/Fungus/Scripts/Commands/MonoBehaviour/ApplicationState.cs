using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnApplication message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Application",
                      "The block will execute when the desired OnApplication message for the monobehaviour is received.")]
    [AddComponentMenu("")]
    public class ApplicationState : EventHandler
    {

        [System.Flags]
        public enum ApplicationMessageFlags
        {
            OnApplicationGetFocus  = 1 << 0,
            OnApplicationLoseFocus = 1 << 1,
            OnApplicationPause    = 1 << 2,
            OnApplicationResume = 1 << 3,
            OnApplicationQuit = 1 << 4,
        }

        [Tooltip("Which of the Application messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected ApplicationMessageFlags FireOn = ApplicationMessageFlags.OnApplicationQuit;

        private void OnApplicationFocus(bool focus)
        {
            if (
                (focus && ((FireOn & ApplicationMessageFlags.OnApplicationGetFocus) != 0))
                ||
                (!focus && ((FireOn & ApplicationMessageFlags.OnApplicationLoseFocus) != 0))
                )
            {
                ExecuteBlock();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if ( 
                (pause && (( FireOn & ApplicationMessageFlags.OnApplicationPause) != 0))
                ||
                (!pause && ((FireOn & ApplicationMessageFlags.OnApplicationResume) != 0))
                )
            {
                ExecuteBlock();
            }
        }

        private void OnApplicationQuit()
        {
            if((FireOn & ApplicationMessageFlags.OnApplicationQuit) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}
