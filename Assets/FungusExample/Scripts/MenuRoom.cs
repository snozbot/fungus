using UnityEngine;
using System.Collections;
using Fungus;

public class MenuRoom : Room
{
	public Room pageRoom;
	public Room viewRoom;
	public Room spriteRoom;
	public Room buttonRoom;
	public Room audioRoom;

	void OnEnter() 
	{
		AddOption("Writing a story with Pages", MoveToWritingRoom);
		AddOption("Controlling the camera with Views", MoveToViewRoom);
		AddOption("Sprites and Animations", MoveToSpriteRoom);
		AddOption("Using Buttons", MoveToButtonsRoom);
		AddOption("Playing music and sound effects", MoveToAudioRoom);
		Choose("Choose an example");
	}

	void MoveToWritingRoom()
	{
		MoveToRoom(pageRoom);
	}

	void MoveToViewRoom()
	{
		MoveToRoom(viewRoom);
	}

	void MoveToSpriteRoom()
	{
		MoveToRoom(spriteRoom);
	}

	void MoveToButtonsRoom()
	{
		MoveToRoom(buttonRoom);
	}

	void MoveToAudioRoom()
	{
		MoveToRoom(audioRoom);
	}
}
