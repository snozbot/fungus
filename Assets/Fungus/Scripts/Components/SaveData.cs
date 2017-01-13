using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Contains a list of game objects whose state will be saved for each Save Point.
    /// </summary>
    public class SaveData : MonoBehaviour
    {
        [Tooltip("A list of Flowchart objects whose variables will be encoded in the save data. Boolean, Integer, Float and String variables are supported.")]
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        #region Public methods

        /// <summary>
        /// Encodes the list of Flowcharts and adds it to a Save Point Data object.
        /// </summary>
        public void Encode(SavePointData savePointData)
        {
            for (int i = 0; i < flowcharts.Count; i++)
            {
                var flowchart = flowcharts[i];
                var flowchartData = FlowchartData.Encode(flowchart);
                savePointData.FlowchartDatas.Add(flowchartData);
            }
        }

        /// <summary>
        /// Decodes the Flowchart data from the save point data.
        /// </summary>
        public void Decode(SavePointData savePointData)
        {
            for (int i = 0; i < savePointData.FlowchartDatas.Count; i++)
            {
                var flowchartData = savePointData.FlowchartDatas[i];
                FlowchartData.Decode(flowchartData);
            }            
        }

        #endregion
    }
}