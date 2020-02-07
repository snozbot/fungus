// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Adapted from the Unity Test Tools project (MIT license)
// https://bitbucket.org/Unity-Technologies/unitytesttools/src/a30d562427e9/Assets/UnityTestTools/

using System;
using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Fungus
{
    [Flags]
    public enum ExecuteMethod
    {
        AfterPeriodOfTime       = 1 << 0,
        Start                   = 1 << 1,
        Update                  = 1 << 2,
        FixedUpdate             = 1 << 3,
        LateUpdate              = 1 << 4,
        OnDestroy               = 1 << 5,
        OnEnable                = 1 << 6,
        OnDisable               = 1 << 7,
        OnControllerColliderHit = 1 << 8,
        OnParticleCollision     = 1 << 9,
        OnJointBreak            = 1 << 10,
        OnBecameInvisible       = 1 << 11,
        OnBecameVisible         = 1 << 12,
        OnTriggerEnter          = 1 << 13,
        OnTriggerExit           = 1 << 14,
        OnTriggerStay           = 1 << 15,
        OnCollisionEnter        = 1 << 16,
        OnCollisionExit         = 1 << 17,
        OnCollisionStay         = 1 << 18,
        OnTriggerEnter2D        = 1 << 19,
        OnTriggerExit2D         = 1 << 20,
        OnTriggerStay2D         = 1 << 21,
        OnCollisionEnter2D      = 1 << 22,
        OnCollisionExit2D       = 1 << 23,
        OnCollisionStay2D       = 1 << 24,
    }

    /// <summary>
    /// Executes an LuaScript component in the same gameobject when a condition occurs.
    /// </summary>
    public class ExecuteHandler : MonoBehaviour, IExecuteHandlerConfigurator
    {
        [Tooltip("Execute after a period of time.")]
        [SerializeField] protected float executeAfterTime = 1f;

        [Tooltip("Repeat execution after a period of time.")]
        [SerializeField] protected bool repeatExecuteTime = true;

        [Tooltip("Repeat forever.")]
        [SerializeField] protected float repeatEveryTime = 1f;

        [Tooltip("Execute after a number of frames have elapsed.")]
        [SerializeField] protected int executeAfterFrames = 1;

        [Tooltip("Repeat execution after a number of frames have elapsed.")]
        [SerializeField] protected bool repeatExecuteFrame = true;

        [Tooltip("Execute on every frame.")]
        [SerializeField] protected int repeatEveryFrame = 1;

        [Tooltip("The bitmask for the currently selected execution methods.")]
        [SerializeField] protected ExecuteMethod executeMethods = ExecuteMethod.Start;

        [Tooltip("Name of the method on a component in this gameobject to call when executing.")]
        [SerializeField] protected string executeMethodName = "OnExecute";

        protected int m_ExecuteOnFrame;

        // Recursively build the full hierarchy path to this game object
        protected static string GetPath(Transform current) 
        {
            if (current.parent == null)
            {
                return current.name;
            }
            return GetPath(current.parent) + "." + current.name;
        }

        protected void Start()
        {
            Execute(ExecuteMethod.Start);

            if (IsExecuteMethodSelected(ExecuteMethod.AfterPeriodOfTime))
            {
                StartCoroutine(ExecutePeriodically());
            }
            if (IsExecuteMethodSelected(ExecuteMethod.Update))
            {
                m_ExecuteOnFrame = Time.frameCount + executeAfterFrames;
            }
        }

        protected IEnumerator ExecutePeriodically()
        {
            yield return new WaitForSeconds(executeAfterTime);
            Execute(ExecuteMethod.AfterPeriodOfTime);
            while (repeatExecuteTime)
            {
                yield return new WaitForSeconds(repeatEveryTime);
                Execute(ExecuteMethod.AfterPeriodOfTime);
            }
        }

        protected bool ShouldExecuteOnFrame()
        {
            if (Time.frameCount > m_ExecuteOnFrame)
            {
                if (repeatExecuteFrame)
                    m_ExecuteOnFrame += repeatEveryFrame;
                else
                    m_ExecuteOnFrame = Int32.MaxValue;
                return true;
            }
            return false;
        }

        protected void OnDisable()
        {
            Execute(ExecuteMethod.OnDisable);
        }

        protected void OnEnable()
        {
            Execute(ExecuteMethod.OnEnable);
        }

        protected void OnDestroy()
        {
            Execute(ExecuteMethod.OnDestroy);
        }

        protected void Update()
        {
            if (IsExecuteMethodSelected(ExecuteMethod.Update) && ShouldExecuteOnFrame())
            {
                Execute(ExecuteMethod.Update);
            }
        }

        protected void FixedUpdate()
        {
            Execute(ExecuteMethod.FixedUpdate);
        }

        protected void LateUpdate()
        {
            Execute(ExecuteMethod.LateUpdate);
        }

        protected void OnControllerColliderHit()
        {
            Execute(ExecuteMethod.OnControllerColliderHit);
        }

        protected void OnParticleCollision()
        {
            Execute(ExecuteMethod.OnParticleCollision);
        }

        protected void OnJointBreak()
        {
            Execute(ExecuteMethod.OnJointBreak);
        }

        protected void OnBecameInvisible()
        {
            Execute(ExecuteMethod.OnBecameInvisible);
        }

        protected void OnBecameVisible()
        {
            Execute(ExecuteMethod.OnBecameVisible);
        }

        protected void OnTriggerEnter()
        {
            Execute(ExecuteMethod.OnTriggerEnter);
        }

        protected void OnTriggerExit()
        {
            Execute(ExecuteMethod.OnTriggerExit);
        }

        protected void OnTriggerStay()
        {
            Execute(ExecuteMethod.OnTriggerStay);
        }

        protected void OnCollisionEnter()
        {
            Execute(ExecuteMethod.OnCollisionEnter);
        }

        protected void OnCollisionExit()
        {
            Execute(ExecuteMethod.OnCollisionExit);
        }

        protected void OnCollisionStay()
        {
            Execute(ExecuteMethod.OnCollisionStay);
        }

        protected void OnTriggerEnter2D()
        {
            Execute(ExecuteMethod.OnTriggerEnter2D);
        }

        protected void OnTriggerExit2D()
        {
            Execute(ExecuteMethod.OnTriggerExit2D);
        }

        protected void OnTriggerStay2D()
        {
            Execute(ExecuteMethod.OnTriggerStay2D);
        }

        protected void OnCollisionEnter2D()
        {
            Execute(ExecuteMethod.OnCollisionEnter2D);
        }

        protected void OnCollisionExit2D()
        {
            Execute(ExecuteMethod.OnCollisionExit2D);
        }

        protected void OnCollisionStay2D()
        {
            Execute(ExecuteMethod.OnCollisionStay2D);
        }

        protected void Execute(ExecuteMethod executeMethod)
        {
            if (IsExecuteMethodSelected(executeMethod))
            {
                Execute();
            }
        }
            
        #region Public members

        /// <summary>
        /// Execute after a period of time.
        /// </summary>
        public virtual float ExecuteAfterTime { get { return executeAfterTime; } set { executeAfterTime = value; } }

        /// <summary>
        /// Repeat execution after a period of time.
        /// </summary>
        public virtual bool RepeatExecuteTime { get { return repeatExecuteTime; } set { repeatExecuteTime = value; } }

        /// <summary>
        /// Repeat forever.
        /// </summary>
        public virtual float RepeatEveryTime { get { return repeatEveryTime; } set { repeatEveryTime = value; } }

        /// <summary>
        /// Execute after a number of frames have elapsed.
        /// </summary>
        public virtual int ExecuteAfterFrames { get { return executeAfterFrames; } set { executeAfterFrames = value; } }

        /// <summary>
        /// Repeat execution after a number of frames have elapsed.
        /// </summary>
        public virtual bool RepeatExecuteFrame { get { return repeatExecuteFrame; } set { repeatExecuteFrame = value; } }

        /// <summary>
        /// Execute on every frame.
        /// </summary>
        public virtual int RepeatEveryFrame { get { return repeatEveryFrame; } set { repeatEveryFrame = value; } }

        /// <summary>
        /// The bitmask for the currently selected execution methods.
        /// </summary>
        public virtual ExecuteMethod ExecuteMethods { get { return executeMethods; } set { executeMethods = value; } }

        /// <summary>
        /// Returns true if the specified execute method option has been enabled.
        /// </summary>
        public virtual bool IsExecuteMethodSelected(ExecuteMethod method)
        {
            return method == (executeMethods & method);
        }

        /// <summary>
        /// Execute the Lua script immediately.
        /// This is the function to call if you want to trigger execution from an external script.
        /// </summary>
        public virtual void Execute()
        {
            // Call any OnExecute methods in components on this gameobject
            if (executeMethodName != "")
            {
                SendMessage(executeMethodName, SendMessageOptions.DontRequireReceiver);
            }
        }

        #endregion

        #region AssertionComponentConfigurator implementation

        public int UpdateExecuteStartOnFrame { set { executeAfterFrames = value; } }

        public int UpdateExecuteRepeatFrequency { set { repeatEveryFrame = value; } }

        public bool UpdateExecuteRepeat { set { repeatExecuteFrame = value; } }

        public float TimeExecuteStartAfter { set { executeAfterTime = value; } }

        public float TimeExecuteRepeatFrequency { set { repeatEveryTime = value; } }

        public bool TimeExecuteRepeat { set { repeatExecuteTime = value; } }

        public ExecuteHandler Component { get { return this; } }

        #endregion
    }
}
