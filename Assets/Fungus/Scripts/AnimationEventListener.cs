using UnityEngine;
using System.Collections;
using Fungus;

// Listens for animation events
// The string parameter specifies a method name on the active room class
public class AnimationEventListener : MonoBehaviour 
{
	void OnAnimationEvent(string methodName)
	{
		Room room = Game.GetInstance().activeRoom;
		if (room == null)
		{
			return;
		}

		room.AnimationEvent(methodName);
	}
}
