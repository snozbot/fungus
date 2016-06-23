/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Fungus;
using UnityTest;

[CommandInfo("Tests",
             "TestNarrative",
             "Test command for narrative integration tests")]
public class NarrativeTests : Command 
{
	public enum TestType
	{
		Show,
		Hide,
		Replace,
		MoveToFront,
		Moving,
		Dimming,
		ResetNoDelay
	}
	
	public TestType testType;

	protected Stage stage;

	static string currentTestName = "";

	static int passCount = 0;

	static void Pass()
	{
		passCount++;
		if (passCount == 12)
		{
			IntegrationTest.Pass();
		}
	}

	static void Fail(string message)
	{
		IntegrationTest.Fail(currentTestName + " : " + message);
	}

	public override void OnEnter()
	{
		GameObject stageGO = GameObject.Find("Stage");
		if (stageGO != null)
		{
			stage = stageGO.GetComponent<Stage>();
		}

		if (stage == null)
		{
			IntegrationTest.Fail("No stage object found");
			Continue();
			return;
		}

		StartCoroutine(DoTest());
	}

	public virtual IEnumerator DoTest()
	{
		// Small delay before performing test to allow tweens to complete
		yield return new WaitForSeconds(1f);

		currentTestName = "Test" + testType.ToString();
		
		switch (testType)
		{
		case TestType.Show:
			TestShow();
			break;
		case TestType.Hide:
			TestHide();
			break;
		case TestType.Replace:
			TestReplace();
			break;
		case TestType.MoveToFront:
			TestMoveToFront();
			break;
		case TestType.Moving:
			TestMoving();
			break;
		case TestType.Dimming:
			TestDimming();
			break;
		case TestType.ResetNoDelay:
			TestResetNoDelay();
			break;
		}
		
		Continue();
	}

	// Test showing multiple characters
	protected virtual void TestShow()
	{
		bool found = (stage.charactersOnStage.Count == 2);

		GameObject johnGO = stage.transform.Find("Canvas/JohnCharacter").gameObject;
		GameObject sherlockGO = stage.transform.Find("Canvas/SherlockCharacter").gameObject;

		found &= (johnGO != null) && (sherlockGO != null);

		if (found)
		{
			Pass();
		}
		else
		{
			Fail("Characters not found on stage" + stage.charactersOnStage.Count);
		}
	}

	// Test hiding a character
	protected virtual void TestHide()
	{
		GameObject johnGO = stage.transform.Find("Canvas/JohnCharacter").gameObject;

		Image johnImage = johnGO.GetComponent<Image>();
		if (johnImage.color.a == 0)
		{
			Pass();
		}
		else
		{
			Fail("Character alpha is not zero " + johnImage.color.a);
		}
	}

	protected virtual void TestFacing()
	{
		GameObject johnGO = stage.transform.Find("Canvas/JohnCharacter").gameObject;
		GameObject sherlockGO = stage.transform.Find("Canvas/SherlockCharacter").gameObject;

		Character johnCharacter = johnGO.GetComponent<Character>();
		Character sherlockCharacter = sherlockGO.GetComponent<Character>();
		if (johnCharacter.portraitsFace == FacingDirection.Right &&
		    sherlockCharacter.portraitsFace == FacingDirection.Left)
		{
			Pass();
		}
		else
		{
			Fail("Characters facing wrong direction");
		}
	}

	protected virtual void TestReplace()
	{
		GameObject johnGO = stage.transform.Find("Canvas/JohnCharacter").gameObject;		
		Image johnImage = johnGO.GetComponent<Image>();

		if (johnImage.color.a == 1f &&
		    johnImage.sprite.name == "bored")
		{
			Pass();
		}
		else
		{
			Fail("Character image not correct");
		}
	}

	protected virtual void TestMoveToFront()
	{
		Transform johnTransform = stage.transform.Find("Canvas/JohnCharacter");		
		
		if (johnTransform.GetSiblingIndex() == johnTransform.parent.childCount - 1)
		{
			Pass();
		}
		else
		{
			Fail("Image position in hierarchy not correct");
		}
	}

	protected virtual void TestMoving()
	{
		Transform johnTransform = stage.transform.Find("Canvas/JohnCharacter");		

		if (Mathf.Approximately(johnTransform.localPosition.x, 0f))
		{
			Pass();
		}
		else
		{
			Debug.Log (johnTransform.localPosition.x);

			Fail("Image position after move not correct");
		}
	}

	protected virtual void TestDimming()
	{
		GameObject sherlockGO = stage.transform.Find("Canvas/SherlockCharacter").gameObject;

		Image sherlockImage = sherlockGO.GetComponent<Image>();

		if (sherlockImage.color.r != 0.5f ||
		    sherlockImage.color.g != 0.5f ||
		    sherlockImage.color.b != 0.5f ||
		    sherlockImage.color.a != 1f)
		{
			Fail("Character image not dimmed");
		}
		else
		{
			Pass();
		}
	}

	protected virtual void TestResetNoDelay()
	{
		// Set the stage durations to 0 so we can rerun the tests in this case
		stage.fadeDuration = 0f;
		stage.moveDuration = 0f;
	}

	public override string GetSummary()
	{
		return "Test type: " + testType.ToString();
	}
}
