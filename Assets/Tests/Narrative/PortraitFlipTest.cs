using UnityEngine;
using System.Collections;

public class PortraitFlipTest : MonoBehaviour {
	
	void Update () 
	{
		Transform t = gameObject.transform.FindChild("Canvas/JohnCharacter");	

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
