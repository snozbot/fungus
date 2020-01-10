// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Serializable container for a string variable.
    /// </summary>
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

    /// <summary>
    /// Serializable container for an integer variable.
    /// </summary>
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

    /// <summary>
    /// Serializable container for a float variable.
    /// </summary>
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

    /// <summary>
    /// Serializable container for a boolean variable.
    /// </summary>
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

    /// <summary>
    /// Serializable container for encoding the state of a Flowchart's variables.
    /// </summary>
    [System.Serializable]
    public class FlowchartData
    {
        [SerializeField] protected string flowchartName;
        [SerializeField] protected List<StringVar> stringVars = new List<StringVar>();
        [SerializeField] protected List<IntVar> intVars = new List<IntVar>();
        [SerializeField] protected List<FloatVar> floatVars = new List<FloatVar>();
        [SerializeField] protected List<BoolVar> boolVars = new List<BoolVar>();

        #region Public methods

        /// <summary>
        /// Gets or sets the name of the encoded Flowchart.
        /// </summary>
        public string FlowchartName { get { return flowchartName; } set { flowchartName = value; } }

        /// <summary>
        /// Gets or sets the list of encoded string variables.
        /// </summary>
        public List<StringVar> StringVars { get { return stringVars; } set { stringVars = value; } }

        /// <summary>
        /// Gets or sets the list of encoded integer variables.
        /// </summary>
        public List<IntVar> IntVars { get { return intVars; } set { intVars = value; } }

        /// <summary>
        /// Gets or sets the list of encoded float variables.
        /// </summary>
        public List<FloatVar> FloatVars { get { return floatVars; } set { floatVars = value; } }

        /// <summary>
        /// Gets or sets the list of encoded boolean variables.
        /// </summary>
        public List<BoolVar> BoolVars { get { return boolVars; } set { boolVars = value; } }

        /// <summary>
        /// Encodes the data in a Flowchart into a structure that can be stored by the save system.
        /// </summary>
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

        /// <summary>
        /// Decodes a FlowchartData object and uses it to restore the state of a Flowchart in the scene.
        /// </summary>
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