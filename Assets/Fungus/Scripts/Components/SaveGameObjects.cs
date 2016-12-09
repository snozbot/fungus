using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    [System.Serializable]
    public class SaveGameObjects : MonoBehaviour
    {
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        #region Public methods

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