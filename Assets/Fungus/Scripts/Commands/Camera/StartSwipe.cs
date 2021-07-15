// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Activates swipe panning mode where the player can pan the camera within the area between viewA & viewB.
    /// </summary>
    [CommandInfo("Camera", 
                 "Start Swipe", 
                 "Activates swipe panning mode where the player can pan the camera within the area between viewA & viewB.")]
    [AddComponentMenu("")]
    public class StartSwipe : Command 
    {
        [Tooltip("Defines one extreme of the scrollable area that the player can pan around")]
        [SerializeField] protected View viewA;

        [Tooltip("Defines one extreme of the scrollable area that the player can pan around")]
        [SerializeField] protected View viewB;

        [Tooltip("Time to move the camera to a valid starting position between the two views")]
        [SerializeField] protected float duration = 0.5f;

        [Tooltip("Multiplier factor for speed of swipe pan")]
        [SerializeField] protected float speedMultiplier = 1f;

        [Tooltip("Camera to use for the pan. Will use main camera if set to none.")]
        [SerializeField] protected Camera targetCamera;

        #region Public members

        public virtual void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
            if (targetCamera == null)
            {
                targetCamera = GameObject.FindObjectOfType<Camera>();
            }
        }

        public override void OnEnter()
        {
            if (targetCamera == null ||
                viewA == null ||
                viewB == null)
            {
                Continue();
                return;
            }

            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.StartSwipePan(targetCamera, viewA, viewB, duration, speedMultiplier, () => Continue() );
        }

        public override string GetSummary()
        {
            if (viewA == null)
            {
                return "Error: No view selected for View A";
            }

            if (viewB == null)
            {
                return "Error: No view selected for View B";
            }

            return viewA.name + " to " + viewB.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}