/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class SpritesTest : MonoBehaviour 
	{
		public SpriteRenderer sprite1;
		public SpriteRenderer sprite2;
		public SpriteRenderer sprite3;

		public void DoTest()
		{
			if (sprite1 == null ||
			    sprite2 == null ||
			    sprite3 == null)
			{
				IntegrationTest.Fail("A sprite property has not been set");
			}

			if (sprite1.sortingOrder == 10 &&
				sprite2.sortingOrder == 5 &&
			    sprite3.sortingOrder == 5)
			{
				IntegrationTest.Pass();
			}
			else
			{
				IntegrationTest.Fail("Sprite order in layer not correct");
			}
		}
	}

}