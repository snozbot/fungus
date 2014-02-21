using UnityEngine;
using System.Collections;
using Fungus;

public class AnimationEventListener : MonoBehaviour 
{
	void OnAnimationEvent(string eventName)
	{
		Room room = Game.GetInstance().activeRoom;
		if (room == null)
		{
			return;
		}

		room.AnimationEvent(eventName);
	}
}
