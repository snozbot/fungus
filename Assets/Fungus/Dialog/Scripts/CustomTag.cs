using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[ExecuteInEditMode]
	public class CustomTag : MonoBehaviour 
	{
		public string tagStartSymbol;
		public string tagEndSymbol;
		public string replaceTagStartWith;
		public string replaceTagEndWith;
		
		static public List<CustomTag> activeCustomTags = new List<CustomTag>();
		
		protected virtual void OnEnable()
		{
			if (!activeCustomTags.Contains(this))
			{
				activeCustomTags.Add(this);
			}
		}
		
		protected virtual void OnDisable()
		{
			activeCustomTags.Remove(this);
		}
	}
	
}