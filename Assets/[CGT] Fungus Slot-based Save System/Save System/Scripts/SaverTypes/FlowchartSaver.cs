using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace Fungus.SaveSystem
{
    /// <summary>
    /// Creates Flowchart save data.
    /// </summary>
    public class FlowchartSaver : DataSaver<FlowchartData>, ISaveCreator<FlowchartData, Flowchart>,
                                    IGroupSaver<FlowchartData>
    {

        [Tooltip("A list of Flowchart objects whose variables will be encoded in the save data. Boolean, Integer, Float and String variables are supported.")]
        [SerializeField] protected Flowchart[] flowcharts =               null;

        #region Methods

        /// <summary>
        /// Creates a single FlowchartData object from a Flowchart passed to it.
        /// </summary>
        public virtual FlowchartData CreateSave(Flowchart flowchart)
        {
            return new FlowchartData(flowchart);
        }

        /// <summary>
        /// Creates a group of Flowchart save data objects from the Flowcharts it has set in the Inspector.
        /// </summary>
        public virtual IList<FlowchartData> CreateSaves()
        {
            var saveGroup =                                     new FlowchartData[flowcharts.Length];

            for (int i = 0; i < flowcharts.Length; i++)
            {
                var flowchart =                                 flowcharts[i];
                if (flowchart == null) 
                {
                    Debug.LogWarning("There is a null flowchart in the flowcharts array " + this.name + " has.");
                    continue;
                }
                var newData =                                   CreateSave(flowchart);
                saveGroup[i] =                                  newData;
            }

            return saveGroup;
        }

        /// <summary>
        /// Encodes the passed FlowchartData group into an array of SaveDataItems.
        /// </summary>
        public virtual IList<SaveDataItem> CreateItems(IList<FlowchartData> dataGroup)
        {
            var itemGroup =                                         new SaveDataItem[dataGroup.Count];
            
            for (int i = 0; i < dataGroup.Count; i++)
            {
                var fcData =                                        dataGroup[i];
                var newItem =                                       CreateItem(fcData);
                itemGroup[i] =                                      newItem;
            }

            return itemGroup;
        }

        /// <summary>
        /// Encodes the flowcharts in the scene to an array of SaveDataItems, and returns it.
        /// </summary>
        public override IList<SaveDataItem> CreateItems()
        {
            var flowchartSaves =                        CreateSaves();
            var items =                                 new SaveDataItem[flowchartSaves.Count];

            for (int i = 0; i < flowchartSaves.Count; i++)
            {
                var flowchartSave =                     flowchartSaves[i];
                var newItem =                           CreateItem(flowchartSave);
                items[i] =                              newItem;
            }

            return items;
        }

        /// <summary>
        /// Creates a single SaveDataItem from a single FlowchartData object.
        /// </summary>
        public virtual SaveDataItem CreateItem(FlowchartData data)
        {
            var jsonString =                            JsonUtility.ToJson(data);
            var newItem =                               new SaveDataItem(saveType.Name, jsonString);
            return newItem;
        }


        #endregion

    }
}