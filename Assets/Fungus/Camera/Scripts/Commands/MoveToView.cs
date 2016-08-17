/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Camera", 
                 "Move To View", 
                 "Moves the camera to a location specified by a View object.")]
    [AddComponentMenu("")]
    public class MoveToView : Command 
    {
        [Tooltip("Time for move effect to complete")]
        public float duration = 1;

        [Tooltip("View to transition to when move is complete")]
        public Fungus.View targetView;

        [Tooltip("Wait until the fade has finished before executing next command")]
        public bool waitUntilFinished = true;

        [Tooltip("Camera to use for the pan. Will use main camera if set to none.")]
        public Camera targetCamera;
        
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

        public override void OnEnter()
        {
            AcquireCamera();
            if (targetCamera == null ||
                targetView == null)
            {
                Continue();
                return;
            }

            CameraController cameraController = CameraController.GetInstance();

            if (waitUntilFinished)
            {
                cameraController.waiting = true;
            }

            Vector3 targetPosition = targetView.transform.position;
            Quaternion targetRotation = targetView.transform.rotation;
            float targetSize = targetView.viewSize;

            cameraController.PanToPosition(targetCamera, targetPosition, targetRotation, targetSize, duration, delegate {
                if (waitUntilFinished)
                {
                    cameraController.waiting = false;
                    Continue();
                }
            });

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnStopExecuting()
        {
            CameraController.GetInstance().StopAllCoroutines();
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
    }

}