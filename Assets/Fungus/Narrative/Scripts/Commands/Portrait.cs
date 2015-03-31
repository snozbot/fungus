using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public struct PortraitState
	{
		public bool onScreen;
		public bool dimmed;
		public DisplayType display;
		public Sprite portrait;
		public RectTransform position;
		public FacingDirection facing;
		public GameObject portraitObj;
		public Image portraitImage;
	}
	
	public enum DisplayType
	{
		None,
		Show,
		Hide,
		Replace,
		MoveToFront
	}
	public enum FacingDirection
	{
		None,
		Left,
		Right
	}
	
	public enum PositionOffset
	{
		None,
		OffsetLeft,
		OffsetRight
	}
	
	[CommandInfo("Narrative", 
	             "Portrait", 
	             "Controls a character portrait. ")]
	public class Portrait : Command 
	{
		[Tooltip("Stage to display portrait on")]
		public Stage stage;
		
		[Tooltip("Display type")]
		public DisplayType display;
		
		[Tooltip("Character to display")]
		public Character character;
		
		[Tooltip("Character to swap with")]
		public Character replacedCharacter;
		
		[Tooltip("Portrait to display")]
		public Sprite portrait;
		
		[Tooltip("Move the portrait from/to this offset position")]
		public PositionOffset offset;
		
		[Tooltip("Move the portrait from this position")]
		public RectTransform fromPosition;
		protected RectTransform fromPositionition;
		
		[Tooltip("Move the portrait to this positoin")]
		public RectTransform toPosition;
		protected RectTransform toPositionition;
		
		[Tooltip("Direction character is facing")]
		public FacingDirection facing;
		
		[Tooltip("Use Default Settings")]
		public bool useDefaultSettings = true;
		
		[Tooltip("Fade Duration")]
		public float fadeDuration;
		
		[Tooltip("Movement Speed")]
		public float moveSpeed;
		
		[Tooltip("Shift Offset")]
		public Vector2 shiftOffset;
		
		[Tooltip("Move")]
		public bool move;
		
		[Tooltip("Start from offset")]
		public bool shiftIntoPlace;
		
		[Tooltip("Wait until the tween has finished before executing the next command")]
		public bool waitUntilFinished = false;
		
		public override void OnEnter()
		{
			// If no display specified, do nothing
			if (display == DisplayType.None)
			{
				Continue();
				return;
			}
			// If no character specified, do nothing
			if (character == null)
			{
				Continue();
				return;
			}
			// If Replace and no replaced character specified, do nothing
			if (display == DisplayType.Replace && replacedCharacter == null)
			{
				Continue();
				return;
			}
			// Selected "use default Portrait Stage"
			if (stage == null)            // Default portrait stage selected
			{
				if (stage == null)        // If no default specified, try to get any portrait stage in the scene
				{
					stage = GameObject.FindObjectOfType<Stage>();
				}
			}
			// If portrait stage does not exist, do nothing
			if (stage == null)
			{
				Continue();
				return;
			}
			
			if (character.state.portraitImage == null)
			{
				CreatePortraitObject(character,stage);
			}
			// if no previous portrait, use default portrait
			if (character.state.portrait == null) 
			{
				character.state.portrait = character.profileSprite;
			}
			// Selected "use previous portrait"
			if (portrait == null) 
			{
				portrait = character.state.portrait;
			}
			// if no previous position, use default position
			if (character.state.position == null)
			{
				character.state.position = stage.defaultPosition.rectTransform;
			}
			// Selected "use previous position"
			if (toPosition == null)
			{
				toPosition = character.state.position;
			}
			if (replacedCharacter != null)
			{
				// if no previous position, use default position
				if (replacedCharacter.state.position == null)
				{
					replacedCharacter.state.position = stage.defaultPosition.rectTransform;
				}
			}
			// If swapping, use replaced character's position
			if (display == DisplayType.Replace)
			{
				toPosition = replacedCharacter.state.position;
			}
			// Selected "use previous position"
			if (fromPosition == null)
			{
				fromPosition = character.state.position;
			}
			// if portrait not moving, use from position is same as to position
			if (!move)
			{
				fromPosition = toPosition;
			}
			if (display == DisplayType.Hide)
			{
				fromPosition = character.state.position;
			}
			// if no previous facing direction, use default facing direction
			if (character.state.facing == FacingDirection.None) 
			{
				character.state.facing = character.portraitsFace;
			}
			// Selected "use previous facing direction"
			if (facing == FacingDirection.None)
			{
				facing = character.state.facing;
			}
			// Use default settings
			if (useDefaultSettings)
			{
				fadeDuration = stage.fadeDuration;
				moveSpeed = stage.moveSpeed;
				shiftOffset = stage.shiftOffset;
			}
			switch(display)
			{
			case (DisplayType.Show):
				Show(character,fromPosition,toPosition);
				character.state.onScreen = true;
				if (!stage.charactersOnStage.Contains(character))
				{
					stage.charactersOnStage.Add(character);
				}
				break;
			case (DisplayType.Hide):
				Hide(character,fromPosition,toPosition);
				character.state.onScreen = false;
				stage.charactersOnStage.Remove(character);
				break;
			case (DisplayType.Replace):
				Show(character,fromPosition,toPosition);
				Hide(replacedCharacter, replacedCharacter.state.position, replacedCharacter.state.position);
				character.state.onScreen = true;
				replacedCharacter.state.onScreen = false;
				stage.charactersOnStage.Add(character);
				stage.charactersOnStage.Remove(replacedCharacter);
				break;
			case (DisplayType.MoveToFront):
				MoveToFront(character);
				break;
			}
			
			if (display == DisplayType.Replace)
			{
				character.state.display = DisplayType.Show;
				replacedCharacter.state.display = DisplayType.Hide;
			}
			else
			{
				character.state.display = display;
			}
			character.state.portrait = portrait;
			character.state.facing = facing;
			character.state.position = toPosition;
			if (!waitUntilFinished)
			{
				Continue();
			}
		}
		public static void CreatePortraitObject(Character character, Stage stage)
		{
			GameObject portraitObj = new GameObject(character.name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			portraitObj.transform.SetParent(stage.portraitCanvas.transform, true);
			Image portraitImage = portraitObj.GetComponent<Image>();
			portraitImage.preserveAspect = true;
			portraitImage.sprite = character.profileSprite;
			// Workaround for bug #92. Tiled switches off an internal quad cropping optimisation.
			portraitImage.type = Image.Type.Tiled;
			Material portraitMaterial = Instantiate(Resources.Load("Portrait")) as Material;
			portraitImage.material = portraitMaterial;
			character.state.portraitObj = portraitObj;
			character.state.portraitImage = portraitImage;
			character.state.portraitImage.material.SetFloat("_Alpha",0);
		}
		protected void SetupPortrait(Character character, RectTransform fromPosition)
		{
			SetRectTransform(character.state.portraitImage.rectTransform, fromPosition);
			character.state.portraitImage.material.SetFloat("_Fade",0);
			character.state.portraitImage.material.SetTexture("_MainTex", character.profileSprite.texture);
			Texture2D blankTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			blankTexture.SetPixel(0, 0, new Color(0f,0f,0f,0f));
			blankTexture.Apply();
			character.state.portraitImage.material.SetTexture("_TexStart", blankTexture as Texture);
			character.state.portraitImage.material.SetTexture("_TexEnd", blankTexture as Texture);
			if (character.state.facing != character.portraitsFace)
			{
				character.state.portraitImage.material.SetFloat("_FlipStart",1);
			}
			else
			{
				character.state.portraitImage.material.SetFloat("_FlipStart",0);
			}
			if (facing != character.portraitsFace)
			{
				character.state.portraitImage.material.SetFloat("_FlipEnd",1);
			}
			else
			{
				character.state.portraitImage.material.SetFloat("_FlipEnd",0);
			}
			character.state.portraitImage.material.SetFloat("_Alpha",1);
		}
		public static void SetRectTransform(RectTransform oldRectTransform, RectTransform newRectTransform)
		{
			oldRectTransform.eulerAngles      = newRectTransform.eulerAngles;
			oldRectTransform.position         = newRectTransform.position;
			oldRectTransform.rotation         = newRectTransform.rotation;
			oldRectTransform.anchoredPosition = newRectTransform.anchoredPosition;
			oldRectTransform.sizeDelta        = newRectTransform.sizeDelta;
			oldRectTransform.anchorMax        = newRectTransform.anchorMax;
			oldRectTransform.anchorMin        = newRectTransform.anchorMin;
			oldRectTransform.pivot            = newRectTransform.pivot;
			oldRectTransform.localScale       = newRectTransform.localScale;
		}
		protected void Show(Character character, RectTransform fromPosition, RectTransform toPosition) 
		{
			if (shiftIntoPlace)
			{
				fromPosition = Instantiate(toPosition) as RectTransform;
				if (offset == PositionOffset.OffsetLeft)
				{
					fromPosition.anchoredPosition = new Vector2(fromPosition.anchoredPosition.x-Mathf.Abs(shiftOffset.x), fromPosition.anchoredPosition.y-Mathf.Abs(shiftOffset.y));
				}
				else if (offset == PositionOffset.OffsetRight)
				{
					fromPosition.anchoredPosition = new Vector2(fromPosition.anchoredPosition.x+Mathf.Abs(shiftOffset.x), fromPosition.anchoredPosition.y+Mathf.Abs(shiftOffset.y));
				}
				else
				{
					fromPosition.anchoredPosition = new Vector2(fromPosition.anchoredPosition.x, fromPosition.anchoredPosition.y);
				}
			}
			SetupPortrait(character, fromPosition);
			if (character.state.display != DisplayType.None && character.state.display != DisplayType.Hide)
			{
				character.state.portraitImage.material.SetTexture("_TexStart", character.state.portrait.texture);
			}
			character.state.portraitImage.material.SetTexture("_TexEnd", portrait.texture);
			UpdateTweens(character, fromPosition, toPosition);
		}
		protected void Hide(Character character, RectTransform fromPosition, RectTransform toPosition)
		{
			if (character.state.display == DisplayType.None)
			{
				return;
			}
			SetupPortrait(character, fromPosition);
			character.state.portraitImage.material.SetTexture("_TexStart", character.state.portrait.texture);
			UpdateTweens(character, fromPosition, toPosition);
		}
		protected void MoveToFront(Character character)
		{
			character.state.portraitImage.transform.SetSiblingIndex(character.state.portraitImage.transform.parent.childCount);
		}
		protected void UpdateTweens(Character character, RectTransform fromPosition, RectTransform toPosition) 
		{
			if (fadeDuration == 0) fadeDuration = float.Epsilon;
			LeanTween.value(character.state.portraitObj,0,1,fadeDuration).setEase(stage.fadeEaseType).setOnComplete(OnComplete).setOnUpdate(
				(float fadeAmount)=>{
				character.state.portraitImage.material.SetFloat("_Fade", fadeAmount);
			}
			);
			float moveDuration = (Vector3.Distance(fromPosition.anchoredPosition,toPosition.anchoredPosition)/moveSpeed);
			if (moveSpeed == 0) moveDuration = float.Epsilon;
			LeanTween.value(character.state.portraitObj,fromPosition.anchoredPosition,toPosition.anchoredPosition,moveDuration).setEase(stage.moveEaseType).setOnComplete(OnComplete).setOnUpdate(
				(Vector3 updatePosition)=>{
				character.state.portraitImage.rectTransform.anchoredPosition = updatePosition;
			}
			);
		}
		public static void Dim(Character character, Stage stage)
		{
			if (character.state.dimmed == false)
			{
				character.state.dimmed = true;
				float fadeDuration = stage.fadeDuration;
				if (fadeDuration == 0) fadeDuration = float.Epsilon;
				LeanTween.value(character.state.portraitObj,1f,0.5f,fadeDuration).setEase(stage.fadeEaseType).setOnUpdate(
					(float tintAmount)=>{
					Color tint = new Color(tintAmount,tintAmount,tintAmount,1);
					character.state.portraitImage.material.SetColor("_Color", tint);
				}
				);
			}
		}
		public static void Undim(Character character, Stage stage)
		{
			if (character.state.dimmed == true)
			{
				character.state.dimmed = false;
				float fadeDuration = stage.fadeDuration;
				if (fadeDuration == 0) fadeDuration = float.Epsilon;
				LeanTween.value(character.state.portraitObj,0.5f,1f,fadeDuration).setEase(stage.fadeEaseType).setOnUpdate(
					(float tintAmount)=>{
					Color tint = new Color(tintAmount,tintAmount,tintAmount,1);
					character.state.portraitImage.material.SetColor("_Color", tint);
				}
				);
			}
		}
		protected void OnComplete() 
		{
			if (waitUntilFinished)
			{
				if (!LeanTween.isTweening (character.state.portraitObj))
				{
					Continue();
				}
			}
		}
		public override string GetSummary()
		{
			if (display == DisplayType.None && character == null)
			{
				return "Error: No character or display selected";
			}
			else if (display == DisplayType.None)
			{
				return "Error: No display selected";
			}
			else if (character == null)
			{
				return "Error: No character selected";
			}
			string displaySummary = "";
			string characterSummary = "";
			string fromPositionSummary = "";
			string toPositionSummary = "";
			string stageSummary = "";
			string portraitSummary = "";
			string facingSummary = "";
			
			displaySummary = StringFormatter.SplitCamelCase(display.ToString());

			if (display == DisplayType.Replace)
			{
				if (replacedCharacter != null)
				{
					displaySummary += " \"" + replacedCharacter.name + "\" with";
				}
			}
			characterSummary = character.name;
			if (stage != null)
			{
				stageSummary = " on \"" + stage.name + "\"";
			}
			
			if (portrait != null)
			{
				portraitSummary = " " + portrait.name;
			}
			if (shiftIntoPlace)
			{
				if (offset != 0)
				{
					fromPositionSummary = offset.ToString();
					fromPositionSummary = " from " + "\"" + fromPositionSummary + "\"";
				}
			}
			else if (fromPosition != null)
			{
				fromPositionSummary = " from " + "\"" + fromPosition.name + "\"";
			}
			if (toPosition != null)
			{
				string toPositionPrefixSummary = "";
				if (move)
					toPositionPrefixSummary = " to ";
				else
					toPositionPrefixSummary = " at ";
				toPositionSummary = toPositionPrefixSummary + "\"" + toPosition.name + "\"";
			}
			if (facing != FacingDirection.None)
			{
				if ( facing == FacingDirection.Left )
				{
					facingSummary = "<--";
				}
				if ( facing == FacingDirection.Right )
				{
					facingSummary = "-->";
				}
				facingSummary = " facing \"" + facingSummary + "\"";
			}
			return displaySummary + " \"" + characterSummary + portraitSummary + "\"" + stageSummary + facingSummary + fromPositionSummary + toPositionSummary;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(230, 200, 250, 255);
		}
		
		public override void OnCommandAdded(Block parentBlock)
		{
			//Default to display type: show
			display = DisplayType.Show;
		}
	}
}