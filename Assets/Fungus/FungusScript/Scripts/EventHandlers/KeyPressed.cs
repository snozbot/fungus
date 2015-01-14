using UnityEngine;
using System.Collections;

namespace Fungus
{
	[EventHandlerInfo("Input",
	                  "Key Pressed",
	                  "The sequence will execute when a key press event occurs.")]
	public class KeyPressed : EventHandler
	{	
		public enum KeyPressType
		{
			KeyDown,	// Execute once when the key is pressed down
			KeyUp,		// Execute once when the key is released
			KeyRepeat	// Execute once per frame when key is held down
		}

		public KeyPressType keyPressType;

		public KeyCode keyCode;

		protected virtual void Update()
		{
			switch (keyPressType)
			{
			case KeyPressType.KeyDown:
				if (Input.GetKeyDown(keyCode))
				{
					ExecuteSequence();
				}
				break;
			case KeyPressType.KeyUp:
				if (Input.GetKeyUp(keyCode))
				{
					ExecuteSequence();
				}
				break;
			case KeyPressType.KeyRepeat:
				if (Input.GetKey(keyCode))
				{
					ExecuteSequence();
				}
				break;
			}
		}
	}
}