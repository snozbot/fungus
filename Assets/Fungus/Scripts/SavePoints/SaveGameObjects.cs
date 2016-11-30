using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    [System.Serializable]
    public class SaveGameObjects
    {
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        #region Public methods

        public List<Flowchart> Flowcharts { get { return flowcharts; } }

        #endregion
    }
}