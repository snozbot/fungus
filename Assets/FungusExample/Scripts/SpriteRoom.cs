using UnityEngine;
using System.Collections;

namespace Fungus.Example
{
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

			SetCharacter("PinkAlienHappy");
			Say("Hey Blue Alien!");
			Say("Show me your funky moves!");

			SetCharacter("BlueAlienHappy");
			Say("Watch and learn Pinky!");
			SetAnimatorTrigger(blueAlienAnim, "StartBlueWalk");

			Wait(4);

			SetAnimatorTrigger(blueAlienAnim, "Stop");

			SetCharacter("PinkAlienHappy");
			Say("Nice moves there Blue Alien!");
			Say("Would you like a nice mushroom to sit down on?");

			FadeSprite(redMushroomSprite, 1f, 1f);

			SetCharacter("BlueAlienSad");
			Say("I'd love to, but alas I cannot!");
			Say("The artist didn't make a sitting animation for me.");
			Say("It's a very rare genetic condition. Sniff.");

			SetCharacter("PinkAlienSad");
			Say("Oh! Sorry to hear that.");

			Wait(1f);
			FadeSprite(redMushroomSprite, 0f, 1f);

			SetCharacter("PinkAlienSad");
			Say("Uh... are you ok?");
			Say("Looks like you're turning a little bit green after all that dancing!");

			SetAnimatorTrigger(blueAlienAnim, "StartGreenWalk");
			Wait(2f);

			SetCharacter("PinkAlienHappy");
			Say("Never mind, I'm sure you'll feel better soon!");
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
}
