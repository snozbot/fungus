using UnityEngine;
using System.Collections;

namespace Fungus
{

	// Used by the Fungus Script window to serialize the currently active Fungus Script object
	// so that the same Fungus Script can be displayed while editing & playing.
	public class FungusState : MonoBehaviour 
	{
		public FungusScript selectedFungusScript;
	}

}