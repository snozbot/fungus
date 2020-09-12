// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    ///
    /// </summary>
    public class FungusSystemSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string FungusSystemKey = "FungusSystemData";
        protected const int DataPriority = 1000;

        public string DataTypeKey => FungusSystemKey;
        public int Order => DataPriority;

        public StringPair[] Encode()
        {
            var fsData = new FungusSystemSaveDataItem();
            fsData.textVariationHistory = TextVariationHandler.GetSerialisedHistory();
            if(FungusManager.Instance.NarrativeLog != null)
                fsData.narLogEntries = FungusManager.Instance.NarrativeLog.GetAllNarrativeLogItems();
            
            if(MenuDialog.GetMenuDialog() != null)
                fsData.lastMenuName = MenuDialog.GetMenuDialog().gameObject.name;

            if(SayDialog.GetSayDialog() != null)
                fsData.lastSayDialogName = SayDialog.GetSayDialog().gameObject.name;

            if (FungusManager.Instance.CameraManager != null)
            {
                var lv = FungusManager.Instance.CameraManager.LastView;
                fsData.lastViewName = lv != null ? lv.gameObject.name : string.Empty;
                fsData.fungusPriority = FungusPrioritySignals.CurrentPriorityDepth;
                fsData.progressMarkerName = ProgressMarker.LastExecutedCustomKey;
            }

            foreach (var stage in Stage.ActiveStages)
            {
                var stageData = new FungusSystemSaveDataItem.StageCharactersData();
                stageData.stageName = stage.name;
                stageData.charactersOnStage = new FungusSystemSaveDataItem.CharacterPortraitData[stage.CharactersOnStage.Count];
                for (int i = 0; i < stage.CharactersOnStage.Count; i++)
                {
                    Character ch = stage.CharactersOnStage[i];
                    stageData.charactersOnStage[i] = new FungusSystemSaveDataItem.CharacterPortraitData()
                    {
                        characterName = ch.name,
                        dimmed = ch.State.dimmed,
                        displayType = ch.State.display,
                        facing = ch.State.facing,
                        portraitLocationName = ch.State.position.name,
                        visiblePortraitName = ch.State.portrait.name
                    };
                }
            }

            var activeStage = Stage.GetActiveStage();
            fsData.lastStage = activeStage != null ? activeStage.gameObject.name : string.Empty;

            return SaveDataItemUtility.CreateSingleElement(DataTypeKey, fsData);
        }

        public bool Decode(StringPair sdi)
        {
            var fsData = JsonUtility.FromJson<FungusSystemSaveDataItem>(sdi.val);
            if (fsData == null)
            {
                Debug.LogError("Failed to decode FungusSystemSaveDataItem");
                return false;
            }

            TextVariationHandler.RestoreFromSerialisedHistory(fsData.textVariationHistory);
            
            if(fsData.narLogEntries.Count > 0)
                FungusManager.Instance.NarrativeLog.LoadHistory(fsData.narLogEntries);
            
            MenuDialog.ActiveMenuDialog = GameObjectUtils.FindObjectOfTypeWithGameObjectName<MenuDialog>(fsData.lastMenuName);
            SayDialog.ActiveSayDialog = GameObjectUtils.FindObjectOfTypeWithGameObjectName<SayDialog>(fsData.lastSayDialogName);

            var v = GameObjectUtils.FindObjectOfTypeWithGameObjectName<View>(fsData.lastViewName);
            if (v != null)
            {
                FungusManager.Instance.CameraManager.PanToView(Camera.main, v, 0f, null);
            }

            FungusPrioritySignals.DoResetPriority();
            for (int i = 0; i < fsData.fungusPriority; i++)
            {
                FungusPrioritySignals.DoIncreasePriorityDepth();
            }

            var port = new PortraitOptions();

            foreach (var stageData in fsData.stages)
            {
                var stage = GameObjectUtils.FindObjectOfTypeWithGameObjectName<Stage>(stageData.stageName);

                if (stage == null)
                {
                    Debug.LogError("Cannot find stage of name " + stageData.stageName +
                        ". Skipping this block of stage save data.");
                    continue;
                }

                foreach (var ch in stageData.charactersOnStage)
                {
                    port.Reset(true);
                    port.character = Character.ActiveCharacters.FirstOrDefault(x => x.name == ch.characterName);
                    if (port.character == null)
                    {
                        Debug.LogError("Could not find matching character name " + ch.characterName + ". Skipping loading character to stage.");
                        continue;
                    }

                    port.portrait = port.character.GetPortrait(ch.visiblePortraitName);
                    port.display = ch.displayType;
                    port.fromPosition = stage.GetPosition(ch.portraitLocationName);
                    port.toPosition = port.fromPosition;
                    port.facing = ch.facing;
                    stage.RunPortraitCommand(port, null);
                    stage.SetDimmed(port.character, ch.dimmed);
                }
            }

            var activeStage = GameObjectUtils.FindObjectOfTypeWithGameObjectName<Stage>(fsData.lastStage);
            if (activeStage != null)
            {
                Stage.MoveStageToFront(activeStage);
            }

            ProgressMarker.LatestExecuted = ProgressMarker.FindWithKey(fsData.progressMarkerName);

            return true;
        }

        public void PreDecode()
        {
        }

        public void PostDecode()
        {
            SaveLoaded.NotifyEventHandlers(ProgressMarker.LastExecutedCustomKey);
        }

        [System.Serializable]
        public class FungusSystemSaveDataItem
        {
            [System.Serializable]
            public class StageCharactersData
            {
                public string stageName;
                public CharacterPortraitData[] charactersOnStage;
            }

            [System.Serializable]
            public struct CharacterPortraitData
            {
                public string characterName, visiblePortraitName, portraitLocationName;
                public bool dimmed;
                public FacingDirection facing;
                public DisplayType displayType;
            }

            public List<int> textVariationHistory = new List<int>();
            public List<NarrativeLogEntry> narLogEntries;
            public string lastMenuName, lastSayDialogName, lastViewName, lastStage, progressMarkerName;
            public int fungusPriority;
            public List<StageCharactersData> stages = new List<StageCharactersData>();
        }
    }
}