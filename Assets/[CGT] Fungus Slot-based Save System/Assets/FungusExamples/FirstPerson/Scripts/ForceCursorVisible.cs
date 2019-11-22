using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.Examples
{
    /// <summary>
    /// Helper for FirstPerson Fungus Demo to work around some of the inflexibility of the Unity standard fps controller
    /// </summary>
    public class ForceCursorVisible : MonoBehaviour
    {
        public bool CursorLocked = true;

        void Update()
        {
            Cursor.visible = !CursorLocked;
            Cursor.lockState = CursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}