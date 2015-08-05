using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "GameObject")]
	[AddComponentMenu("")]
	public class GameObjectVariable : VariableBase<GameObject>
	{}

	[System.Serializable]
	public struct GameObjectData
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(GameObjectVariable))]
		public GameObjectVariable gameObjectRef;
		
		[SerializeField]
		public GameObject gameObjectVal;

		public GameObjectData(GameObject v)
		{
			gameObjectVal = v;
			gameObjectRef = null;
		}
		
		public static implicit operator GameObject(GameObjectData gameObjectData)
		{
			return gameObjectData.Value;
		}

		public GameObject Value
		{
			get { return (gameObjectRef == null) ? gameObjectVal : gameObjectRef.value; }
			set { if (gameObjectRef == null) { gameObjectVal = value; } else { gameObjectRef.value = value; } }
		}

		public string GetDescription()
		{
			if (gameObjectRef == null)
			{
				return gameObjectVal.ToString();
			}
			else
			{
				return gameObjectRef.key;
			}
		}
	}

}