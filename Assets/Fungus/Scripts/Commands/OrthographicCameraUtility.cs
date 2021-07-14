// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

public enum CameraUtilSelect
{
    ScrollPinchToZoom,
    DragCamera,
    CameraFollow,
    None
}

namespace Fungus
{
    /// <summary>
    /// Functionalitites for orthographic camere for platformer or adverture type of games
    /// </summary>
    [CommandInfo("Camera",
                 "Orthographic Camera Utility",
                 "Pinch or mouse scroll to zoom in/out and drag a camera")]
    [AddComponentMenu("")]
    public class OrthographicCameraUtility : Command
    {
        [Tooltip("Camera behaviour")]
        [SerializeField] protected CameraUtilSelect action;

        [Tooltip("Minimum zoom out limit of camera")]
        [SerializeField] protected float minValue = 3;

        [Tooltip("Maximum zoom in limit of camera")]
        [SerializeField] protected float maxValue = 12;

        [Tooltip("Velocity")]
        [SerializeField] protected float velocity = 0.1f;        

        [Tooltip("Camera to use. Will use main camera if set to none.")]
        [SerializeField] protected Camera targetCamera;
        protected float tmpVal = 0f;
        protected Vector3 ResetCamera; // original camera position
        protected Vector3 Origin; // place where mouse is first pressed
        protected Vector3 Diference; // change in position of mouse relative to origin
        protected Vector3 touchStart;
        protected bool isScrollToZoom = false;
        protected bool isDragged = false;
        protected bool isCameraFollow = false;
        protected Vector3 initalOffset;
        protected Vector3 cameraPosition;
        [Tooltip("Reset to default position via right mouse click")]
        [SerializeField] protected bool rightMouseReset = false;

        [Tooltip("Set smooth level of camera movements")]
        [SerializeField] protected float smoothness;
        [Tooltip("Target object for camera to follow")]
        [SerializeField] protected Transform targetObject;
        public virtual bool SetPinch { get { return isScrollToZoom; } set { isScrollToZoom = value; } }
        public virtual bool SetCameraFollow { get { return isCameraFollow; } set { isCameraFollow = value; } }
        public virtual bool SetMouseDrag { get { return isDragged; } set { isDragged = value; } }
        void FixedUpdate()
        {
            if (isCameraFollow)
            {
                cameraPosition = targetObject.position + initalOffset;
                targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, cameraPosition, smoothness * Time.fixedDeltaTime);
            }
        }

        void LateUpdate()
        {
            if (isScrollToZoom)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    touchStart = targetCamera.ScreenToWorldPoint(Input.mousePosition);
                }

                if (Input.touchCount == 2)
                {
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).sqrMagnitude;
                    float currentMagnitude = (touchZero.position - touchOne.position).sqrMagnitude;
                    float difference = currentMagnitude - prevMagnitude;

                    Zoom(difference * 0.01f);
                }
                Zoom(Input.GetAxis("Mouse ScrollWheel"));
            }

            if (isDragged)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Origin = MousePos();
                }

                if (Input.GetMouseButton(0))
                {
                    Diference = MousePos() - targetCamera.transform.position;
                    targetCamera.transform.position = Origin - Diference;
                }

                if (rightMouseReset)
                {
                    if (Input.GetMouseButton(1)) // reset camera to original position
                    {
                        targetCamera.transform.position = ResetCamera;
                    }
                }
            }
        }
        //Return the position of the mouse in world coordinates
        protected Vector3 MousePos()
        {
            return targetCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        protected void Zoom(float increment)
        {
            //Min/Max Range
            if(tmpVal >= minValue && tmpVal <=maxValue)
            {
                tmpVal += increment;
            }

            //On high mouse scroll rate the above would sometimes fail, thus second check is needed
            tmpVal = tmpVal >= maxValue? maxValue:tmpVal;
            tmpVal = tmpVal <= minValue? minValue:tmpVal;

            targetCamera.orthographicSize = Mathf.SmoothDamp(targetCamera.orthographicSize, tmpVal, ref velocity, smoothness);
        }

        #region Public members
        public override void OnEnter()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
            if (targetCamera == null)
            {            
                Debug.LogWarning("Camera not found");
                Continue();
                return;
            }
            if(!targetCamera.orthographic)
            {
                Debug.LogWarning("Camera projection type is not orthographic");
                Continue();
                return;
            }
            if(targetCamera != null && action != CameraUtilSelect.None)
            {
                if(targetCamera.orthographic)
                {
                    switch (action)
                    {
                        case CameraUtilSelect.ScrollPinchToZoom:
                            isScrollToZoom = true;
                            tmpVal = targetCamera.orthographicSize;
                            break;
                        case CameraUtilSelect.LockCameraToObject:
                            if(targetObject != null)
                            {
                                initalOffset = targetCamera.transform.position - targetObject.position;
                                isCameraFollow = true;
                            }
                            break;
                        case CameraUtilSelect.DragCamera:
                            isDragged = true;
                            break;
                    }
                }
            }
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}