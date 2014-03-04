using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 * This class gives easy access to every scripting command available in Fungus.
	 */
	public class GameController : MonoBehaviour 
	{
		#region General Methods
		
		/**
		 * Transitions from the current Room to another Room.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param room The Room to transition to.
		 */
		public void MoveToRoom(Room room)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new MoveToRoomCommand(room));
		}
		
		/**
		 * Wait for a period of time before executing the next command.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param duration The wait duration in seconds
		 */
		public void Wait(float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new WaitCommand(duration));
		}
		
		/**
		 * Call a delegate method provided by the client.
		 * Used to queue the execution of arbitrary code as part of a command sequeunce.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param callAction The Action delegate method to be called when the command executes.
		 */
		public void Call(Action callAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new CallCommand(callAction));
		}
		
		#endregion
		#region View Methods

		/**
		 * Sets the currently active View immediately.
		 * The main camera snaps to the new active View. If the View contains a Page object, this Page becomes the active Page.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param view The View object to make active
		 */
		public void SetView(View view)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetViewCommand(view));
		}

		/**
		 * Pans the camera to the target View over a period of time.
		 * The pan starts at the current camera position and performs a smoothed linear pan to the target View.
		 * Command execution blocks until the pan completes. When the camera arrives at the target View, this View becomes the active View.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param targetView The View to pan the camera to.
		 * @param duration The length of time in seconds needed to complete the pan.
		 */
		public void PanToView(View targetView, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new PanToViewCommand(targetView, duration));
		}
		
		/**
		 * Pans the camera through a sequence of target Views over a period of time.
		 * The pan starts at the current camera position and performs a smooth pan through all Views in the list.
		 * Command execution blocks until the pan completes. When the camera arrives at the last View in the list, this View becomes the active View.
		 * If more control is required over the camera path then you should instead use an Animator component to precisely control the Camera path.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param duration The length of time in seconds needed to complete the pan.
		 * @param targetViews A parameter list of views to visit during the pan.
		 */
		public void PanToPath(float duration, params View[] targetViews)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new PanToPathCommand(targetViews, duration));
		}
		
		/**
		 * Fades out the current camera View, and fades in again using the target View.
		 * If the target View contains a Page object, this Page becomes the active Page.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param targetView The View object to fade to.
		 * @param duration The length of time in seconds needed to complete the pan.
		 */
		public void FadeToView(View targetView, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new FadeToViewCommand(targetView, duration));
		}

		#endregion
		#region Page Methods

		/**
		 * Sets the currently active Page for story text display.
		 * Once this command executes, all story text added using Say(), AddOption(), Choose(), etc. will be displayed on this Page.
		 * When a Room contains multiple Page objects, this method can be used to control which Page object is active at a given time.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param page The Page object to make active
		 */
		public void SetPage(Page page)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetPageCommand(page));
		}

		/**
		 * Sets the currently active style for displaying Pages.
		 * Once this command executes, all Pages will display using the new style.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param pageStyle The style object to make active
		 */
		public void SetPageStyle(PageStyle pageStyle)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetPageStyleCommand(pageStyle));
		}

		/**
		 * Sets the title text displayed at the top of the active Page.
		 * The title text is only displayed when there is some story text or options to be shown.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param titleText The text to display as the title of the Page.
		 */
		public void Title(string titleText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new TitleCommand(titleText));
		}

		/**
		 * Writes story text to the currently active Page.
		 * A 'continue' button is displayed when the text has fully appeared.
		 * Command execution halts until the user chooses to continue.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param storyText The text to be written to the currently active Page.
		 */
		public void Say(string storyText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SayCommand(storyText));
		}
		
		/**
		 * Adds an option to the current list of player options.
	     * Use the Choose() method to display previously added options.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param optionText The text to be displayed for this option
		 * @param optionAction The Action delegate method to be called when the player selects the option
		 */
		public void AddOption(string optionText, Action optionAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new AddOptionCommand(optionText, optionAction));
		}

		/**
		 * Display all previously added options as buttons, with no text prompt.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		public void Choose()
		{
			Choose("");
		}
		
		/**
		 * Displays a story text prompt, followed by all previously added options.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param chooseText The story text to be written above the list of options
		 */
		public void Choose(string chooseText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new ChooseCommand(chooseText));
		}

		#endregion
		#region State Methods
		
		/**
		 * Sets a boolean flag value.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the flag
		 * @param value The boolean value to set the flag to
		 */
		public void SetFlag(string key, bool value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetFlagCommand(key, value));
		}
		
		/**
		 * Gets the current state of a flag.
		 * Flag values are set using SetFlag().
		 * Returns false if the flag has previously been set to false, or has not yet been set.
		 * @param key The name of the flag
		 * @return The boolean state of the flag.
		 */
		public bool GetFlag(string key)
		{
			GameState state = Game.GetInstance().state;
			return state.GetFlag(key);
		}

		/**
		 * Sets an integer counter value.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the counter
		 * @param value The value to set the counter to
		 */
		public void SetCounter(string key, int value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetCounterCommand(key, value));
		}
		
		/**
		 * Gets the current value of a counter.
	     * Counter values are set using SetCounter().
		 * Returns zero if the counter has not previously been set to a value.
		 * @param key The name of the counter
		 * @return The current value of the counter
		 */
		public int GetCounter(string key)
		{
			GameState state = Game.GetInstance().state;
			return state.GetCounter(key);
		}

		/**
		 * Sets an inventory item count to 1.
		 * This supports the common case where the player can only collect 1 instance of an inventory item.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the inventory item
		 */
		public void SetInventory(string key)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetInventoryCommand(key, 1));
		}
		
		/**
		 * Sets the inventory count for an item.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the inventory item
		 * @param value The inventory count for the item
		 */
		public void SetInventory(string key, int value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetInventoryCommand(key, value));
		}
		
		/**
		 * Gets the current inventory count for an item.
	     * Inventory counts are set using SetInventory().
		 * Returns zero if the inventory count has not previously been set to a value.
		 * @param key The name of the inventory item
		 * @return The current inventory count of the item
		 */
		public int GetInventory(string key)
		{
			GameState state = Game.GetInstance().state;
			return state.GetInventory(key);
		}
		
		/**
		 * Check if player has an inventory item.
		 * @param key The name of the inventory item
		 * @return Returns true if the inventory count for an item is greater than zero
		 */
		public bool HasInventory(string key)
		{
			GameState state = Game.GetInstance().state;
			return (state.GetInventory(key) > 0);
		}

		#endregion
		#region Sprite Methods
		
		/**
		 * Makes a sprite completely transparent immediately.
		 * This changes the alpha component of the sprite renderer color to 0.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 */
		public void HideSprite(SpriteRenderer spriteRenderer)
		{
			FadeSprite(spriteRenderer, 0, 0, Vector2.zero);
		}
		
		/**
		 * Makes a sprite completely opaque immediately.
		 * This changes the alpha component of the sprite renderer color to 1.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 */
		public void ShowSprite(SpriteRenderer spriteRenderer)
		{
			FadeSprite(spriteRenderer, 1, 0, Vector2.zero);
		}
		
		/**
		 * Fades the transparency of a sprite over a period of time.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 * @param targetAlpha The final required transparency value [0..1]
		 * @param duration The duration of the fade transition in seconds
		 */
		public void FadeSprite(SpriteRenderer spriteRenderer, float targetAlpha, float duration)
		{
			FadeSprite(spriteRenderer, targetAlpha, duration, Vector2.zero);
		}
		
		/**
		 * Fades the transparency of a sprite over a period of time, and applies a sliding motion to the sprite's position.
		 * The sprite starts at a position calculated by adding the current sprite position + the slide offset.
		 * The sprite then smoothly moves from this starting position to the original position of the sprite.
		 * Automatically adds a SpriteFader component to the sprite object to perform the transition.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 * @param targetAlpha The final required transparency value [0..1]
		 * @param duration The duration of the fade transition in seconds
		 * @param slideOffset Offset to the starting position for the slide effect.
		 */
		public void FadeSprite(SpriteRenderer spriteRenderer, float targetAlpha, float duration, Vector2 slideOffset)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			Color color = spriteRenderer.color;
			color.a = targetAlpha;
			commandQueue.AddCommand(new FadeSpriteCommand(spriteRenderer, color, duration, slideOffset));
		}
		
		/**
		 * Makes a sprite behave as a clickable button.
		 * Automatically adds a Button component to the object to respond to player input.
		 * If no Collider2D already exists on the object, then a BoxCollider2D is automatically added.
		 * Use RemoveButton() to make a sprite non-clickable again.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be made clickable
		 * @param buttonAction The Action delegate method to be called when the player clicks on the button
		 */
		public void AddButton(SpriteRenderer spriteRenderer, Action buttonAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new AddButtonCommand(spriteRenderer, buttonAction));
		}
		
		/**
		 * Makes a sprite stop behaving as a clickable button.
		 * Removes the Button component from the sprite object.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be made non-clickable
		 */
		public void RemoveButton(SpriteRenderer spriteRenderer)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new RemoveButtonCommand(spriteRenderer));
		}
		
		/**
		 * Sets an animator trigger to change the animation state for an animated sprite.
		 * This is the primary method of controlling Unity animations from a Fungus command sequence.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param animator The sprite to be made non-clickable
		 * @param triggerName Name of a trigger parameter in the animator controller
		 */
		public void SetAnimatorTrigger(Animator animator, string triggerName)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetAnimatorTriggerCommand(animator, triggerName));
		}

		#endregion
		#region Audio Methods
		
		/**
		 * Plays game music using an audio clip.
		 * One music clip may be played at a time.
		 * @param audioClip The music clip to play
		 */
		public void PlayMusic(AudioClip audioClip)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new PlayMusicCommand(audioClip));
		}
		
		/**
		 * Stops playing game music.
		 */
		public void StopMusic()
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new StopMusicCommand());
		}
		
		/**
		 * Sets the game music volume immediately.
		 * @param musicVolume The new music volume value [0..1]
		 */
		public void SetMusicVolume(float musicVolume)
		{
			SetMusicVolume(musicVolume, 0f);
		}
		
		/**
		 * Fades the game music volume to required level over a period of time.
		 * @param musicVolume The new music volume value [0..1]
		 * @param duration The length of time in seconds needed to complete the volume change.
		 */
		public void SetMusicVolume(float musicVolume, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new SetMusicVolumeCommand(musicVolume, duration));
		}
		
		/**
		 * Plays a sound effect once.
		 * Multiple sound effects can be played at the same time.
		 * @param audioClip The sound effect clip to play
		 */
		public void PlaySound(AudioClip audioClip)
		{
			PlaySound(audioClip, 1f);
		}
		
		/**
		 * Plays a sound effect once, at the specified volume.
		 * Multiple sound effects can be played at the same time.
		 * @param audioClip The sound effect clip to play
		 * @param volume The volume level of the sound effect
		 */
		public void PlaySound(AudioClip audioClip, float volume)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new PlaySoundCommand(audioClip, volume));
		}

		#endregion
	}
}