using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [System.Serializable]
    public class SavePointData
    {
        [SerializeField] protected string saveKey;
        [SerializeField] protected string sceneName;
        [SerializeField] protected List<FlowchartData> flowchartData = new List<FlowchartData>();

        #region Public methods

        public string SaveKey { get { return saveKey; } set { saveKey = value; } }
        public string SceneName { get { return sceneName; } set { sceneName = value; } }
        public List<FlowchartData> FlowchartData { get { return flowchartData; } set { flowchartData = value; } }

        #endregion
    }
}