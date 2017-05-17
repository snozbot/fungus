// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;

public class PortraitFlipTest : MonoBehaviour {
	
	void Update () 
	{
		Transform t = gameObject.transform.Find("Canvas/JohnCharacter");	

		if (t == null)
		{
			return;
		}

		if (t.transform.localScale.x != -1f)
		{
			IntegrationTest.Fail("Character object not flipped horizontally");
		}
	}
}
