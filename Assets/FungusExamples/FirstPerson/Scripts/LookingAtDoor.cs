using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.Examples
{
    public class LookingAtDoor : MonoBehaviour
    {
        public Collider doorCol;
        public float gazeTime = 0.2f;
        private float gazeCounter = 0;
        public BlockReference runBlockWhenGazed;
        public Transform eye;

        public void ActivateNow()
        {
            enabled = true;
        }

        private void Update()
        {
            RaycastHit hit;
            if(Physics.Raycast(eye.position, eye.forward, out hit))
            {
                if(hit.collider == doorCol)
                {
                    gazeCounter += Time.deltaTime;
                }
                else
                {
                    gazeCounter = 0;
                }
            }
            else
            {
                gazeCounter = 0;
            }

            if(gazeCounter >= gazeTime)
            {
                runBlockWhenGazed.Execute();
                enabled = false;
            }
        }
    }
}