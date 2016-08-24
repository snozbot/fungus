using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Select the UI element when the gameobject is enabled.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class SelectOnEnable : MonoBehaviour
    {
        protected Selectable selectable;

        protected void Awake()
        {
            selectable = GetComponent<Selectable>();
        }

        protected void OnEnable()
        {
            selectable.Select();
        }
    }
}
