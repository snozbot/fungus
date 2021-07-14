// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;

public enum CameraUtilSelect
{
    ScrollPinchToZoom,
    DragCamera,
    CameraFollow,
    None
}
public enum CameraUtilState
{
    Enable,
    Disable
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
    public class CameraUtility : Command
    {
        [Tooltip("Camera behaviour")]
        [SerializeField] protected CameraUtilSelect action;
        [Tooltip("Active State")]
        [SerializeField] protected CameraUtilState activeState;

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
        protected Vector3 initalOffset;
        protected Vector3 cameraPosition;
        [Tooltip("Reset to default position via right mouse click")]
        [SerializeField] protected bool rightMouseReset = false;

        [Tooltip("Set smooth level of camera movements")]
        [SerializeField] protected float smoothness;
        [Tooltip("Target object for camera to follow")]
        [SerializeField] protected Transform targetObject;
        public static bool isScrollToZoom{get;set;}
        public static bool isDragged{get;set;}
        public static bool isCameraFollow{get;set;}
        public void ExecOrthoCamera(string act, bool state)
        {
            if(state)
            {
                var validateOrtho = new Action(() =>
                {
                    if(act.Contains("Scroll"))
                    isScrollToZoom = !state;
                    else if(act.Contains("Drag"))
                    isDragged = !state;
                    else if(act.Contains("Follow"))
                    isCameraFollow = !state;
                });

                CameraUtilityHelper.OrthoActionLists.Add((validateOrtho, act));
            }
            else
            {
                if(CameraUtilityHelper.OrthoActionLists.Count > 0)
                {
                    var vals = CameraUtilityHelper.OrthoActionLists;
                    for(int i = 0; i < vals.Count; i++)
                    {
                        if(vals[i].Item2.Equals(act))
                        {
                            vals[i].Item1.Invoke();
                        }
                    }
                }
            }
        }
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
            if (targetCamera == null || !targetCamera.orthographic)
            {            
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
                                if(activeState == CameraUtilState.Enable)
                                {
                                    tmpVal = targetCamera.orthographicSize;
                                    ExecOrthoCamera("ScrollPinchZoom", isScrollToZoom = true);                                                                   
                                }
                                else
                                {
                                    ExecOrthoCamera("ScrollPinchZoom", false);
                                }
                                break;
                            case CameraUtilSelect.CameraFollow:
                                if(activeState == CameraUtilState.Enable)
                                {
                                    if(targetObject != null)
                                    {
                                        initalOffset = targetCamera.transform.position - targetObject.position;
                                        ExecOrthoCamera("CameraFollow", isCameraFollow = true);
                                    }
                                }
                                else
                                {
                                    ExecOrthoCamera("CameraFollow", false);
                                }
                                break;
                            case CameraUtilSelect.DragCamera:
                                if(activeState == CameraUtilState.Enable)
                                {
                                    ExecOrthoCamera("DragCamera", isDragged = true);
                                }
                                else
                                {
                                    ExecOrthoCamera("DragCamera", false);
                                }
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