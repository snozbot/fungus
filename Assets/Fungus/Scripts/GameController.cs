using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 * This class gives easy static access to every scripting command available in Fungus.
	 */
	public class GameController : MonoBehaviour 
	{
		#region Game Methods

		/**
		 * Clears the command queue.
		 */
		public static void Clear()
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.Clear();
		}

		/**
		 * Executes the commadns in the command queue.
		 */
		public static void Execute()
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.Execute ();
		}

		/**
		 * Transitions from the current Room to another Room.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param room The Room to transition to.
		 */
		public static void MoveToRoom(Room room)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.MoveToRoom(room));
		}

		/**
		 * Transitions to a different scene.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param sceneName The name of the scene to transition to.
		 */
		public static void MoveToScene(string sceneName)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.MoveToScene(sceneName));
		}

		/**
		 * Wait for a period of time before executing the next command.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param duration The wait duration in seconds
		 */
		public static void Wait(float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Wait(duration));
		}

		/**
		 * Wait until player taps, clicks or presses a key before executing the next command.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		public static void WaitForInput()
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.WaitForInput());
		}

		/**
		 * Call a delegate method provided by the client.
		 * Used to queue the execution of arbitrary code as part of a command sequeunce.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param callAction The Action delegate method to be called when the command executes.
		 */
		public static void Call(Action callAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(callAction));
		}

		/**
		 * Save the current game state to persistant storage.
		 * @param saveName The name of the saved game data.
		 */
		public static void Save(string saveName = "_fungus")
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Game.GetInstance().SaveGame(saveName);
			}));
		}

		/**
		 * Load the current game state from persistant storage.
		 * @param saveName The name of the saved game data.
		 */
		public static void Load(string saveName = "_fungus")
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Game.GetInstance().LoadGame(saveName);
			}));
		}

		#endregion
		#region View Methods

		/**
		 * Sets the currently active View immediately.
		 * The main camera snaps to the new active View.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param view The View object to make active
		 */
		public static void SetView(View view)
		{
			PanToView(view, 0);
		}

		/**
		 * Pans the camera to the target View over a period of time.
		 * The pan starts at the current camera position and performs a smoothed linear pan to the target View.
		 * Command execution blocks until the pan completes.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param targetView The View to pan the camera to.
		 * @param duration The length of time in seconds needed to complete the pan.
		 * @param wait Wait for pan to finish before executing next command.
		 */
		public static void PanToView(View targetView, float duration, bool wait = true)
		{
			PanToPosition(targetView.transform.position, targetView.transform.rotation, targetView.viewSize, duration, wait);
		}

		/**
		 * Pans the camera to the target position and size over a period of time.
		 * The pan starts at the current camera position and performs a smoothed linear pan to the target.
		 * Command execution blocks until the pan completes.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param targetPosition The position to pan the camera to.
		 * @param targetRotation The rotation to pan the camera to.
		 * @param targetSize The orthographic size to pan the camera to.
		 * @param duration The length of time in seconds needed to complete the pan.
		 * @param wait Wait for pan to finish before executing next command.
		 */
		public static void PanToPosition(Vector3 targetPosition, Quaternion targetRotation, float targetSize, float duration, bool wait)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.PanToPosition(targetPosition, targetRotation, targetSize, duration, wait));
		}
		
		/**
		 * Pans the camera through a sequence of target Views over a period of time.
		 * Note: Does not support camera Rotation.
		 * The pan starts at the current camera position and performs a smooth pan through all Views in the list.
		 * Command execution blocks until the pan completes.
		 * If more control is required over the camera path then you should instead use an Animator component to precisely control the Camera path.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param duration The length of time in seconds needed to complete the pan.
		 * @param targetViews A parameter list of views to visit during the pan.
		 */
		[Obsolete("Use a Camera animation instead.")]
		public static void PanToPath(float duration, params View[] targetViews)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.PanToPath(targetViews, duration));
		}
		
		/**
		 * Fades out the current camera View, and fades in again using the target View.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param targetView The View object to fade to.
		 * @param duration The length of time in seconds needed to complete the pan.
		 */
		public static void FadeToView(View targetView, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.FadeToView(targetView, duration));
		}

		/**
		 * Activates swipe panning mode.
		 * The camera first pans to the nearest point between the two views over a period of time.
		 * The player can then pan around the rectangular area defined between viewA & viewB.
		 * Swipe panning remains active until a StopSwipePan, SetView, PanToView, FadeToPath or FadeToView command is executed.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param viewA View object which defines one extreme of the pan area.
		 * @param viewB View object which defines the other extreme of the pan area.
		 */
		public static void StartSwipePan(View viewA, View viewB, float duration = 1f)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.StartSwipePan(viewA, viewB, duration));
		}

		/**
		 * Deactivates swipe panning mode.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		public static void StopSwipePan()
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.StopSwipePan());
		}

		/**
		 * Stores the current camera view using a name.
		 * You can later use PanToStoredView() to pan back to this stored position by name.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param viewName Name to associate with the stored camera view.
		 */
		public static void StoreView(string viewName = "")
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.StoreView(viewName));
		}
				
		/**
		 * Moves camera from the current position to a previously stored view over a period of time.
		 * @param duration Time needed for pan to complete.
		 * @param viewName Name of a camera view that was previously stored using StoreView().
		 */
		public static void PanToStoredView(float duration, string viewName = "")
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.PanToStoredView(viewName, duration));
		}

		/**
		 * Applies a random shake to the camera.
		 * @param x Horizontal shake amount in world units.
		 * @param x Vertical shake amount in world units.
		 * @param duration Length of time for shake effect to last.
		 */
		public static void ShakeCamera(float x, float y, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				iTween.ShakePosition(Camera.main.gameObject, new Vector3(x, y, 0), duration);
			}));
		}

		#endregion
		#region Dialog Methods

		/**
		 * Sets the Dialog object to use for displaying story text and options.
		 */
		public static void SetDialog(Dialog dialog)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Game.GetInstance().dialog = dialog;
			}));
		}

		/**
		 * Sets the active Character within the current active Dialog.
		 * Each character has a distinct name, color & image.
		 */
		public static void SetCharacter(string characterID)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Dialog dialog = Game.GetInstance().dialog;
				if (dialog != null)
				{
					dialog.SetCharacter(characterID);
				}
			}));
		}

		/**
		 * Writes story text to the dialog.
		 * A 'continue' button is displayed when the text has fully appeared.
		 * Command execution halts until the user chooses to continue.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param storyText The text to be written to the dialog.
		 */
		public static void Say(string storyText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Say(storyText));
		}
		
		/**
		 * Adds an option to the current list of player options.
	     * Use the Choose() method to display previously added options.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param optionText The text to be displayed for this option
		 * @param optionAction The Action delegate method to be called when the player selects the option
		 */
		public static void AddOption(string optionText, Action optionAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.AddOption(optionText, optionAction));
		}

		/**
		 * Adds an option with no action to the current list of player options.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param optionText The text to be displayed for this option
		 * @param optionAction The Action delegate method to be called when the player selects the option
		 */
		public static void AddOption(string optionText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.AddOption(optionText, delegate {}));
		}

		/**
		 * Sets a time limit for choosing from a list of options.
		 * The timer will activate the next time a Say() command is executed that displays options.
		 */
		public static void SetTimeout(float timeoutDuration, Action timeoutAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call( delegate {
				IDialog dialog = Game.GetInstance().GetDialog();
				if (dialog != null)
				{
					dialog.SetTimeout(timeoutDuration, timeoutAction);
				}
			}));
		}

		#endregion
		#region State Methods

		/**
		 * Sets a globally accessible integer value.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the value to set
		 * @param value The value to set
		 */
		[Obsolete("Use SetInteger() instead.")]
		public static void SetValue(string key, int value)
		{
			SetInteger(key, value);
		}

		/**
		 * Sets a globally accessible integer value to 1.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the value to set
		 */
		[Obsolete("Use SetInteger() instead")]
		public static void SetValue(string key)
		{
			SetInteger(key, 1);
		}

		/**
		 * Sets a globally accessible integer value to 0.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The key of the value.
		 */
		[Obsolete("Use Variables.DeleteKey() instead")]
		public static void ClearValue(string key)
		{
			Variables.SetInteger(key, 0);
			Variables.SetFloat(key, 0);
			Variables.SetBoolean(key, false);
		}

		/**
		 * Gets a globally accessible integer value.
		 * Returns zero if the value has not previously been set.
		 * @param key The name of the value
		 * @return The integer value for this key, or 0 if not previously set.
		 */
		[Obsolete("Use Variables.GetInteger() instead")]
		public static int GetValue(string key)
		{
			return Variables.GetInteger(key);
		}

		/**
		 * Gets a globally accessible string value.
		 * @param key The name of the value
		 * @return The string value for this key, or the empty string if not previously set.
		 */
		[Obsolete("Use Variables.GetString() instead")]
		public static string GetString(string key)
		{
			return Variables.GetString(key);
		}

		/**
		 * Checks if a value is non-zero.
		 * @param key The name of the value to check.
		 * @return Returns true if the value is not equal to zero.
		 */
		[Obsolete("Use Variables.GetInteger() or Variables.HasKey() instead")]
		public static bool HasValue(string key)
		{
			return Variables.GetInteger(key) != 0;
		}

		/**
		 * Sets a globally accessible boolean variable.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the boolean variable to set
		 * @param value The boolean value to set
		 */
		public static void SetBoolean(string key, bool value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Variables.SetBoolean(key, value);
			}));
		}

		/**
		 * Sets a globally accessible integer variable.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the integer variable to set
		 * @param value The integer value to set
		 */
		public static void SetInteger(string key, int value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Variables.SetInteger(key, value);
			}));
		}

		/**
		 * Sets a globally accessible float variable.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the float variable to set
		 * @param value The flaot value to set
		 */
		public static void SetFloat(string key, float value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Variables.SetFloat(key, value);
			}));
		}

		/**
		 * Sets a globally accessible string variable.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the variable to set
		 * @param value The string variable to set
		 */
		public static void SetString(string key, string value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				Variables.SetString(key, value);
			}));
		}

		/**
		 * Adds a delta amount to the named integer variable.
		 * @param key The name of the integer variable to set.
		 * @param delta The delta value to add to the variable.
		 */
		public static void AddInteger(string key, int delta)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				int value = Variables.GetInteger(key);
				Variables.SetInteger(key, value + delta);
			}));
		}

		/**
		 * Multiplies the named integer variable.
		 * @param key The name of the integer variable to set.
		 * @param multiplier The multiplier to multiply the integer variable by.
		 */
		public static void MultiplyInteger(string key, int multiplier)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				int value = Variables.GetInteger(key);
				Variables.SetInteger(key, value * multiplier);
			}));
		}

		/**
		 * Adds a delta amount to the named float variable.
		 * @param key The name of the float variable to set.
		 * @param delta The delta value to add to the variable.
		 */
		public static void AddFloat(string key, float delta)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				float value = Variables.GetFloat(key);
				Variables.SetFloat(key, value + delta);
			}));
		}

		/**
		 * Multiplies the named float variable.
		 * @param key The name of the float variable to set.
		 * @param multiplier The multiplier to multiply the float variable by.
		 */
		public static void MultiplyFloat(string key, float multiplier)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				float value = Variables.GetFloat(key);
				Variables.SetFloat(key, value * multiplier);
			}));
		}

		/**
		 * Toggles the state of a boolean variable.
		 * @param The name of the boolean variable to toggle.
		 */
		public static void ToggleBoolean(string key)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				bool value = Variables.GetBoolean(key);
				Variables.SetBoolean(key, !value);
			}));
		}

		#endregion
		#region Sprite Methods
		
		/**
		 * Makes a sprite completely transparent immediately.
		 * This changes the alpha component of the sprite renderer color to 0.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 */
		public static void HideSprite(SpriteRenderer spriteRenderer)
		{
			FadeSprite(spriteRenderer, 0, 0, Vector2.zero);
		}
		
		/**
		 * Makes a sprite completely opaque immediately.
		 * This changes the alpha component of the sprite renderer color to 1.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 */
		public static void ShowSprite(SpriteRenderer spriteRenderer)
		{
			FadeSprite(spriteRenderer, 1, 0, Vector2.zero);
		}

		/**
		 * Shows or hides a sprite immediately, depending on the visible parameter.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 * @param visible Shows the sprite when true, hides the sprite when false.
		 */
		public static void ShowSprite(SpriteRenderer spriteRenderer, bool visible)
		{
			if (visible)
			{
				ShowSprite(spriteRenderer);
			}
			else
			{
				HideSprite(spriteRenderer);
			}
		}

		/**
		 * Fades the transparency of a sprite over a period of time.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be modified
		 * @param targetAlpha The final required transparency value [0..1]
		 * @param duration The duration of the fade transition in seconds
		 */
		public static void FadeSprite(SpriteRenderer spriteRenderer, float targetAlpha, float duration)
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
		public static void FadeSprite(SpriteRenderer spriteRenderer, float targetAlpha, float duration, Vector2 slideOffset)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			Color color = spriteRenderer.color;
			color.a = targetAlpha;
			commandQueue.AddCommand(new Command.FadeSprite(spriteRenderer, color, duration, slideOffset));
		}

		/**
		 * Displays a button sprite object and sets the action callback method for the button.
		 * If no Collider2D already exists on the object, then a BoxCollider2D is automatically added.
		 * Use HideButton() to make the sprite invisible and non-clickable again.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param button The button component of the sprite object to be shown.
		 * @param buttonAction The Action delegate method to be called when the player clicks on the button
		 */
		public static void ShowButton(Button button, Action buttonAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.ShowButton(button, true, buttonAction));
		}

		/**
		 * Hides the button sprite and makes it stop behaving as a clickable button.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be made non-clickable
		 */
		public static void HideButton(Button button)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.ShowButton(button, false, null));
		}

		/**
		 * Sets an animator trigger to change the animation state for an animated sprite.
		 * This is the primary method of controlling Unity animations from a Fungus command sequence.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param animator The sprite to be made non-clickable
		 * @param triggerName Name of a trigger parameter in the animator controller
		 */
		public static void SetAnimatorTrigger(Animator animator, string triggerName)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetAnimatorTrigger(animator, triggerName));
		}

		#endregion
		#region Audio Methods
		
		/**
		 * Plays game music using an audio clip.
		 * One music clip may be played at a time.
		 * @param musicClip The music clip to play
		 */
		public static void PlayMusic(AudioClip musicClip)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				MusicManager.GetInstance().PlayMusic(musicClip);
			}));
		}
		
		/**
		 * Stops playing game music.
		 */
		public static void StopMusic()
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				MusicManager.GetInstance().StopMusic();
			}));
		}
		
		/**
		 * Sets the game music volume immediately.
		 * @param volume The new music volume value [0..1]
		 */
		public static void SetMusicVolume(float volume)
		{
			SetMusicVolume(volume, 0f);
		}
		
		/**
		 * Fades the game music volume to required level over a period of time.
		 * @param volume The new music volume value [0..1]
		 * @param duration The length of time in seconds needed to complete the volume change.
		 */
		public static void SetMusicVolume(float volume, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				MusicManager.GetInstance().SetMusicVolume(volume, duration);
			}));
		}
		
		/**
		 * Plays a sound effect once.
		 * Multiple sound effects can be played at the same time.
		 * @param soundClip The sound effect clip to play
		 */
		public static void PlaySound(AudioClip soundClip)
		{
			PlaySound(soundClip, 1f);
		}
		
		/**
		 * Plays a sound effect once, at the specified volume.
		 * Multiple sound effects can be played at the same time.
		 * @param soundClip The sound effect clip to play
		 * @param volume The volume level of the sound effect
		 */
		public static void PlaySound(AudioClip soundClip, float volume)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Call(delegate {
				MusicManager.GetInstance().PlaySound(soundClip, volume);
			}));
		}

		#endregion
		#region Obsolete Methods

		// These methods are provided for backwards compatibility purposes and will be removed in a future release.

		/**
		 * Sets the screen space rectangle used to display the story text using a Page object.
		 * Page objects can be edited visually in the Unity editor which is useful for accurate placement.
		 * The actual screen space rect used is based on both the Page bounds and the camera transform at the time the command is executed.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param page A Page object which defines the screen rect to use when rendering story text.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPage(Page page, PageController.Layout pageLayout = PageController.Layout.FullSize)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetPage(page, pageLayout));
		}
		
		/**
		 * Sets the screen space rectangle used to display the story text using a ScreenRect object.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param screenRect A ScreenRect object which defines a rect in normalized screen space coordinates.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageRect(PageController.ScreenRect screenRect, PageController.Layout pageLayout = PageController.Layout.FullSize)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetPageRect(screenRect, pageLayout));
		}
		
		/**
		 * Sets the screen space rectangle used to display the story text.
		 * The rectangle coordinates are in normalized screen space. e.g. x1 = 0 (left), y1 = 0 (top) x2 = 1 (right) y2 = 1 (bottom).
		 * The origin is at the top left of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param x1 Page rect left coordinate in normalized screen space coords [0..1]
		 * @param y1 Page rect top coordinate in normalized screen space coords [0..1
		 * @param x2 Page rect right coordinate in normalized screen space coords [0..1]
		 * @param y2 Page rect bottom coordinate in normalized screen space coords [0..1]
		 * @param pageLayout Layout mode for positioning page within the rect.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageRect(float x1, float y1, float x2, float y2, PageController.Layout pageLayout = PageController.Layout.FullSize)
		{
			PageController.ScreenRect screenRect = new PageController.ScreenRect();
			screenRect.x1 = x1;
			screenRect.y1 = y1;
			screenRect.x2 = x2;
			screenRect.y2 = y2;
			SetPageRect(screenRect, pageLayout);
		}
		
		/**
		 * Display story page at the top of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param scaleX Scales the width of the Page [0..1]. 1 = full screen width.
		 * @param scaleY Scales the height of the Page [0..1]. 1 = full screen height.
		 * @param pageLayout Controls how the Page is positioned and sized based on the displayed content.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageTop(float scaleX, float scaleY, PageController.Layout pageLayout)
		{
			PageController.ScreenRect screenRect = PageController.CalcScreenRect(new Vector2(scaleX, scaleY), PageController.PagePosition.Top);
			SetPageRect(screenRect, pageLayout);
		}
		
		/**
		 * Display story page at the top of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageTop()
		{
			Vector2 pageScale = Game.GetInstance().pageController.defaultPageScale;
			SetPageTop(pageScale.x, pageScale.y, PageController.Layout.FullSize);
		}
		
		/**
		 * Display story page at the middle of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param scaleX Scales the width of the Page [0..1]. 1 = full screen width.
		 * @param scaleY Scales the height of the Page [0..1]. 1 = full screen height.
		 * @param pageLayout Controls how the Page is positioned and sized based on the displayed content.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageMiddle(float scaleX, float scaleY, PageController.Layout pageLayout)
		{
			PageController.ScreenRect screenRect = PageController.CalcScreenRect(new Vector2(scaleX, scaleY), PageController.PagePosition.Middle);
			SetPageRect(screenRect, pageLayout);
		}
		
		/**
		 * Display story page at the middle of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageMiddle()
		{
			Vector2 pageScale = Game.GetInstance().pageController.defaultPageScale;
			SetPageMiddle(pageScale.x, pageScale.y, PageController.Layout.FitToMiddle);
		}
		
		/**
		 * Display story page at the bottom of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param scaleX Scales the width of the Page [0..1]. 1 = full screen width.
		 * @param scaleY Scales the height of the Page [0..1]. 1 = full screen height.
		 * @param pageLayout Controls how the Page is positioned and sized based on the displayed content.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageBottom(float scaleX, float scaleY, PageController.Layout pageLayout)
		{
			PageController.ScreenRect screenRect = PageController.CalcScreenRect(new Vector2(scaleX, scaleY), PageController.PagePosition.Bottom);
			SetPageRect(screenRect, pageLayout);
		}
		
		/**
		 * Display story page at the bottom of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageBottom()
		{
			Vector2 pageScale = Game.GetInstance().pageController.defaultPageScale;
			SetPageBottom(pageScale.x, pageScale.y, PageController.Layout.FullSize);
		}
		
		/**
		 * Sets the active style for displaying the Page.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param pageStyle The style object to make active
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetPageStyle(PageStyle pageStyle)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetPageStyle(pageStyle));
		}
		
		/**
		 * Obsolete: Use SetHeader() instead.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void Title(string titleText)
		{
			SetHeader(titleText);
		}
		
		/**
		 * Sets the header text displayed at the top of the page.
		 * The header text is only displayed when there is some story text or options to be shown.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param footerText The text to display as the header of the Page.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetHeader(string headerText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetHeader(headerText));
		}
		
		/**
		 * Sets the footer text displayed at the top of the page.
		 * The footer text is only displayed when there is some story text or options to be shown.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param footerText The text to display as the footer of the Page.
		 */
		[Obsolete("Pages are obsolete. Please use the new Dialog system instead.")]
		public static void SetFooter(string footerText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetFooter(footerText));
		}

		/**
		 * Display all previously added options as buttons, with no text prompt.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		[Obsolete("No longer required. Use Say() instead.")]
		public static void Choose()
		{
			Choose("");
		}
		
		/**
		 * Displays a story text prompt, followed by all previously added options.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param chooseText The story text to be written above the list of options
		 */
		[Obsolete("No longer required. Use Say() instead.")]
		public static void Choose(string chooseText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Choose(chooseText));
		}

		#endregion
	}
}