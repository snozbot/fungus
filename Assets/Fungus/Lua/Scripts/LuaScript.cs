// Adapted from the Unity Test Tools project (MIT license)
// https://bitbucket.org/Unity-Technologies/unitytesttools/src/a30d562427e9/Assets/UnityTestTools/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Fungus
{
    
    [Serializable]
    public class LuaScript : MonoBehaviour, ILuaScriptConfigurator
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
            
        [SerializeField] public float executeAfterTime = 1f;
        [SerializeField] public bool repeatExecuteTime = true;
        [SerializeField] public float repeatEveryTime = 1f;
        [SerializeField] public int executeAfterFrames = 1;
        [SerializeField] public bool repeatExecuteFrame = true;
        [SerializeField] public int repeatEveryFrame = 1;
        [SerializeField] public bool hasFailed;

        [SerializeField] public ExecuteMethod executeMethods = ExecuteMethod.Start;

        /// <summary>
        /// The Lua Environment to use when executing Lua script.
        /// </summary>
		[Tooltip("The Lua Environment to use when executing Lua script.")]
        public LuaEnvironment luaEnvironment;

        /// <summary>
        /// Lua script file to execute.
        /// </summary>
        [Tooltip("Lua script file to execute.")]
        public TextAsset luaFile;

        /// <summary>
        /// Lua script to execute.
        /// </summary>
        [Tooltip("Lua script to execute.")]
        [TextArea(5, 50)]
        public string luaScript = "";

        /// <summary>
        /// Run the script as a Lua coroutine so execution can be yielded for asynchronous operations.
        /// </summary>
        [Tooltip("Run the script as a Lua coroutine so execution can be yielded for asynchronous operations.")]
        public bool runAsCoroutine = true;

        /// <summary>
        /// Require the fungus Lua module at the start of the script. Equivalent to 'local fungus = require('fungus')
        /// </summary>
        [Tooltip("Require the fungus Lua module at the start of the script. Equivalent to 'local fungus = require('fungus')")]
        public bool useFungusModule = true;

        private int m_ExecuteOnFrame;

        protected string friendlyName = "";

        // Recursively build the full hierarchy path to this game object
        private static string GetPath(Transform current) 
        {
            if (current.parent == null)
            {
                return current.name;
            }
            return GetPath(current.parent) + "." + current.name;
        }

        public void Start()
        {
			if (luaEnvironment == null)        
			{
				// Create a Lua Environment if none exists yet
				luaEnvironment = LuaEnvironment.GetLua();
			}

			if (luaEnvironment == null)        
			{
				Debug.LogError("No Lua Environment found");
				return;
			}
					                
            // Cache a descriptive name to use in Lua error messages
            friendlyName = GetPath(transform) + ".LuaScript";

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

        public IEnumerator ExecutePeriodically()
        {
            yield return new WaitForSeconds(executeAfterTime);
            Execute(ExecuteMethod.AfterPeriodOfTime);
            while (repeatExecuteTime)
            {
                yield return new WaitForSeconds(repeatEveryTime);
                Execute(ExecuteMethod.AfterPeriodOfTime);
            }
        }

        public bool ShouldExecuteOnFrame()
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

        public void OnDisable()
        {
            Execute(ExecuteMethod.OnDisable);
        }

        public void OnEnable()
        {
            Execute(ExecuteMethod.OnEnable);
        }

        public void OnDestroy()
        {
            Execute(ExecuteMethod.OnDestroy);
        }

        public void Update()
        {
            if (IsExecuteMethodSelected(ExecuteMethod.Update) && ShouldExecuteOnFrame())
            {
                Execute(ExecuteMethod.Update);
            }
        }

        public void FixedUpdate()
        {
            Execute(ExecuteMethod.FixedUpdate);
        }

        public void LateUpdate()
        {
            Execute(ExecuteMethod.LateUpdate);
        }

        public void OnControllerColliderHit()
        {
            Execute(ExecuteMethod.OnControllerColliderHit);
        }

        public void OnParticleCollision()
        {
            Execute(ExecuteMethod.OnParticleCollision);
        }

        public void OnJointBreak()
        {
            Execute(ExecuteMethod.OnJointBreak);
        }

        public void OnBecameInvisible()
        {
            Execute(ExecuteMethod.OnBecameInvisible);
        }

        public void OnBecameVisible()
        {
            Execute(ExecuteMethod.OnBecameVisible);
        }

        public void OnTriggerEnter()
        {
            Execute(ExecuteMethod.OnTriggerEnter);
        }

        public void OnTriggerExit()
        {
            Execute(ExecuteMethod.OnTriggerExit);
        }

        public void OnTriggerStay()
        {
            Execute(ExecuteMethod.OnTriggerStay);
        }

        public void OnCollisionEnter()
        {
            Execute(ExecuteMethod.OnCollisionEnter);
        }

        public void OnCollisionExit()
        {
            Execute(ExecuteMethod.OnCollisionExit);
        }

        public void OnCollisionStay()
        {
            Execute(ExecuteMethod.OnCollisionStay);
        }

        public void OnTriggerEnter2D()
        {
            Execute(ExecuteMethod.OnTriggerEnter2D);
        }

        public void OnTriggerExit2D()
        {
            Execute(ExecuteMethod.OnTriggerExit2D);
        }

        public void OnTriggerStay2D()
        {
            Execute(ExecuteMethod.OnTriggerStay2D);
        }

        public void OnCollisionEnter2D()
        {
            Execute(ExecuteMethod.OnCollisionEnter2D);
        }

        public void OnCollisionExit2D()
        {
            Execute(ExecuteMethod.OnCollisionExit2D);
        }

        public void OnCollisionStay2D()
        {
            Execute(ExecuteMethod.OnCollisionStay2D);
        }

        private void Execute(ExecuteMethod executeMethod)
        {
            if (IsExecuteMethodSelected(executeMethod))
            {
                Execute();
            }
        }

        public bool IsExecuteMethodSelected(ExecuteMethod method)
        {
            return method == (executeMethods & method);
        }

        /// <summary>
        /// Execute the Lua script immediately.
        /// This is the function to call if you want to trigger execution from an external script.
        /// </summary>
        public virtual void Execute()
        {
            if (luaEnvironment == null)
            {
                Debug.LogWarning("No Lua Environment found");
            }
            else
            {
                string s = "";
                if (useFungusModule)
                {
                    s = "fungus = require('fungus')\n";
                }
 
                if (luaFile != null)
                {
                    s += luaFile.text;
                }
                else if (luaScript.Length > 0)
                {
                    s += luaScript;
                }

                luaEnvironment.DoLuaString(s, friendlyName, runAsCoroutine);
            }
        }
            
        #region AssertionComponentConfigurator
        public int UpdateExecuteStartOnFrame { set { executeAfterFrames = value; } }
        public int UpdateExecuteRepeatFrequency { set { repeatEveryFrame = value; } }
        public bool UpdateExecuteRepeat { set { repeatExecuteFrame = value; } }
        public float TimeExecuteStartAfter { set { executeAfterTime = value; } }
        public float TimeExecuteRepeatFrequency { set { repeatEveryTime = value; } }
        public bool TimeExecuteRepeat { set { repeatExecuteTime = value; } }
        public LuaScript Component { get { return this; } }
        #endregion
    }

    public interface ILuaScriptConfigurator
    {
        /// <summary>
        /// If the assertion is evaluated in Update, after how many frame should the evaluation start. Defult is 1 (first frame)
        /// </summary>
        int UpdateExecuteStartOnFrame { set; }
        /// <summary>
        /// If the assertion is evaluated in Update and UpdateExecuteRepeat is true, how many frame should pass between evaluations
        /// </summary>
        int UpdateExecuteRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated in Update, should the evaluation be repeated after UpdateExecuteRepeatFrequency frames
        /// </summary>
        bool UpdateExecuteRepeat { set; }

        /// <summary>
        /// If the assertion is evaluated after a period of time, after how many seconds the first evaluation should be done
        /// </summary>
        float TimeExecuteStartAfter { set; }
        /// <summary>
        /// If the assertion is evaluated after a period of time and TimeExecuteRepeat is true, after how many seconds should the next evaluation happen
        /// </summary>
        float TimeExecuteRepeatFrequency { set; }
        /// <summary>
        /// If the assertion is evaluated after a period, should the evaluation happen again after TimeExecuteRepeatFrequency seconds
        /// </summary>
        bool TimeExecuteRepeat { set; }

        LuaScript Component { get; }
    }

}
