Using Rooms
===========

A [Room](@ref Fungus.Room) is a Game Object that contains a script that inherits from the [Room class](@ref Fungus.Room). Rooms are the main unit of organization in a Fungus game. They are similar to locations in a point and click adventure game.

Rooms may be connected to each other via public Room properties. Connected Rooms are automatically shown in the Unity scene view via a connecting arrow.
Rooms may contain [Views](@ref Fungus.View), [Pages](@ref Fungus.Page), Sprites, [Buttons](@ref Fungus.Button) and any other type of Game Object required to construct the location.

- - -

# How do I add a Room to a scene?

Note: Every Room in a Fungus game must have an attached script component which inherits from the [Room class](@ref Fungus.Room). This script controls all Fungus related behavior for that Room.

1. Create an instance of the Fungus/Prefabs/Room.prefab in the Fungus library. This creates a standard Room, but with no Room script attached yet.
2. Create a new c# script in your project folder. Give it the same name as your Room.
3. Add a `using Fungus;` declaration at the top of the script to allow easy access to the Fungus library.
4. Add a `void OnEnter()` method. This will be called every time the player enters the room.

## C# Code Example

~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	void OnEnter() 
	{
		Say("Hello world!");
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes
- The default start() and update() methods are not typically used in Fungus games. Feel free to delete them.
- A documented example Room script is provided in Fungus/Scripts/RoomTemplate.cs in the Fungus library.

- - -

# How do I move between Rooms?

1. Ensure there are at least two Room objects in your scene.
2. Add a public Room property to your Room script and setup the reference to the other Room in the inspector.
3. Use the [MoveToRoom](@ref Fungus.GameController.MoveToRoom) command to transition to the other Room.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	public Room otherRoom; // Another Room

	void OnEnter() 
	{
		Wait(5);
		MoveToRoom(otherRoom); // Current Room fades out, new Room fades in
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes
- Use [Game.RoomFadeDuration](@ref Fungus.Game.roomFadeDuration) to control the fade transition time.

- - -

# How do I check if a Room has already been Visited?

The [Room.visitCount](@ref Fungus.Room.visitCount) property tracks how many times the player has entered a Room. 

Use the [Room.IsFirstVisit](@ref Fungus.Room.IsFirstVisit) method to check if this is the first visit to the Room.

## C# Code Example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	void OnEnter() 
	{
		if (IsFirstVisit())
		{
			Say("Welcome to the room stranger!");
		}
		else
		{
			Say("Nice to have you back!");
		}
	}
}
~~~~~~~~~~~~~~~~~~~~
