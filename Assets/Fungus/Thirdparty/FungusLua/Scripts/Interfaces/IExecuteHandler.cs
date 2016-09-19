// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;

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
    public interface IExecuteHandler
    {
        float ExecuteAfterTime { get; set; }

        bool RepeatExecuteTime { get; set; }

        float RepeatEveryTime { get; set; }

        int ExecuteAfterFrames { get; set; }

        bool RepeatExecuteFrame { get; set; }

        int RepeatEveryFrame { get; set; }

        ExecuteMethod ExecuteMethods { get; set; }

        /// <summary>
        /// Returns true if the specified execute method option has been enabled.
        /// </summary>
        bool IsExecuteMethodSelected(ExecuteMethod method);

        /// <summary>
        /// Execute the Lua script immediately.
        /// This is the function to call if you want to trigger execution from an external script.
        /// </summary>
        void Execute();
    }

    public interface IExecuteHandlerConfigurator
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

        ExecuteHandler Component { get; }
    }
}