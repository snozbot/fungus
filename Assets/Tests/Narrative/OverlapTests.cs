/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;
using Fungus;

public class OverlapTests : MonoBehaviour 
{
	public SayDialog sayDialog_LeftImage;
	public SayDialog sayDialog_RightImage;

	public void Step1()
	{
		if (!sayDialog_RightImage.CharacterImage.IsActive())
		{
			IntegrationTest.Fail("Character image not active");
		}

		if (sayDialog_RightImage.CharacterImage.transform.position.x < sayDialog_RightImage.StoryText.transform.position.x)
		{
			IntegrationTest.Fail("Character image not on right hand side");
		}
	}

	public void Step2()
	{		
		if (sayDialog_RightImage.CharacterImage.IsActive())
		{
			IntegrationTest.Fail("Character image should not be active");
		}

		float width = sayDialog_RightImage.StoryText.rectTransform.rect.width;
		if (!Mathf.Approximately(width, 1439))
		{
			IntegrationTest.Fail("Story text width not correct");
		}
	}

	public void Step3()
	{		
		if (!sayDialog_LeftImage.CharacterImage.IsActive())
		{
			IntegrationTest.Fail("Character image not active");
		}

		if (sayDialog_LeftImage.CharacterImage.transform.position.x > sayDialog_LeftImage.StoryText.transform.position.x)
		{
			IntegrationTest.Fail("Character image not on left hand side");
		}
	}

	public void Step4()
	{		
		if (sayDialog_LeftImage.CharacterImage.IsActive())
		{
			IntegrationTest.Fail("Character image should not be active");
		}

		float width = sayDialog_LeftImage.StoryText.rectTransform.rect.width;
		if (!Mathf.Approximately(width, 1439))
		{
			IntegrationTest.Fail("Story text width not correct");
		}

		IntegrationTest.Pass();
	}
}
