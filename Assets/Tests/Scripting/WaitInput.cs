// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;
using Fungus;

[CommandInfo("Flow",
			 "WaitInput", 
			 "Waits for a period of time or for player input before executing the next command in the block.")]
[AddComponentMenu("")]
public class WaitInput : Command
{
	[Tooltip("Duration to wait for. If negative will wait until player input occurs.")]
	public float duration = 1;

	public override void OnEnter()
	{
		StartCoroutine( CheckInput() );
	}

	IEnumerator CheckInput()
	{
		float timer = 0f;
		while (duration < 0f || timer < duration)
		{
			timer += Time.deltaTime;

			if( Input.anyKeyDown || Input.GetMouseButtonDown(0) )
			{
				break;
			}

			yield return null;
		}

		Continue();
	}

	public override string GetSummary()
	{
		if( duration <= float.Epsilon )
		{
			return "Any input";
		}

		return "Any input, or " + duration.ToString() + " seconds";
	}

	public override Color GetButtonColor()
	{
		return new Color32(235, 191, 217, 255);
	}
}

