// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls DontDestroyOnLoad on the target gameobject.
    /// </summary>
    [CommandInfo("Scripting",
                 "DestroyOnLoad",
                 "Calls DontDestroyOnLoad on the target gameobject")]
    [AddComponentMenu("")]
    public class DestroyOnLoad : Command
    {
        [SerializeField] protected GameObjectData target;

        public override void OnEnter()
        {
            DontDestroyOnLoad(target.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return target.Value != null ? target.Value.name : "Error: no target set";
        }

        public override bool HasReference(Variable variable)
        {
            return variable == target.gameObjectRef;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}