using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Listener component to handle animation events.
	 * Usage: 
	 * 1. Attach this script to the animated sprite that you want to listen to for events.
	 * 2. Add an event on the animation timeline 
	 * 3. Edit the event and choose the 'CallRoomMethod' function
	 * 4. In the string parameters box, enter the name of the method to call in the active Room's script.
	 */
	public class AnimationListener : MonoBehaviour 
	{
		/**
		 * Handler method for animation events.
		 * The string event parameter is used to call a named method on the active room class
		 */
		void CallRoomMethod(string methodName)
		{
			// TODO: Execute a FungusScript sequence
		}
	}
}