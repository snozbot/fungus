Using Sprites
=============

# How do I control sprite visibility?

1. Add a public SpriteRenderer property to your Room script and setup a reference to your SpriteRenderer object in the inspector.
2. Use the [ShowSprite](@ref Fungus.GameController.ShowSprite) command to make a sprite visible instantly.
3. Use the [HideSprite](@ref Fungus.GameController.HideSprite) command to make a sprite invisible instantly.
4. Use the [FadeSprite](@ref Fungus.GameController.FadeSprite) command to fade a sprite in or out over a period of time.

## C# Code Example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	public SpriteRenderer mySprite; 

	void OnEnter() 
	{
		HideSprite(mySprite); // Sets sprite alpha to 0 (invisible)
		Wait(5);
		ShowSprite(mySprite); // Sets sprite alpha to 1 (fully visible)
		Wait(5);
		FadeSprite(mySprite, 0f, 5f); // Fades sprite alpha to 0 over 5 seconds
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes
- The [FadeSprite](@ref Fungus.GameController.FadeSprite) command does not pause command execution. This allows for fading multiple sprites simultaneously.
- Use a [Wait](@ref Fungus.GameController.Wait) command if you need to wait for a sprite fade to finish before continuing.

- - -

# How do I make Buttons?

TODO: Document this!

- - -

# How do I trigger an animation?

TODO: Document this!

- - -

# How do I listen for Animation Events?

TODO: Document this!
