// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Key press modes supported by Key Pressed event handler.
    /// </summary>
    public enum KeyPressType
    {
        /// <summary> Execute once when the key is pressed down. </summary>
        KeyDown,
        /// <summary> Execute once when the key is released </summary>
        KeyUp,
        /// <summary> Execute once per frame when key is held down. </summary>
        KeyRepeat
    }

    /// <summary>
    /// The block will execute when a key press event occurs.
    /// </summary>
    [EventHandlerInfo("Input",
                      "Key Pressed",
                      "The block will execute when a key press event occurs.")]
    [AddComponentMenu("")]
    public class KeyPressed : EventHandler
    {
        [Tooltip("The type of keypress to activate on")]
        [SerializeField] protected KeyPressType keyPressType;

        [Tooltip("Keycode of the key to activate on")]
        [SerializeField] protected KeyCode keyCode;

        protected virtual void Update()
        {
            switch (keyPressType)
            {
            case KeyPressType.KeyDown:
                if (Input.GetKeyDown(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            case KeyPressType.KeyUp:
                if (Input.GetKeyUp(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            case KeyPressType.KeyRepeat:
                if (Input.GetKey(keyCode))
                {
                    ExecuteBlock();
                }
                break;
            }
        }
    }
}