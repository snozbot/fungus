using UnityEngine;
using System.Collections;
using Fungus;

public class ButtonRoom : Room 
{
	public Room menuRoom;

	public AudioClip effectClip;

	public SpriteRenderer homeSprite;
	public SpriteRenderer musicSprite;
	public SpriteRenderer questionSprite;
	
	void OnEnter()
	{
		// Normal button, always visible
		AddButton(homeSprite, OnHomeClicked);

		// Auto buttons, hidden when story/options are being displayed
		AddAutoButton(musicSprite, OnMusicClicked);
		AddAutoButton(questionSprite, OnQuestionClicked);
	
		// NOTE: Add auto buttons before first Say() command to ensure they start hidden

		Say("The Mushroom read his book with great interest.");
		Say("After turning the last page, he considered his options.");

	}

	void OnHomeClicked()
	{
		MoveToRoom(menuRoom);
	}

	void OnMusicClicked()
	{
		PlaySound(effectClip);
	}

	void OnQuestionClicked()
	{
		Say("What book was he reading?");
		Say("Perhaps we will never know for sure.");
	}
}
