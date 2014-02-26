using UnityEngine;
using System;
using System.Collections;
using Fungus;

// This facade class gives easy access to all game control 
// functionality available in Fungus
public class GameController : MonoBehaviour 
{
	//
	// Synchronous methods
	// The following methods all execute immediately
	//
	
	// Return true if the boolean flag for the key has been set to true
	public bool GetFlag(string key)
	{
		GameState state = Game.GetInstance().state;
		return state.GetFlag(key);
	}
	
	// Returns the count value for the key
	// Returns zero if no value has been set.
	public int GetCounter(string key)
	{
		GameState state = Game.GetInstance().state;
		return state.GetCounter(key);
	}
	
	// Returns the inventory count value for the key
	// Returns zero if no inventory count has been set.
	public int GetInventory(string key)
	{
		GameState state = Game.GetInstance().state;
		return state.GetInventory(key);
	}
	
	// Returns true if the inventory count for the key is greater than zero
	public bool HasInventory(string key)
	{
		GameState state = Game.GetInstance().state;
		return (state.GetInventory(key) > 0);
	}

	//
	// Asynchronous methods
	// The following methods all queue commands for later execution in strict serial order
	//
	
	// Wait for a period of time before executing the next command
	public void Wait(float duration)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new WaitCommand(duration));
	}
	
	// Call a delegate method provided by the client
	// Used to queue the execution of arbitrary code.
	public void Call(Action callAction)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new CallCommand(callAction));
	}
	
	// Sets the currently active view immediately.
	// The main camera snaps to the active view.
	public void SetView(View view)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetViewCommand(view));
	}
	
	// Sets the currently active page for text rendering
	public void SetPage(Page page)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetPageCommand(page));
	}
	
	// Sets the title text displayed at the top of the active page
	public void Title(string titleText)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new TitleCommand(titleText));
	}
	
	// Writes story text to the currently active page.
	// A 'continue' button is displayed when the text has fully appeared.
	public void Say(string storyText)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SayCommand(storyText));
	}
	
	// Adds an option button to the current list of options.
	// Use the Choose command to display added options.
	public void AddOption(string optionText, Action optionAction)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new AddOptionCommand(optionText, optionAction));
	}

	// Display all previously added options as buttons, with no text prompt
	public void Choose()
	{
		Choose("");
	}
	
	// Displays a text prompt, followed by all previously added options as buttons.
	public void Choose(string chooseText)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new ChooseCommand(chooseText));
	}
	
	// Changes the active room to a different room
	public void MoveToRoom(Room room)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new MoveToRoomCommand(room));
	}
	
	// Sets a global boolean flag value
	public void SetFlag(string key, bool value)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetFlagCommand(key, value));
	}
	
	// Sets a global integer counter value
	public void SetCounter(string key, int value)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetCounterCommand(key, value));
	}
	
	// Sets a global inventory count value
	// Assumes that the count value is 1 (common case)
	public void SetInventory(string key)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetInventoryCommand(key, 1));
	}
	
	// Sets a global inventory count value
	public void SetInventory(string key, int value)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetInventoryCommand(key, value));
	}
	
	// Sets sprite alpha to 0 immediately
	public void HideSprite(SpriteRenderer spriteRenderer)
	{
		FadeSprite(spriteRenderer, 0, 0, Vector2.zero);
	}
	
	// Sets sprite alpha to 1 immediately
	public void ShowSprite(SpriteRenderer spriteRenderer)
	{
		FadeSprite(spriteRenderer, 1, 0, Vector2.zero);
	}
	
	// Fades a sprite to a given alpha value over a period of time
	public void FadeSprite(SpriteRenderer spriteRenderer, float targetAlpha, float duration)
	{
		FadeSprite(spriteRenderer, targetAlpha, duration, Vector2.zero);
	}
	
	// Fades a sprite to a given alpha value over a period of time, and applies a sliding motion to the sprite transform
	public void FadeSprite(SpriteRenderer spriteRenderer, float targetAlpha, float duration, Vector2 slideOffset)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		Color color = spriteRenderer.color;
		color.a = targetAlpha;
		commandQueue.AddCommand(new FadeSpriteCommand(spriteRenderer, color, duration, slideOffset));
	}
	
	// Makes a sprite behave as a clickable button
	public void AddButton(SpriteRenderer buttonSprite, Action buttonAction)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new AddButtonCommand(buttonSprite, buttonAction));
	}
	
	// Makes a sprite stop behaving as a clickable button
	public void RemoveButton(SpriteRenderer buttonSprite)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new RemoveButtonCommand(buttonSprite));
	}
	
	// Sets an animator trigger to change the animation state for an animated sprite
	public void SetAnimatorTrigger(Animator animator, string triggerName)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetAnimatorTriggerCommand(animator, triggerName));
	}
	
	// Pans the camera to the target view over a period of time
	public void PanToView(View targetView, float duration)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new PanToViewCommand(targetView, duration));
	}
	
	// Pans the camera through a sequence of target views over a period of time
	public void PanToPath(float duration, params View[] targetViews)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new PanToPathCommand(targetViews, duration));
	}
	
	// Snaps the camera to the target view immediately
	public void SnapToView(View targetView)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new PanToViewCommand(targetView, 0f));
	}
	
	// Fades out the current camera view, and fades in again using the target view.
	public void FadeToView(View targetView, float duration)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new FadeToViewCommand(targetView, duration));
	}
	
	// Plays game music using an audio clip
	public void PlayMusic(AudioClip audioClip)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new PlayMusicCommand(audioClip));
	}
	
	// Stops playing game music
	public void StopMusic()
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new StopMusicCommand());
	}
	
	// Sets music volume immediately
	public void SetMusicVolume(float musicVolume)
	{
		SetMusicVolume(musicVolume, 0f);
	}
	
	// Fades music volume to required level over a period of time
	public void SetMusicVolume(float musicVolume, float duration)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new SetMusicVolumeCommand(musicVolume, duration));
	}
	
	// Plays a sound effect once
	public void PlaySound(AudioClip audioClip)
	{
		PlaySound(audioClip, 1f);
	}
	
	// Plays a sound effect once, at the specified volume
	public void PlaySound(AudioClip audioClip, float volume)
	{
		CommandQueue commandQueue = Game.GetInstance().commandQueue;
		commandQueue.AddCommand(new PlaySoundCommand(audioClip, volume));
	}
}