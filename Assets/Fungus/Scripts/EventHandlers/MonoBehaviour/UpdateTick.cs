// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute every chosen Update, or FixedUpdate or LateUpdate.
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Update",
                      "The block will execute every chosen Update, or FixedUpdate or LateUpdate.")]
    [AddComponentMenu("")]
    public class UpdateTick : EventHandler
    {
        [System.Flags]
        public enum UpdateMessageFlags
        {
            Update = 1 << 0,
            FixedUpdate = 1 << 1,
            LateUpdate = 1 << 2,
        }

        [Tooltip("Which of the Update messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected UpdateMessageFlags FireOn = UpdateMessageFlags.Update;

        private void Update()
        {
            if ((FireOn & UpdateMessageFlags.Update) != 0)
            {
                ExecuteBlock();
            }
        }

        private void FixedUpdate()
        {
            if ((FireOn & UpdateMessageFlags.FixedUpdate) != 0)
            {
                ExecuteBlock();
            }
        }

        private void LateUpdate()
        {
            if ((FireOn & UpdateMessageFlags.LateUpdate) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}