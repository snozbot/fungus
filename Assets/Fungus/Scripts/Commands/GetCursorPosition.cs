using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Gets the current cursor position and return it to a Vector3Variable
    /// </summary>
    [CommandInfo("Scripting",
                  "Get Cursor Position",
                  "Sets the cursors position as a vector 3 variable")]

    [AddComponentMenu("")]
    public class GetCursorPosition : Command
    {

        [Tooltip("Input text or a string variable")]
        [SerializeField]
        protected Vector3Data cursorPosition;

        protected Vector3 planePoint = new Vector3(0, 0, 0);
        protected Vector3 planeNormal = new Vector3(0, 0, 1);

        public override void OnEnter()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float dist;
            Plane p = new Plane(planeNormal, planePoint);
            p.Raycast(ray, out dist);

            cursorPosition.Value = ray.GetPoint(dist);

            Continue();
        }
    }
}