using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class FakeWriterInput : MonoBehaviour 
	{
		public float delay;

		void Start () 
		{
			Invoke("DoFakeInput", delay);
		}
		
		void DoFakeInput()
		{
			Writer writer = GetComponent<Writer>();
			writer.OnNextLineEvent();
		}
	}

}