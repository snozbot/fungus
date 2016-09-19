// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
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
    /// <summary>
    /// Executes an LuaScript component in the same gameobject when a condition occurs.
    /// </summary>
    public class ExecuteHandler : MonoBehaviour, IExecuteHandler, IExecuteHandlerConfigurator
    {
        [SerializeField] protected float executeAfterTime = 1f;

        [SerializeField] protected bool repeatExecuteTime = true;

        [SerializeField] protected float repeatEveryTime = 1f;

        [SerializeField] protected int executeAfterFrames = 1;

        [SerializeField] protected bool repeatExecuteFrame = true;

        [SerializeField] protected int repeatEveryFrame = 1;

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

        #region AssertionComponentConfigurator

        public int UpdateExecuteStartOnFrame { set { executeAfterFrames = value; } }

        public int UpdateExecuteRepeatFrequency { set { repeatEveryFrame = value; } }

        public bool UpdateExecuteRepeat { set { repeatExecuteFrame = value; } }

        public float TimeExecuteStartAfter { set { executeAfterTime = value; } }

        public float TimeExecuteRepeatFrequency { set { repeatEveryTime = value; } }

        public bool TimeExecuteRepeat { set { repeatExecuteTime = value; } }

        public ExecuteHandler Component { get { return this; } }

        #endregion

        #region IExecuteHandler implementation

        public virtual float ExecuteAfterTime { get { return executeAfterTime; } set { executeAfterTime = value; } }

        public virtual bool RepeatExecuteTime { get { return repeatExecuteTime; } set { repeatExecuteTime = value; } }

        public virtual float RepeatEveryTime { get { return repeatEveryTime; } set { repeatEveryTime = value; } }

        public virtual int ExecuteAfterFrames { get { return executeAfterFrames; } set { executeAfterFrames = value; } }

        public virtual bool RepeatExecuteFrame { get { return repeatExecuteFrame; } set { repeatExecuteFrame = value; } }

        public virtual int RepeatEveryFrame { get { return repeatEveryFrame; } set { repeatEveryFrame = value; } }

        public virtual ExecuteMethod ExecuteMethods { get { return executeMethods; } set { executeMethods = value; } }

        public virtual bool IsExecuteMethodSelected(ExecuteMethod method)
        {
            return method == (executeMethods & method);
        }

        public virtual void Execute()
        {
            // Call any OnExecute methods in components on this gameobject
            if (executeMethodName != "")
            {
                SendMessage(executeMethodName, SendMessageOptions.DontRequireReceiver);
            }
        }

        #endregion
    }
}
