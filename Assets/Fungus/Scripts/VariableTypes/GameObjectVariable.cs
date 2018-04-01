// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
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
        public static readonly CompareOperator[] compareOperators = { CompareOperator.Equals, CompareOperator.NotEquals };
        public static readonly SetOperator[] setOperators = { SetOperator.Assign };

        public virtual bool Evaluate(CompareOperator compareOperator, GameObject gameObjectValue)
        {
            GameObject lhs = Value;
            GameObject rhs = gameObjectValue;

            bool condition = false;

            switch (compareOperator)
            {
                case CompareOperator.Equals:
                    condition = lhs == rhs;
                    break;
                case CompareOperator.NotEquals:
                    condition = lhs != rhs;
                    break;
                default:
                    Debug.LogError("The " + compareOperator.ToString() + " comparison operator is not valid.");
                    break;
            }

            return condition;
        }

        public override void Apply(SetOperator setOperator, GameObject value)
        {
            switch (setOperator)
            {
                case SetOperator.Assign:
                    Value = value;
                    break;
                default:
                    Debug.LogError("The " + setOperator.ToString() + " set operator is not valid.");
                    break;
            }
        }
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
                return gameObjectVal.ToString();
            }
            else
            {
                return gameObjectRef.Key;
            }
        }
    }
}