// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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