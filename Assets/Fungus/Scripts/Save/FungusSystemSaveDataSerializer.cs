// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// 
    /// </summary>
    public class FungusSystemSaveDataSerializer : SaveDataSerializer
    {
        [System.Serializable]
        public class FungusSystemData
        {
            //public class StageCharacterPortrait { public string characterName, visiblePortrait, portraitLocation; }

            public List<int> textVariationHistory = new List<int>();
            public List<NarrativeLogEntry> narLogEntries;
            public string lastMenuName, lastSayDialogName, lastViewName, lastStage;
            public int fungusPriority;
            //portrait showing , character, spriteName, stage location name
            //public List<StageCharacterPortrait> stageCharacterPortraits = new List<StageCharacterPortrait>();
        }

        protected const string FungusSystemKey = "FungusSystemData";
        protected const int DataPriority = 1000;

        public override string DataTypeKey => FungusSystemKey;
        public override int Order => DataPriority;

        public override void Encode(SavePointData data)
        {
            var fsData = new FungusSystemData();
            fsData.textVariationHistory = TextVariationHandler.GetSerialisedHistory();
            fsData.narLogEntries = FungusManager.Instance.NarrativeLog.GetAllNarrativeLogItems();
            fsData.lastMenuName = MenuDialog.GetMenuDialog().gameObject.name;
            fsData.lastSayDialogName = SayDialog.GetSayDialog().gameObject.name;
            var lv = FungusManager.Instance.CameraManager.LastView;
            fsData.lastViewName = lv != null ? lv.gameObject.name : string.Empty;
            fsData.fungusPriority = FungusPrioritySignals.CurrentPriorityDepth;
            var stage = Stage.GetActiveStage();
            fsData.lastStage = stage != null ? stage.gameObject.name : string.Empty;

            var tvDataItem = SaveDataItem.Create(FungusSystemKey, JsonUtility.ToJson(fsData));
            data.SaveDataItems.Add(tvDataItem);
        }

        protected override void ProcessItem(SaveDataItem sditem)
        {
            var fsData = JsonUtility.FromJson<FungusSystemData>(sditem.Data);
            if (fsData == null)
            {
                Debug.LogError("Failed to decode Text Variation save data item");
                return;
            }

            TextVariationHandler.RestoreFromSerialisedHistory(fsData.textVariationHistory);
            FungusManager.Instance.NarrativeLog.LoadHistory(fsData.narLogEntries);
            MenuDialog.ActiveMenuDialog = GameObjectUtils.FindObjectOfTypeWithGameObjectName<MenuDialog>(fsData.lastMenuName);
            SayDialog.ActiveSayDialog = GameObjectUtils.FindObjectOfTypeWithGameObjectName<SayDialog>(fsData.lastSayDialogName);
            
            var v = GameObjectUtils.FindObjectOfTypeWithGameObjectName<View>(fsData.lastViewName);
            if(v != null)
            {
                FungusManager.Instance.CameraManager.PanToView(Camera.main, v, 0f, null);
            }

            FungusPrioritySignals.DoResetPriority();
            for (int i = 0; i < fsData.fungusPriority; i++)
            {
                FungusPrioritySignals.DoIncreasePriorityDepth();
            }

            var stage = GameObjectUtils.FindObjectOfTypeWithGameObjectName<Stage>(fsData.lastStage);
            if(stage != null)
            {
                Stage.MoveStageToFront(stage);
            } 
        }
    }
}