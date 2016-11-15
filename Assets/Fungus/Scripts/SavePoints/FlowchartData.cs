using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    [System.Serializable]
    public class StringVar
    {
        [SerializeField] protected string key;
        [SerializeField] protected string value;

        #region Public methods

        public string Key { get { return key; } set { key = value; } }
        public string Value { get { return value; } set { this.value = value; } }

        #endregion
    }

    [System.Serializable]
    public class IntVar
    {
        [SerializeField] protected string key;
        [SerializeField] protected int value;

        #region Public methods

        public string Key { get { return key; } set { key = value; } }
        public int Value { get { return value; } set { this.value = value; } }

        #endregion
    }

    [System.Serializable]
    public class FloatVar
    {
        [SerializeField] protected string key;
        [SerializeField] protected float value;

        #region Public methods

        public string Key { get { return key; } set { key = value; } }
        public float Value { get { return value; } set { this.value = value; } }

        #endregion
    }

    [System.Serializable]
    public class BoolVar
    {
        [SerializeField] protected string key;
        [SerializeField] protected bool value;

        #region Public methods

        public string Key { get { return key; } set { key = value; } }
        public bool Value { get { return value; } set { this.value = value; } }

        #endregion
    }

    [System.Serializable]
    public class FlowchartData
    {
        [SerializeField] protected string flowchartName;
        [SerializeField] protected List<StringVar> stringVars = new List<StringVar>();
        [SerializeField] protected List<IntVar> intVars = new List<IntVar>();
        [SerializeField] protected List<FloatVar> floatVars = new List<FloatVar>();
        [SerializeField] protected List<BoolVar> boolVars = new List<BoolVar>();

        #region Public methods

        public string FlowchartName { get { return flowchartName; } set { flowchartName = value; } }
        public List<StringVar> StringVars { get { return stringVars; } set { stringVars = value; } }
        public List<IntVar> IntVars { get { return intVars; } set { intVars = value; } }
        public List<FloatVar> FloatVars { get { return floatVars; } set { floatVars = value; } }
        public List<BoolVar> BoolVars { get { return boolVars; } set { boolVars = value; } }

        public static FlowchartData Encode(Flowchart flowchart)
        {
            var flowchartData = new FlowchartData();

            flowchartData.FlowchartName = flowchart.name;

            for (int i = 0; i < flowchart.Variables.Count; i++) 
            {
                var v = flowchart.Variables[i];

                // Save string
                var stringVariable = v as StringVariable;
                if (stringVariable != null)
                {
                    var d = new StringVar();
                    d.Key = stringVariable.Key;
                    d.Value = stringVariable.Value;
                    flowchartData.StringVars.Add(d);
                }

                // Save int
                var intVariable = v as IntegerVariable;
                if (intVariable != null)
                {
                    var d = new IntVar();
                    d.Key = intVariable.Key;
                    d.Value = intVariable.Value;
                    flowchartData.IntVars.Add(d);
                }

                // Save float
                var floatVariable = v as FloatVariable;
                if (floatVariable != null)
                {
                    var d = new FloatVar();
                    d.Key = floatVariable.Key;
                    d.Value = floatVariable.Value;
                    flowchartData.FloatVars.Add(d);
                }

                // Save bool
                var boolVariable = v as BooleanVariable;
                if (boolVariable != null)
                {
                    var d = new BoolVar();
                    d.Key = boolVariable.Key;
                    d.Value = boolVariable.Value;
                    flowchartData.BoolVars.Add(d);
                }
            }

            return flowchartData;
        }

        public static void Decode(FlowchartData flowchartData)
        {
            var go = GameObject.Find(flowchartData.FlowchartName);
            if (go == null)
            {
                Debug.LogError("Failed to find flowchart object specified in save data");
                return;
            }

            var flowchart = go.GetComponent<Flowchart>();
            if (flowchart == null)
            {
                Debug.LogError("Failed to find flowchart object specified in save data");
                return;
            }

            for (int i = 0; i < flowchartData.BoolVars.Count; i++)
            {
                var boolVar = flowchartData.BoolVars[i];
                flowchart.SetBooleanVariable(boolVar.Key, boolVar.Value);
            }
            for (int i = 0; i < flowchartData.IntVars.Count; i++)
            {
                var intVar = flowchartData.IntVars[i];
                flowchart.SetIntegerVariable(intVar.Key, intVar.Value);
            }
            for (int i = 0; i < flowchartData.FloatVars.Count; i++)
            {
                var floatVar = flowchartData.FloatVars[i];
                flowchart.SetFloatVariable(floatVar.Key, floatVar.Value);
            }
            for (int i = 0; i < flowchartData.StringVars.Count; i++)
            {
                var stringVar = flowchartData.StringVars[i];
                flowchart.SetStringVariable(stringVar.Key, stringVar.Value);
            }
        }

        #endregion
    }
}