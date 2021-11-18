// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Standard comparison operators.
    /// </summary>
    public enum CompareOperator
    {
        /// <summary> == mathematical operator.</summary>
        Equals,
        /// <summary> != mathematical operator.</summary>
        NotEquals,
        /// <summary> < mathematical operator.</summary>
        LessThan,
        /// <summary> > mathematical operator.</summary>
        GreaterThan,
        /// <summary> <= mathematical operator.</summary>
        LessThanOrEquals,
        /// <summary> >= mathematical operator.</summary>
        GreaterThanOrEquals
    }

    /// <summary>
    /// Mathematical operations that can be performed on variables.
    /// </summary>
    public enum SetOperator
    {
        /// <summary> = operator. </summary>
        Assign,
        /// <summary> =! operator. </summary>
        Negate,
        /// <summary> += operator. </summary>
        Add,
        /// <summary> -= operator. </summary>
        Subtract,
        /// <summary> *= operator. </summary>
        Multiply,
        /// <summary> /= operator. </summary>
        Divide
    }

    /// <summary>
    /// Scope types for Variables.
    /// </summary>
    public enum VariableScope
    {
        /// <summary> Can only be accessed by commands in the same Flowchart. </summary>
        Private,
        /// <summary> Can be accessed from any command in any Flowchart. </summary>
        Public,
        /// <summary> Creates and/or references a global variable of that name, all variables of this name and scope share the same underlying fungus variable and exist for the duration of the instance of Unity.</summary>
        Global,
    }

    /// <summary>
    /// Attribute class for variables.
    /// </summary>
    public sealed class VariableInfoAttribute : System.Attribute
    {
        //Note do not use "isPreviewedOnly:true", it causes the script to fail to load without errors shown
        public VariableInfoAttribute(string category, string variableType, int order = 0, bool isPreviewedOnly = false)
        {
            this.Category = category;
            this.VariableType = variableType;
            this.Order = order;
            this.IsPreviewedOnly = isPreviewedOnly;
        }
        
        public string Category { get; set; }
        public string VariableType { get; set; }
        public int Order { get; set; }
        public bool IsPreviewedOnly { get; set; }
    }

    /// <summary>
    /// Attribute class for variable properties.
    /// </summary>
    public sealed class VariablePropertyAttribute : PropertyAttribute 
    {
        public VariablePropertyAttribute (params System.Type[] variableTypes) 
        {
            this.VariableTypes = variableTypes;
        }

        public VariablePropertyAttribute(AllVariableTypes.VariableAny any)
        {
            VariableTypes = AllVariableTypes.AllFungusVarTypes;
        }

        public VariablePropertyAttribute (string defaultText, params System.Type[] variableTypes) 
        {
            this.defaultText = defaultText;
            this.VariableTypes = variableTypes;
        }

        public string defaultText = "<None>";
        public string compatibleVariableName = string.Empty;

        public System.Type[] VariableTypes { get; set; }
    }

    /// <summary>
    /// Abstract base class for variables.
    /// </summary>
    [RequireComponent(typeof(Flowchart))]
    [System.Serializable]
    public abstract class Variable : MonoBehaviour
    {
        [SerializeField] protected VariableScope scope;

        [SerializeField] protected string key = "";

        #region Public members

        /// <summary>
        /// Visibility scope for the variable.
        /// </summary>
        public virtual VariableScope Scope { get { return scope; } set { scope = value; } }

        /// <summary>
        /// String identifier for the variable.
        /// </summary>
        public virtual string Key { get { return key; } set { key = value; } }

        /// <summary>
        /// Callback to reset the variable if the Flowchart is reset.
        /// </summary>
        public abstract void OnReset();

        /// <summary>
        /// Used by SetVariable, child classes required to declare and implement operators.
        /// </summary>
        /// <param name="setOperator"></param>
        /// <param name="value"></param>
        public abstract void Apply(SetOperator setOperator, object value);

        /// <summary>
        /// Used by Ifs, While, and the like. Child classes required to declare and implement comparisons.
        /// </summary>
        /// <param name="compareOperator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool Evaluate(CompareOperator compareOperator, object value);

        /// <summary>
        /// Does the underlying type provide support for +-*/
        /// </summary>
        public virtual bool IsArithmeticSupported(SetOperator setOperator) { return false; }

        /// <summary>
        /// Does the underlying type provide support for < <= > >=
        /// </summary>
        public virtual bool IsComparisonSupported() { return false; }
        
        /// <summary>
        /// Boxed or referenced value of type defined within inherited types.
        /// Not recommended for direct use, primarily intended for use in editor code.
        /// </summary>
        public abstract object GetValue();

        //we are required to be on a flowchart so we provide this as a helper
        public virtual Flowchart GetFlowchart()
        {
            return GetComponent<Flowchart>();
        }
        #endregion
    }

    /// <summary>
    /// Generic concrete base class for variables.
    /// </summary>
    public abstract class VariableBase<T> : Variable
    {
        //caching mechanism for global static variables
        private VariableBase<T> _globalStaicRef;
        private VariableBase<T> globalStaicRef
        {
            get
            {
                if (_globalStaicRef != null)
                {
                    return _globalStaicRef;
                }
                else if(Application.isPlaying)
                {
                    return _globalStaicRef = FungusManager.Instance.GlobalVariables.GetOrAddVariable(Key, value, this.GetType());
                }
                else
                {
                    return null;
                }
            }
        }

        [SerializeField] protected T value;
        public virtual T Value
        {
            get
            {
                if (scope != VariableScope.Global || !Application.isPlaying)
                {
                    return this.value;
                }
                else
                { 
                    return globalStaicRef.value;
                }
            }
            set
            {
                if (scope != VariableScope.Global || !Application.isPlaying)
                {
                    this.value = value;
                }
                else
                {
                    globalStaicRef.Value = value;
                }
            }
        }

        public override object GetValue()
        {
            return value;
        }

        protected T startValue;

        public override void OnReset()
        {
            Value = startValue;
        }
        
        public override string ToString()
        {
            if (Value != null)
                return Value.ToString();
            else
                return "Null";
        }
        
        protected virtual void Start()
        {
            // Remember the initial value so we can reset later on
            startValue = Value;
        }

        //Apply to get from base system.object to T
        public override void Apply(SetOperator op, object value)
        {
            if(value is T || value == null)
            {
                Apply(op, (T)value);
            }
            else if(value is VariableBase<T>)
            {
                var vbg = value as VariableBase<T>;
                Apply(op, vbg.Value);
            }
            else
            {
                Debug.LogError("Cannot do Apply on variable, as object type: " + value.GetType().Name + " is incompatible with " + typeof(T).Name);
            }
        }

        public virtual void Apply(SetOperator setOperator, T value)
        {
            switch (setOperator)
            {
            case SetOperator.Assign:
                Value = value;
                break;
            default:
                Debug.LogError("The " + setOperator.ToString() + " set operator is not valid.");
                break;
            }
        }

        //Apply to get from base system.object to T
        public override bool Evaluate(CompareOperator op, object value)
        {
            if (value is T || value == null)
            {
                return Evaluate(op, (T)value);
            }
            else if (value is VariableBase<T>)
            {
                var vbg = value as VariableBase<T>;
                return Evaluate(op, vbg.Value);
            }
            else
            {
                Debug.LogError("Cannot do Evaluate on variable, as object type: " + value.GetType().Name + " is incompatible with " + typeof(T).Name);
            }

            return false;
        }

        public virtual bool Evaluate(CompareOperator compareOperator, T value)
        {
            bool condition = false;

            switch (compareOperator)
            {
            case CompareOperator.Equals:
                condition = Equals(Value, value);// Value.Equals(value);
                break;
            case CompareOperator.NotEquals:
                condition = !Equals(Value, value);
                break;
            default:
                Debug.LogError("The " + compareOperator.ToString() + " comparison operator is not valid.");
                break;
            }

            return condition;
        }

        public override bool IsArithmeticSupported(SetOperator setOperator)
        {
            return setOperator == SetOperator.Assign || base.IsArithmeticSupported(setOperator);
        }
    }
}
