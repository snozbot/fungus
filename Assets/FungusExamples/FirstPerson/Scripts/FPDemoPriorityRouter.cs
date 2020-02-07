// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fungus.Examples
{
    public class FPDemoPriorityRouter : MonoBehaviour
    {
        public Behaviour[] componentEnabledOutsideFungusPriority;
        public Behaviour[] componentEnabledInsideFungusPriority;

        void OnEnable()
        {
            Fungus.FungusPrioritySignals.OnFungusPriorityStart += FungusPrioritySignals_OnFungusPriorityStart;
            Fungus.FungusPrioritySignals.OnFungusPriorityEnd += FungusPrioritySignals_OnFungusPriorityEnd;
        }

        void OnDisable()
        {
            Fungus.FungusPrioritySignals.OnFungusPriorityStart -= FungusPrioritySignals_OnFungusPriorityStart;
            Fungus.FungusPrioritySignals.OnFungusPriorityEnd -= FungusPrioritySignals_OnFungusPriorityEnd;
        }

        private void FungusPrioritySignals_OnFungusPriorityEnd()
        {
            foreach (var item in componentEnabledOutsideFungusPriority)
            {
                item.enabled = true;
            }
            foreach (var item in componentEnabledInsideFungusPriority)
            {
                item.enabled = false;
            }
        }

        private void FungusPrioritySignals_OnFungusPriorityStart()
        {
            foreach (var item in componentEnabledOutsideFungusPriority)
            {
                item.enabled = false;
            }
            foreach (var item in componentEnabledInsideFungusPriority)
            {
                item.enabled = true;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}