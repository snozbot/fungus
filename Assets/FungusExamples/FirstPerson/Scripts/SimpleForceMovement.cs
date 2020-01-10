// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Fungus.Examples
{
    /// <summary>
    /// A very simple movement script for a fungus demo. Not intended for use in full projects.
    /// 
    /// Primarily exists to avoid reliance on Unity Standard Assets.
    /// </summary>
    public class SimpleForceMovement : MonoBehaviour
    {
        public Rigidbody rb;
        public Transform getForwardFrom;
        public float forceScale;

        void FixedUpdate()
        {
            var forward = getForwardFrom.forward;
            forward.y = 0;
            forward.Normalize();

            var right = getForwardFrom.right;
            right.y = 0;
            right.Normalize();

            forward *= Input.GetAxis("Vertical");
            right *= Input.GetAxis("Horizontal");

            var movVec = forward + right;
            if (movVec.magnitude > 1)
                movVec = movVec.normalized;

            rb.AddForce(movVec * forceScale);
        }
    }
}