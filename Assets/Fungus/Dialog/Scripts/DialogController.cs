using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogController : MonoBehaviour 
{
	public Sprite testCharacter;

	public Canvas dialogCanvas;
	public List<Button> optionButtons = new List<Button>();
	public Text nameText;
	public Text storyText;
	public Image continueImage;
	public Image leftImage;
	public Image rightImage;

	public enum ImageSide
	{
		Left,
		Right
	};
	
	public void SetCharacterImage(Sprite image, ImageSide side)
	{
		if (leftImage != null)
		{
			if (image != null &&
			    side == ImageSide.Left)
			{
				leftImage.sprite = image;
				leftImage.enabled = true;
			}
			else
			{
				leftImage.enabled = false;
			}
		}

		if (rightImage != null)
		{
			rightImage.sprite = null;
			if (image != null &&
			    side == ImageSide.Right)
			{
				rightImage.sprite = image;
				rightImage.enabled = true;
			}
			else
			{
				rightImage.sprite = null;
				rightImage.enabled = false;
			}
		}
	}

	public void SetCharacterName(string name, Color color)
	{
		if (nameText != null)
		{
			nameText.text = name;
			nameText.color = color;
		}
	}

	public void SetStoryText(string text)
	{
		if (storyText != null)
		{
			storyText.text = text;
		}
	}

	public void ShowContinueIcon(bool visible)
	{
		if (continueImage != null)
		{
			continueImage.enabled = visible;
		}
	}

	public void ClearOptions()
	{
		if (optionButtons == null)
		{
			return;
		}

		foreach (Button button in optionButtons)
		{
			button.gameObject.SetActive(false);
		}
	}

	public void AddOption(string text, Action action)
	{
		if (optionButtons == null)
		{
			return;
		}
		
		foreach (Button button in optionButtons)
		{
			if (!button.gameObject.activeSelf)
			{
				button.gameObject.SetActive(true);

				Text textComponent = button.GetComponentInChildren<Text>();
				if (textComponent != null)
				{
					textComponent.text = text;
				}

				// TODO: Connect action

				break;
			}
		}
	}

	public void Start()
	{
		SetCharacterImage(testCharacter, ImageSide.Left);
		SetCharacterName("Podrick", Color.red);
		SetStoryText("Simple story text");
		ShowContinueIcon(false);

		ClearOptions();
		AddOption("Something 1", Callback );
		AddOption("Something 2", Callback );
	}

	void Callback()
	{
		Debug.Log ("Callback");
	}

	//public UnityEvent testEvent;

	// Write story text over time
	// Show character image (with side, fade in?)
	// Hide / Show canvas
	// Show continue image
	// Show one button
	// Show button grid

}
