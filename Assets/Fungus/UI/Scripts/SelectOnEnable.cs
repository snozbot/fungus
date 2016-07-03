using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
	[RequireComponent(typeof(Selectable))]
	public class SelectOnEnable : MonoBehaviour
	{
		private Selectable selectable;

		private void Awake()
		{
			selectable = GetComponent<Selectable>();
		}

		private void OnEnable()
		{
			selectable.Select();
		}
	}
}
