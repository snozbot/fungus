using UnityEngine;
using System.Collections;
using Fungus;

/**
 * This class is a template to use as a starting point for your own Room scripts.
 * 1. Select this script in the Project window in Unity3D
 * 2. Choose Edit > Duplicate from the menu. A copy of the file will be created.
 * 3. Rename the file to match the name of your room (e.g. DungeonRoom)
 * 4. Edit the script and rename the class to match the file name (e.g. public class RoomTemplate => public class DungeonRoom)
 * 5. Save the script and add it as a component to your Room game object in Unity 3D.
 */
public class RoomTemplate : Room 
{
	// Add public properties here. 
	// These will appear in the inspector window in Unity so you can connect them to objects in your scene

	// Some common examples:
	// public View mainView;
	// public Page dialogPage;
	// public Room otherRoom;
	// public SpriteRenderer characterSprite;
	// public Animator characterAnim;
	// public AudioClip musicClip;

	/** 
	 * OnEnter() is always called when the player enters the room
	 */
	void OnEnter()
	{
		// Add any sequence of Fungus commands you want here.
		// See FungusExample/Scripts for examples
	}	
}
