using UnityEngine;
using System.Collections;
using Fungus;

public class SpritesRoom : Room 
{
	public Room menuRoom;
	public Animator blueAlienAnim;
	public SpriteRenderer blueAlienSprite;
	public SpriteRenderer redMushroomSprite;

	void OnEnter() 
	{	
		HideSprite(redMushroomSprite);
		FadeSprite(redMushroomSprite, 1f, 2f);

		ShowSprite(blueAlienSprite);

		Say("Pink Alien says to Blue Alien...");
		Say("...'Show me your funky moves!'");

		SetAnimatorTrigger(blueAlienAnim, "StartBlueWalk");

		Say("Blue Alien starts to dance.");
		Say("Tap on Blue Alien to stop him dancing.");

		FadeSprite(redMushroomSprite, 0f, 2f);
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

	// This method is called by the Animation Event Listener component on the blue alien.
	// When the GreenAlienWalk animation finishes it fires an event which calls this method.
	void AlienAnimationFinished()
	{
		SetAnimatorTrigger(blueAlienAnim, "Stop");

		Say("Well done Blue Alien! Time to say goodbye!");

		FadeSprite(blueAlienSprite, 0, 1f);
		Wait(1f);

		Say("Heh. That Blue Alien - what a guy!");

		MoveToRoom(menuRoom);
	}
}
