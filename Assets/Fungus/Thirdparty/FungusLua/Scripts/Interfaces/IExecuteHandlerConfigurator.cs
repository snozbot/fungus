// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
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

        /// <summary>
        /// Returns the ExecuteHandler component.
        /// </summary>
        ExecuteHandler Component { get; }
    }
}