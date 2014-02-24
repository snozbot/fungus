using UnityEngine;
using System.Collections;
using Fungus;

public class SpritesRoom : Room 
{
	public Room menuRoom;
	public Animator blueAlienAnim;
	public SpriteController blueAlienSprite;

	void OnEnter() 
	{	
		ShowSprite(blueAlienSprite);

		Say("Pink Alien says to Blue Alien...");
		Say("...'Show me your funky moves!'");

		SetAnimatorTrigger(blueAlienAnim, "StartBlueWalk");

		Say("Blue Alien starts to dance.");
		Say("Tap on Blue Alien to stop him dancing.");
	}	

	// This method is called from the Button component on the BlueAlien object
	void StopDancing()
	{
		SetAnimatorTrigger(blueAlienAnim, "Stop");

		Say("Nice moves there Blue Alien!");
		Say("Uh oh, you look like you're turning a little green after all that dancing!");

		SetAnimatorTrigger(blueAlienAnim, "StartGreenWalk");

		Say("Never mind, you'll feel better soon!");
	}

	void OnAnimationEvent(string eventName)
	{
		if (eventName == "GreenAnimationFinished")
		{
			SetAnimatorTrigger(blueAlienAnim, "Stop");

			Say("Well done Blue Alien! Time to say goodbye!");

			FadeSprite(blueAlienSprite, 0, 1f);
			Wait(1f);

			Say("Heh. That Blue Alien - what a guy!");

			MoveToRoom(menuRoom);
		}
	}
}
