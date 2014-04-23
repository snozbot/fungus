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
		#region General Methods

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
		 */
		public static void PanToView(View targetView, float duration)
		{
			PanToPosition(targetView.transform.position, targetView.viewSize, duration);
		}

		/**
		 * Pans the camera to the target position and size over a period of time.
		 * The pan starts at the current camera position and performs a smoothed linear pan to the target.
		 * Command execution blocks until the pan completes.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param targetView The View to pan the camera to.
		 * @param duration The length of time in seconds needed to complete the pan.
		 */
		public static void PanToPosition(Vector3 targetPosition, float targetSize, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.PanToPosition(targetPosition, targetSize, duration));
		}
		
		/**
		 * Pans the camera through a sequence of target Views over a period of time.
		 * The pan starts at the current camera position and performs a smooth pan through all Views in the list.
		 * Command execution blocks until the pan completes.
		 * If more control is required over the camera path then you should instead use an Animator component to precisely control the Camera path.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param duration The length of time in seconds needed to complete the pan.
		 * @param targetViews A parameter list of views to visit during the pan.
		 */
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

		#endregion
		#region Page Methods

		/**
		 * Sets the display rect for the active Page using a PageBounds object.
		 * PageBounds objects can be edited visually in the Unity editor which is useful for accurate placement.
		 * The actual screen space rect used is based on both the PageBounds values and the camera transform at the time the command is executed.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param pageBounds The bounds object to use when calculating the Page display rect.
		 */
		public static void SetPageBounds(PageBounds pageBounds, Page.Layout pageLayout = Page.Layout.FullSize)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetPageBounds(pageBounds, pageLayout));
		}

		public static void SetPageRect(Page.ScreenRect screenRect, Page.Layout pageLayout = Page.Layout.FullSize)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetPageRect(screenRect, pageLayout));
		}

		/**
		 * Sets the screen space rectangle used to display the Page.
		 * The rectangle coordinates are in normalized screen space. e.g. x1 = 0 (Far left), x1 = 1 (Far right).
		 * The origin is at the top left of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param x1 Page rect left coordinate in normalized screen space coords [0..1]
		 * @param y1 Page rect top coordinate in normalized screen space coords [0..1
		 * @param x2 Page rect right coordinate in normalized screen space coords [0..1]
		 * @param y2 Page rect bottom coordinate in normalized screen space coords [0..1]
		 * @param pageLayout Layout mode for positioning page within the rect.
		 */
		public static void SetPageRect(float x1, float y1, float x2, float y2, Page.Layout pageLayout = Page.Layout.FullSize)
		{
			Page.ScreenRect screenRect = new Page.ScreenRect();
			screenRect.x1 = x1;
			screenRect.y1 = y1;
			screenRect.x2 = x2;
			screenRect.y2 = y2;
			SetPageRect(screenRect, pageLayout);
		}

		/**
		 * Sets the active Page to display at the top of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param scaleX Scales the width of the Page [0..1]. 1 = full screen width.
		 * @param scaleY Scales the height of the Page [0..1]. 1 = full screen height.
		 * @param pageLayout Controls how the Page is positioned and sized based on the displayed content.
		 */
		public static void SetPageTop(float scaleX, float scaleY, Page.Layout pageLayout)
		{
			Page.ScreenRect screenRect = Page.CalcScreenRect(new Vector2(scaleX, scaleY), Page.PagePosition.Top);
			SetPageRect(screenRect, pageLayout);
		}

		/**
		 * Sets the currently active Page to display at the top of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		public static void SetPageTop()
		{
			Vector2 pageScale = Game.GetInstance().defaultPageScale;
			SetPageTop(pageScale.x, pageScale.y, Page.Layout.FullSize);
		}

		/**
		 * Sets the active Page to display at the middle of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param scaleX Scales the width of the Page [0..1]. 1 = full screen width.
		 * @param scaleY Scales the height of the Page [0..1]. 1 = full screen height.
		 * @param pageLayout Controls how the Page is positioned and sized based on the displayed content.
		 */
		public static void SetPageMiddle(float scaleX, float scaleY, Page.Layout pageLayout)
		{
			Page.ScreenRect screenRect = Page.CalcScreenRect(new Vector2(scaleX, scaleY), Page.PagePosition.Middle);
			SetPageRect(screenRect, pageLayout);
		}

		/**
		 * Sets the currently active Page to display at the middle of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		public static void SetPageMiddle()
		{
			Vector2 pageScale = Game.GetInstance().defaultPageScale;
			SetPageMiddle(pageScale.x, pageScale.y, Page.Layout.FitToMiddle);
		}

		/**
		 * Sets the active Page to display at the bottom of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param scaleX Scales the width of the Page [0..1]. 1 = full screen width.
		 * @param scaleY Scales the height of the Page [0..1]. 1 = full screen height.
		 * @param pageLayout Controls how the Page is positioned and sized based on the displayed content.
		 */
		public static void SetPageBottom(float scaleX, float scaleY, Page.Layout pageLayout)
		{
			Page.ScreenRect screenRect = Page.CalcScreenRect(new Vector2(scaleX, scaleY), Page.PagePosition.Bottom);
			SetPageRect(screenRect, pageLayout);
		}

		/**
		 * Sets the currently active Page to display at the bottom of the screen.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		public static void SetPageBottom()
		{
			Vector2 pageScale = Game.GetInstance().defaultPageScale;
			SetPageBottom(pageScale.x, pageScale.y, Page.Layout.FullSize);
		}

		/**
		 * Sets the active style for displaying the Page.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param pageStyle The style object to make active
		 */
		public static void SetPageStyle(PageStyle pageStyle)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetPageStyle(pageStyle));
		}

		/**
		 * Obsolete! Use Header() instead.
		 */
		[System.Obsolete("use SetHeader() instead")]
		public static void Title(string titleText)
		{
			SetHeader(titleText);
		}

		/**
		 * Sets the header text displayed at the top of the active Page.
		 * The header text is only displayed when there is some story text or options to be shown.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param footerText The text to display as the header of the Page.
		 */
		public static void SetHeader(string headerText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetHeader(headerText));
		}

		/**
		 * Sets the footer text displayed at the top of the active Page.
		 * The footer text is only displayed when there is some story text or options to be shown.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param footerText The text to display as the footer of the Page.
		 */
		public static void SetFooter(string footerText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetFooter(footerText));
		}

		/**
		 * Writes story text to the currently active Page.
		 * A 'continue' button is displayed when the text has fully appeared.
		 * Command execution halts until the user chooses to continue.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param storyText The text to be written to the currently active Page.
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
		 * Display all previously added options as buttons, with no text prompt.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 */
		public static void Choose()
		{
			Choose("");
		}
		
		/**
		 * Displays a story text prompt, followed by all previously added options.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param chooseText The story text to be written above the list of options
		 */
		public static void Choose(string chooseText)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.Choose(chooseText));
		}

		#endregion
		#region State Methods

		/**
		 * Sets a globally accessible integer value.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the value to set
		 * @param value The value to set
		 */
		public static void SetValue(string key, int value)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetValue(key, value));
		}

		/**
		 * Sets a globally accessible integer value to 1.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The name of the value to set
		 */
		public static void SetValue(string key)
		{
			SetValue(key, 1);
		}

		/**
		 * Sets a globally accessible integer value to 0.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param key The key of the value.
		 */
		public static void ClearValue(string key)
		{
			SetValue(key, 0);
		}

		/**
		 * Gets a globally accessible integer value.
		 * Returns zero if the value has not previously been set.
		 * @param key The name of the value
		 * @return The integer value for this key, or 0 if not previously set.
		 */
		public static int GetValue(string key)
		{
			return Game.GetInstance().GetGameValue(key);
		}

		/**
		 * Checks if a value is non-zero.
		 * @param key The name of the value to check.
		 * @return Returns true if the value is not equal to zero.
		 */
		public static bool HasValue(string key)
		{
			return GetValue(key) != 0;
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
		 * Displays a button and sets 
		 * Automatically adds a Button component to the object to respond to player input.
		 * If no Collider2D already exists on the object, then a BoxCollider2D is automatically added.
		 * Use RemoveButton() to make a sprite non-clickable again.
		 * This method returns immediately but it queues an asynchronous command for later execution.
		 * @param spriteRenderer The sprite to be made clickable
		 * @param buttonAction The Action delegate method to be called when the player clicks on the button
		 */
		public static void ShowButton(Button button, Action buttonAction)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.ShowButton(button, true, buttonAction));
		}

		/**
		 * Makes a sprite stop behaving as a clickable button.
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
		 * @param audioClip The music clip to play
		 */
		public static void PlayMusic(AudioClip audioClip)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.PlayMusic(audioClip));
		}
		
		/**
		 * Stops playing game music.
		 */
		public static void StopMusic()
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.StopMusic());
		}
		
		/**
		 * Sets the game music volume immediately.
		 * @param musicVolume The new music volume value [0..1]
		 */
		public static void SetMusicVolume(float musicVolume)
		{
			SetMusicVolume(musicVolume, 0f);
		}
		
		/**
		 * Fades the game music volume to required level over a period of time.
		 * @param musicVolume The new music volume value [0..1]
		 * @param duration The length of time in seconds needed to complete the volume change.
		 */
		public static void SetMusicVolume(float musicVolume, float duration)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.SetMusicVolume(musicVolume, duration));
		}
		
		/**
		 * Plays a sound effect once.
		 * Multiple sound effects can be played at the same time.
		 * @param audioClip The sound effect clip to play
		 */
		public static void PlaySound(AudioClip audioClip)
		{
			PlaySound(audioClip, 1f);
		}
		
		/**
		 * Plays a sound effect once, at the specified volume.
		 * Multiple sound effects can be played at the same time.
		 * @param audioClip The sound effect clip to play
		 * @param volume The volume level of the sound effect
		 */
		public static void PlaySound(AudioClip audioClip, float volume)
		{
			CommandQueue commandQueue = Game.GetInstance().commandQueue;
			commandQueue.AddCommand(new Command.PlaySound(audioClip, volume));
		}

		#endregion
	}
}