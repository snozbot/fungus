// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
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
    Disable,
    DisableAll,
    None
}

namespace Fungus
{
    /// <summary>
    /// Functionalitites for orthographic camera for platformer or adverture type of games
    /// </summary>
    [CommandInfo("Camera",
                 "Orthographic Camera Utility",
                 "Orthographic camera utility for actions such as; scroll/pinch to zoom, camera follow, and drag camera")]
    [AddComponentMenu("")]
    public class OrthographicCameraUtility : Command
    {
        [Tooltip("Camera behaviour")]
        [SerializeField] public CameraUtilSelect action;

        [Tooltip("Active State")]
        [SerializeField] public CameraUtilState activeState;

        [Tooltip("Minimum zoom out limit of camera")]
        [SerializeField] protected float minValue = 3;

        [Tooltip("Maximum zoom in limit of camera")]
        [SerializeField] protected float maxValue = 12;

        [Tooltip("Speed")]
        [SerializeField] protected float speed = 10f;

        [Tooltip("Set smooth/damp level of camera movements")]
        [SerializeField] protected float smoothness = 1f;

        [Tooltip("Velocity")]
        [SerializeField] protected float velocity = 0.1f;

        [Tooltip("Camera to use. Will use main camera if set to none.")]
        [SerializeField] protected Camera targetCamera;

        [Tooltip("Reset to default position via right mouse click")]
        [SerializeField] protected bool rightMouseReset = false;        

        [Tooltip("Target object for camera to follow")]
        [SerializeField] protected Transform targetObject;
        protected float tmpVal = 0f;
        protected Vector3 ResetCamera; // original camera position
        protected Vector3 Origin; // place where mouse is first pressed
        protected Vector3 Diference; // change in position of mouse relative to origin
        protected Vector3 touchStart;
        protected Vector3 initialOffset;
        protected Vector3 cameraPosition;
        public static bool isScrollToZoom { get; set; }
        public static bool isDragged { get; set; }
        public static bool isCameraFollow { get; set; }
        public void DisableAllOrthoUtility()
        {
            if (CameraUtilityHelper.OrthoActionLists.Count > 0)
            {
                var vals = CameraUtilityHelper.OrthoActionLists;

                for (int i = 0; i < vals.Count; i++)
                {
                    vals[i].Item1.Invoke();
                }

                vals = new List<(Action, string)>();
            }
        }
        public void ExecOrthoCamera(string act, bool state)
        {
            if (state)
            {
                var validateOrtho = new Action(() =>
                {
                    if (act.Contains("Scroll"))
                        isScrollToZoom = !state;
                    else if (act.Contains("Drag"))
                        isDragged = !state;
                    else if (act.Contains("Follow"))
                        isCameraFollow = !state;
                });

                var vals = CameraUtilityHelper.OrthoActionLists;
                    
                var matchingvalues = vals.Where(x => x.Item2.Contains(act)).ToList();
                if (matchingvalues.Count == 0)
                {
                    vals.Add((validateOrtho, act));
                }
            }
            else
            {
                if (CameraUtilityHelper.OrthoActionLists.Count > 0)
                {
                    var vals = CameraUtilityHelper.OrthoActionLists;

                    for (int i = 0; i < vals.Count; i++)
                    {
                        if (vals[i].Item2.Equals(act))
                        {
                            vals[i].Item1.Invoke();
                            vals.Remove(vals[i]);
                            vals.TrimExcess();
                        }
                    }
                }
            }
        }
        void FixedUpdate()
        {
            if (isCameraFollow)
            {
                cameraPosition = targetObject.position + initialOffset;
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

                    Zoom(difference * 0.01f * speed);
                }
                Zoom(Input.GetAxis("Mouse ScrollWheel") * speed);
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
            if (tmpVal >= minValue && tmpVal <= maxValue)
            {
                tmpVal += increment;
            }
            //Mouse with high scroll rate sometimes would fail, thus second check is needed
            tmpVal = tmpVal >= maxValue ? maxValue : tmpVal;
            tmpVal = tmpVal <= minValue ? minValue : tmpVal;

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
            if (targetCamera != null && action != CameraUtilSelect.None)
            {
                if (targetCamera.orthographic)
                {
                    switch (action)
                    {
                        case CameraUtilSelect.ScrollPinchToZoom:
                            if (activeState == CameraUtilState.Enable)
                            {
                                tmpVal = targetCamera.orthographicSize;
                                ExecOrthoCamera("ScrollPinchZoom", isScrollToZoom = true);
                            }
                            else if (activeState == CameraUtilState.Disable)
                            {
                                ExecOrthoCamera("ScrollPinchZoom", false);
                            }
                            break;
                        case CameraUtilSelect.CameraFollow:
                            if (activeState == CameraUtilState.Enable)
                            {
                                if (targetObject != null)
                                {
                                    initialOffset = targetCamera.transform.position - targetObject.position;
                                    ExecOrthoCamera("CameraFollow", isCameraFollow = true);
                                }
                            }
                            else if (activeState == CameraUtilState.Disable)
                            {
                                ExecOrthoCamera("CameraFollow", false);
                            }
                            break;
                        case CameraUtilSelect.DragCamera:
                            if (activeState == CameraUtilState.Enable)
                            {
                                ExecOrthoCamera("DragCamera", isDragged = true);
                            }
                            else if (activeState == CameraUtilState.Disable)
                            {
                                ExecOrthoCamera("DragCamera", false);
                            }
                            break;
                    }
                }
            }

            if(activeState == CameraUtilState.DisableAll)
            {
                DisableAllOrthoUtility();
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            string tCam = string.Empty;
            string tObj = string.Empty;
            string tOrt = string.Empty;

            if (targetCamera == null)
            {
                return tCam = "Error: No camera selected";
            }
            if (targetCamera != null && targetCamera.orthographic == false)
            {
                return tOrt = "Error: Camera's projection type is not orthographic";
            }
            if (targetObject == null && action == CameraUtilSelect.CameraFollow)
            {
                return tObj = "Error: No target object selected";
            }

            return  tCam + " : " + tObj + " : " + tOrt;
        }
        
        public override void OnCommandAdded(Block parentBlock)
        {
            //Default to display type: None
            action = CameraUtilSelect.None;
            activeState = CameraUtilState.None;
        }
        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}