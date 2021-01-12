// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Linq;
using UnityEngine;

namespace Fungus
{
    public static class GameObjectUtils
    {
        /// <summary>
        /// Often we require that each of a certain type in a scene have a unique name this helper ensures that, but
        /// requires parts of UnityEditor. Fortunately, this logic is desired withing Validation or creation in scenes.
        /// </summary>
        public static void UniqueGameObjectNamePerType<T>(T targetComp) where T : MonoBehaviour
        {
#if UNITY_EDITOR
            var allOfType = GameObject.FindObjectsOfType<T>();
            var others = allOfType.Where(x => x != targetComp).ToList();
            var res = others.FirstOrDefault(x => x.gameObject.name == targetComp.gameObject.name);
            if (res != null)
            {
                Debug.LogWarning(typeof(T).Name + " need unique names. Multiple of name " + targetComp.gameObject.name + " found.");

                var allOtherNames = others.Select(x => x.gameObject.name).ToArray();
                targetComp.gameObject.name = UnityEditor.ObjectNames.GetUniqueName(allOtherNames, targetComp.gameObject.name);
            }
#endif
        }

        public static T FindObjectOfTypeWithGameObjectName<T>(string name) where T : MonoBehaviour
        {
            return GameObject.FindObjectsOfType<T>().FirstOrDefault(x => x.gameObject.name == name);
        }
    }
}
