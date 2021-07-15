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
    Rotate,
    None
}
public enum CameraUtilState
{
    Enable,
    Disable,
    DisableAll,
    None
}
public enum CameraUtilRotate
{
    RotateX,
    RotateY,
    RotateZ,
    Rotate,
    SetToDefaultRotation,
    None
}
namespace Fungus
{
    /// <summary>
    /// Functionalitites for orthographic camera for platformer or adverture type of games
    /// </summary>
    [CommandInfo("Camera",
                 "Orthographic Camera Utility",
                 "Orthographic camera utility for actions such as; scroll/pinch to zoom, camera follow, and drag camera.\n\nNOTE: CameraFollow and Drag can't be assign to the same camera at once!")]
    [AddComponentMenu("")]
    public class OrthographicCameraUtility : Command
    {
        [Tooltip("Camera behaviour")]
        [SerializeField] public CameraUtilSelect action;

        [Tooltip("Camera behaviour")]
        [SerializeField] public CameraUtilRotate rotate;
        [Tooltip("Single axis value")]
        [SerializeField] protected FloatData rotateValue = new FloatData(0f);

        [Tooltip("Terget rotation")]
        [SerializeField] protected Vector3Data rotateVector3 = new Vector3Data(Vector3.zero);

        [Tooltip("Duration")]
        [SerializeField] protected FloatData duration = new FloatData(2f);

        [Tooltip("Active state")]
        [SerializeField] public CameraUtilState activeState;

        [Tooltip("Minimum zoom out limit of camera")]
        [SerializeField] protected FloatData minValue = new FloatData(3f);

        [Tooltip("Maximum zoom in limit of camera")]
        [SerializeField] protected FloatData maxValue = new FloatData(10f);

        [Tooltip("Speed")]
        [SerializeField] protected FloatData speed = new FloatData(10f);

        [Tooltip("Set smooth/damp level of camera movements")]
        [SerializeField] protected FloatData smoothness = new FloatData(1f);
        [Tooltip("Enable dampening effect")]
        [SerializeField] protected BooleanData smoothDamp = new BooleanData(true);

        [Tooltip("Velocity in Vector3")]
        [SerializeField] protected Vector3 velocityVec3 = Vector3.zero;

        [Tooltip("Velocity")]
        [SerializeField] protected float velocity = 0.1f;

        [Tooltip("Camera to use. Will use main camera if set to none")]
        [SerializeField] protected Camera targetCamera;

        [Tooltip("Reset to default position via right mouse click")]
        [SerializeField] protected BooleanData rightMouseReset;        

        [Tooltip("Target object to follow")]
        [SerializeField] protected TransformData targetObject;
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

                vals = new List<(Action, string, string)>();
            }
        }
        public void ExecOrthoCamera(string act, string objName, bool state)
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

                if(vals.Count > 0)
                {
                    //1 camera can only have 1 instance of drag, follow and scroll at once
                    //And will be ignored if same camera assigned
                    var matchingvalues = vals.Where(x => x.Item3.Contains(objName) && x.Item2.Contains(act)).ToArray();

                    if (matchingvalues.Length == 0)
                    {
                        vals.Add((validateOrtho, act, objName));
                        Debug.Log(vals.Count);
                    }
                }
                else
                {
                    vals.Add((validateOrtho, act, objName));
                }
            }
            else
            {
                if (CameraUtilityHelper.OrthoActionLists.Count > 0)
                {
                    var vals = CameraUtilityHelper.OrthoActionLists;

                    for (int i = 0; i < vals.Count; i++)
                    {
                        if (vals[i].Item2.Equals(act) && vals[i].Item3.Equals(objName))
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
                cameraPosition = targetObject.Value.position + initialOffset;
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
                    Diference = MousePos() - (targetCamera.transform.position);
                    if(!smoothDamp)
                    targetCamera.transform.position = Origin - Diference;
                    else
                    targetCamera.transform.position = Vector3.SmoothDamp(targetCamera.transform.position, Origin - Diference, ref velocityVec3, smoothness * Time.deltaTime);
                }

                if (rightMouseReset)
                {
                    if (Input.GetMouseButton(1)) // Resets camera to original position
                    {
                        if(smoothDamp)
                        targetCamera.transform.position = Vector3.SmoothDamp(targetCamera.transform.position, ResetCamera, ref velocityVec3, smoothness * Time.deltaTime);
                        else
                        targetCamera.transform.position = ResetCamera;
                    }
                }
            }
        }
        //Returns the position of the mouse in world coordinates
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
        public void SetDefaultOrthoRotation()
        {
            var ltD = CameraUtilityHelper.OrthoRotateInstance;
            if(ltD != null)
            {
                LeanTween.cancel(targetCamera.gameObject, true);
            }
            LeanTween.rotate(targetCamera.gameObject, CameraUtilityHelper.defCamPos, duration).setEaseInOutQuad().setOnComplete(()=>
            {
                ltD = null;
            });
        }
        protected void RotateOrthoCamera()
        {
            CameraUtilityHelper.defCamPos = targetCamera.transform.rotation.eulerAngles;
            var ltD = CameraUtilityHelper.OrthoRotateInstance;

            if(ltD == null)
            {
                switch (rotate)
                {
                    case CameraUtilRotate.RotateX:
                        ltD = LeanTween.rotateX(targetCamera.gameObject, rotateValue, duration).updateNow().setEaseInOutQuad().setOnComplete(()=>{ltD = null;});
                    break;
                    case CameraUtilRotate.RotateY:
                        ltD = LeanTween.rotateY(targetCamera.gameObject, rotateValue, duration).updateNow().setEaseInOutQuad().setOnComplete(()=>{ltD = null;});
                    break;
                    case CameraUtilRotate.RotateZ:
                        ltD = LeanTween.rotateZ(targetCamera.gameObject, rotateValue, duration).updateNow().setEaseInOutQuad().setOnComplete(()=>{ltD = null;});
                    break;
                    case CameraUtilRotate.Rotate:
                        ltD = LeanTween.rotate(targetCamera.gameObject, rotateVector3, duration).updateNow().setEaseInOutQuad().setOnComplete(()=>{ltD = null;});
                    break;
                    case CameraUtilRotate.SetToDefaultRotation:
                        SetDefaultOrthoRotation();
                    break;
                }
            }
        }
        #region Public members
        public override void OnEnter()
        {
            if (activeState == CameraUtilState.DisableAll)
            {
                DisableAllOrthoUtility();
                Continue();
                return;
            }
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
                    var tName = targetCamera.name;
                    switch (action)
                    {
                        case CameraUtilSelect.ScrollPinchToZoom:
                            if (activeState == CameraUtilState.Enable)
                            {
                                tmpVal = targetCamera.orthographicSize;
                                ExecOrthoCamera("ScrollPinchZoom", tName, isScrollToZoom = true);
                            }
                            else if (activeState == CameraUtilState.Disable)
                            {
                                ExecOrthoCamera("ScrollPinchZoom", tName, false);
                            }
                            break;
                        case CameraUtilSelect.CameraFollow:
                            if (activeState == CameraUtilState.Enable)
                            {
                                if (targetObject.Value != null)
                                {
                                    initialOffset = targetCamera.transform.position - targetObject.Value.position;
                                    ExecOrthoCamera("CameraFollow", tName, isCameraFollow = true);
                                }
                            }
                            else if (activeState == CameraUtilState.Disable)
                            {
                                ExecOrthoCamera("CameraFollow", tName, false);
                            }
                            break;
                        case CameraUtilSelect.DragCamera:
                            if (activeState == CameraUtilState.Enable)
                            {
                                ExecOrthoCamera("DragCamera", tName, isDragged = true);
                            }
                            else if (activeState == CameraUtilState.Disable)
                            {
                                ExecOrthoCamera("DragCamera", tName, false);
                            }
                            break;
                        case CameraUtilSelect.Rotate:
                            if(rotate != CameraUtilRotate.None && rotate != CameraUtilRotate.SetToDefaultRotation)
                            {
                                RotateOrthoCamera();
                            }
                            if(rotate == CameraUtilRotate.SetToDefaultRotation)
                            {
                                SetDefaultOrthoRotation();
                            }
                            break;
                    }
                }
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
            if (targetObject.Value == null && action == CameraUtilSelect.CameraFollow)
            {
                return tObj = "Error: No target object selected";
            }

            return  tCam + " : " + tObj + " : " + tOrt;
        }
        public override bool HasReference(Variable variable)
        {
            return targetObject.transformRef == variable || minValue.floatRef == variable || maxValue.floatRef == variable ||
            speed.floatRef == variable || smoothness.floatRef == variable || duration.floatRef == variable || rotateValue.floatRef == variable ||
            smoothDamp.booleanRef == variable || base.HasReference(variable);
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