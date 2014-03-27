Using the Command Queue
=======================

# How do I wait between commands?

- To wait for a period of time, use the [Wait](@ref Fungus.GameController.Wait) command.
- To wait for the player to tap or click, use the [WaitForInput](@ref Fungus.GameController.WaitForInput) command.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	void OnEnter() 
	{
		Say("Hello");
		Wait(5); // Wait for 5 seconds
		Say("World");
		WaitForInput(); // Wait for player to tap / click
		Say("Goodbye");
	}
}
~~~~~~~~~~~~~~~~~~~~

- - -

# How do I execute a method as part of a command sequence?

The [Call](@ref Fungus.GameController.Call) commmand allows you to call a delegate method as part of a command sequence. This allows for code reuse as a you can place a common command sequence into a method and then use the Call command to invoke it from multiple locations in your code.

## C# Code Example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	void OnEnter() 
	{
		Call(SayHello);
		Wait(5);
		Call(SayHello); // SayHello is called after a delay of 5 seconds
	}

	void SayHello()
	{
		Say("Hello");
	}

}
~~~~~~~~~~~~~~~~~~~~

- - -

# How do I control when Fungus commands start executing?

It is possible to only use Fungus for certain parts of your game (e.g. for cutscenes). In this scenario we would like to trigger the execution of commands based on an arbitrary gameplay event. To do this, just add your commands to the command queue and then tell it to execute manually.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

// A component class that does not inherit from Room
public class MyGameObject : MonoBehavior 
{
	public Room startRoom; // The Room we want to start Fungus execution with

	void Start() 
	{
		// Ensure the command queue is empty
		Game.Clear();

		// Add any required Fungus commands
		// It's a good idea to move to a start Room to ensure that everything is setup correctly.
		// You can then add whatever commands are needed in the OnEnter() method for the Room.
		Game.MoveToRoom(startRoom);

		// Start executing queued commands
		Game.Execute();
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes
- Fungus commands can be called anywhere in your code by using the inherited static methods on the [Game class](@ref Fungus.Game).
- For convenience, the "Game." prefix is optional when invoking commands in a script that inherits from the [Room class](@ref Fungus.Room).
- In scripts that do not inherit from Room, you must use the "Game." prefix.
