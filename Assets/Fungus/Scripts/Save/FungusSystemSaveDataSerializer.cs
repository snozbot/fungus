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
    public class FungusSystemSaveDataSerializer : SaveDataSerializer
    {
        [System.Serializable]
        public class FungusSystemData
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
            public string lastMenuName, lastSayDialogName, lastViewName, lastStage;
            public int fungusPriority;
            public List<StageCharactersData> stages = new List<StageCharactersData>();
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

            foreach (var stage in Stage.ActiveStages)
            {
                var stageData = new FungusSystemData.StageCharactersData();
                stageData.stageName = stage.name;
                stageData.charactersOnStage = new FungusSystemData.CharacterPortraitData[stage.CharactersOnStage.Count];
                for (int i = 0; i < stage.CharactersOnStage.Count; i++)
                {
                    Character ch = stage.CharactersOnStage[i];
                    stageData.charactersOnStage[i] = new FungusSystemData.CharacterPortraitData()
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

            var port = new PortraitOptions();

            foreach (var stageData in fsData.stages)
            {
                var stage = GameObjectUtils.FindObjectOfTypeWithGameObjectName<Stage>(stageData.stageName);

                if(stage == null)
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
            if(activeStage != null)
            {
                Stage.MoveStageToFront(activeStage);
            } 
        }
    }
}