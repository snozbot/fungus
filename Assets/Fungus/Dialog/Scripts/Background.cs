using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Background : MonoBehaviour 
{
	public Canvas backgroundCanvas;
	public Image backgroundImage;

	public virtual void SetBackgroundImage(Sprite imageSprite)
	{
		if (backgroundCanvas != null)
		{
			backgroundCanvas.gameObject.SetActive(imageSprite != null);
		}

		if (backgroundImage != null)
		{
			backgroundImage.sprite = imageSprite;
		}
	}
}
