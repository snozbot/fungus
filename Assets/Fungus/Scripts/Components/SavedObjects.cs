using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Contains a list of game objects whose state will be saved for each Save Point.
    /// </summary>
    public class SavedObjects : MonoBehaviour
    {
        [Tooltip("A list of Flowchart objects whose variables will be encoded in the save data. Boolean, Integer, Float and String variables are supported.")]
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        #region Public methods

        /// <summary>
        /// Encodes the list of saved objects and adds it to a Save Point Data object.
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

        #endregion
    }
}