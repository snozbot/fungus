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