// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fungus
{

	/// <summary>
	/// Test utility that simulates user clicking on a button after a delay.
	/// </summary>
	public class FakeButtonClick : MonoBehaviour 
	{
		public float delay;

		void Start () 
		{
			Invoke("DoFakeInput", delay);
		}
		
		void DoFakeInput()
		{
			Button button = GetComponent<Button>();
			button.onClick.Invoke();
		}
	}

}