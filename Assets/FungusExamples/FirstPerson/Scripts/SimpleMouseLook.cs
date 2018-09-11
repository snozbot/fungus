using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fungus.Examples
{
    /// <summary>
    /// A very simple mouse look behaviour for a Fungus demo. Not intended for use in full projects.
    /// 
    /// Primarily exists to avoid reliance on Unity Standard Assets.
    /// </summary>
    public class SimpleMouseLook : MonoBehaviour
    {

        public float xsen = 1, ysen = 1;
        public float maxPitch = 60;
        public Transform target;

        private float pitch = 0;

        // Update is called once per frame
        void Update()
        {
            var curEuler = target.localRotation.eulerAngles;
            curEuler = new Vector3(pitch - Input.GetAxis("Mouse Y"), curEuler.y + Input.GetAxis("Mouse X"), 0);
            curEuler.z = 0;
            curEuler.x = Mathf.Clamp(curEuler.x, -maxPitch, maxPitch);
            pitch = curEuler.x;
            target.localRotation = Quaternion.Euler(curEuler);
        }
    }
}