/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;

public class DummyText : MonoBehaviour 
{
	protected string _text;

	public string text 
	{
		get { return _text; }
		set 
		{ 
			_text = value; 
			Debug.Log (_text); 
			IntegrationTest.Pass(); 
		}
	}
}
