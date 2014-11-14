using UnityEngine;
using System.Collections;

namespace Fungus
{
	public enum TargetObjectType
	{
		Owner,
		Other
	}

	[System.Serializable]
	public class TargetObject 
	{	
		public TargetObjectType targetType;
		public GameObject otherGameObject;

		public virtual string GetSummary()
		{
			if (targetType == TargetObjectType.Owner)
			{
				return "Owner";
			}
			else if (otherGameObject != null)
			{
				return otherGameObject.name;
			}

			return null;
		}
	}

}