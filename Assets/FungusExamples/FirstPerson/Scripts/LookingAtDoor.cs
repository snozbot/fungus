// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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

        public VariableReference fungusBoolHasGazed;

        public void ActivateNow()
        {
            enabled = true;
        }

        private void Update()
        {
            var curCounter = gazeCounter;
            RaycastHit hit;
            if (Physics.Raycast(eye.position, eye.forward, out hit))
            {
                if (hit.collider == doorCol)
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

            if (gazeCounter >= gazeTime && curCounter <= gazeTime)
            {
                runBlockWhenGazed.Execute();
                fungusBoolHasGazed.Set(true);
            }
        }
    }
}