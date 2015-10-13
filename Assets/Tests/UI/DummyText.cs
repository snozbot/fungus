using UnityEngine;
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
