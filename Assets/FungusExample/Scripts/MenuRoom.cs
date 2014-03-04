using UnityEngine;
using System.Collections;
using Fungus;

public class MenuRoom : Room
{
	public Room pageRoom;
	public Room viewRoom;
	public Room spriteRoom;
	public Room audioRoom;

	void OnEnter() 
	{
		AddOption("1. Writing a story with Pages", MoveToWriting);
		AddOption("2. Controlling the camera with Views", MoveToViews);
		AddOption("3. Using Sprites and Animations", MoveToAnimations);
		AddOption("4. Playing music and sound effects", MoveToAudio);
		Choose("Choose an example");
	}

	void MoveToWriting()
	{
		MoveToRoom(pageRoom);
	}

	void MoveToViews()
	{
		MoveToRoom(viewRoom);
	}

	void MoveToAnimations()
	{
		MoveToRoom(spriteRoom);
	}

	void MoveToAudio()
	{
		MoveToRoom(audioRoom);
	}
}
