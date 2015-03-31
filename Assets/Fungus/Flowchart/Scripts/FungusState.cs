using UnityEngine;
using System.Collections;

namespace Fungus
{

	// Used by the Flowchart window to serialize the currently active Flowchart object
	// so that the same Flowchart can be displayed while editing & playing.
	[AddComponentMenu("")]
	public class FungusState : MonoBehaviour 
	{
		public Flowchart selectedFlowchart;
	}

}