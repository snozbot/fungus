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
        void Update()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}