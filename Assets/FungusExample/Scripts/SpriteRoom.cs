using UnityEngine;
using System.Collections;
using Fungus;

public class SpriteRoom : Room 
{
	public Room menuRoom;

	public Animator blueAlienAnim;
	public SpriteRenderer blueAlienSprite;
	public SpriteRenderer redMushroomSprite;

	void OnEnter() 
	{	
		HideSprite(redMushroomSprite);

		ShowSprite(blueAlienSprite);

		Say("Pink Alien says to Blue Alien...");
		Say("...'Show me your funky moves!'");

		SetAnimatorTrigger(blueAlienAnim, "StartBlueWalk");

		Say("Blue Alien starts to dance.");
		Say("Tap on Blue Alien to stop him dancing.");

		AddButton(blueAlienSprite, StopDancing);
	}	

	// This method is called from the Button component on the BlueAlien object
	void StopDancing()
	{
		RemoveButton(blueAlienSprite);

		SetAnimatorTrigger(blueAlienAnim, "Stop");

		Say("Nice moves there Blue Alien!");

		FadeSprite(redMushroomSprite, 1f, 1f);

		Say("Maybe you want a nice mushroom to sit down on?");
		Say("Don't want to sit? Ok, no problem.");

		FadeSprite(redMushroomSprite, 0f, 1f);

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
