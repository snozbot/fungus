// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Fungus
{
    /// <summary>
    /// Storage for a collection of fungus variables that can then be accessed globally.
    /// </summary>
    public class GlobalVariables : MonoBehaviour
    {
        protected Flowchart holder;

        public Flowchart GlobalVariableFlowchart { get { return holder; } }

        Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

        void Awake()
        {
            holder = new GameObject("GlobalVariables").AddComponent<Flowchart>();
            holder.transform.parent = transform;
        }

        private void OnEnable()
        {
            //we don't touch load or save as the GlobalVarSaveData deals with those
            SaveManagerSignals.OnSaveReset += SaveManagerSignals_OnSaveReset;
        }

        private void OnDisable()
        {
            SaveManagerSignals.OnSaveReset -= SaveManagerSignals_OnSaveReset;
        }

        private void SaveManagerSignals_OnSaveReset()
        {
            //remove all the vars
            ClearVars();
        }

        public virtual void ClearVars()
        {
            foreach (var item in holder.Variables)
            {
                Destroy(item);
            }

            holder.Variables.Clear();
        }

        public Variable GetVariable(string variableKey)
		{
			Variable v = null;
			variables.TryGetValue(variableKey, out v);
			return v;
		}

        public VariableBase<T> GetOrAddVariable<T>(string variableKey, T defaultvalue, Type type)
        {
            Variable v = null;
            VariableBase<T> vAsT = null;
            var res = variables.TryGetValue(variableKey, out v);

            if(res && v != null)
            {
                vAsT = v as VariableBase<T>;

                if (vAsT != null)
                {
                    return vAsT;
                }
                else
                {
                    Debug.LogError("A fungus variable of name " + variableKey + " already exists, but of a different type");
                }
            }
            else
            {
                //create the variable
                vAsT = AddVariable(variableKey, type) as VariableBase<T>;
                vAsT.Value = defaultvalue;
            }

            return vAsT;
        }

        public Variable AddVariable(string key, Type type)
        {
            var v = holder.gameObject.AddComponent(type) as Variable;
            v.Key = key;
            v.Scope = VariableScope.Public;
            variables[key] = v;
            holder.Variables.Add(v);
            return v;
        }
    }
}