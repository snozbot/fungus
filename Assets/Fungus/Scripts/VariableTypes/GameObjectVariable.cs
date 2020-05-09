// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// GameObject variable type.
    /// </summary>
    [VariableInfo("Other", "GameObject")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class GameObjectVariable : VariableBase<GameObject>
    {
    }

    /// <summary>
    /// Container for a GameObject variable reference or constant value.
    /// </summary>
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
            get { return (gameObjectRef == null) ? gameObjectVal : gameObjectRef.Value; }
            set { if (gameObjectRef == null) { gameObjectVal = value; } else { gameObjectRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (gameObjectRef == null)
            {
                return gameObjectVal != null ? gameObjectVal.ToString() : "Null";
            }
            else
            {
                return gameObjectRef.Key;
            }
        }
    }
}