// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Moves the camera to a location specified by a View object.
    /// </summary>
    [CommandInfo("Camera", 
                 "Move To View", 
                 "Moves the camera to a location specified by a View object.")]
    [AddComponentMenu("")]
    public class MoveToView : Command 
    {
        [Tooltip("Time for move effect to complete")]
        [SerializeField] protected float duration = 1;

        [Tooltip("View to transition to when move is complete")]
        [SerializeField] protected View targetView;
        public virtual View TargetView { get { return targetView; } }

        [Tooltip("Wait until the fade has finished before executing next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        [Tooltip("Camera to use for the pan. Will use main camera if set to none.")]
        [SerializeField] protected Camera targetCamera;

        [SerializeField] protected LeanTweenType orthoSizeTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] protected LeanTweenType posTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] protected LeanTweenType rotTweenType = LeanTweenType.easeInOutQuad;

        protected virtual void AcquireCamera()
        {
            if (targetCamera != null)
            {
                return;
            }

            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = GameObject.FindObjectOfType<Camera>();
            }
        }

        public virtual void Start()
        {
            AcquireCamera();
        }

        #region Public members

        public override void OnEnter()
        {
            AcquireCamera();
            if (targetCamera == null ||
                targetView == null)
            {
                Continue();
                return;
            }

            var cameraManager = FungusManager.Instance.CameraManager;

            Vector3 targetPosition = targetView.transform.position;
            Quaternion targetRotation = targetView.transform.rotation;
            float targetSize = targetView.ViewSize;

            cameraManager.PanToPosition(targetCamera, targetPosition, targetRotation, targetSize, duration, delegate {
                if (waitUntilFinished)
                {
                    Continue();
                }
            }, orthoSizeTweenType, posTweenType, rotTweenType);

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnStopExecuting()
        {
            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.Stop();
        }

        public override string GetSummary()
        {
            if (targetView == null)
            {
                return "Error: No view selected";
            }
            else
            {
                return targetView.name;
            }
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}