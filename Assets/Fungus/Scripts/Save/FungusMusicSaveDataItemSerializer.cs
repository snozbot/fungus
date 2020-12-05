// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    [System.Serializable]
    /// <summary>
    /// This component encodes and decodes the added flowcharts and their blocks and running states.
    /// </summary>
    public class FungusMusicSaveDataItemSerializer : ISaveDataItemSerializer
    {
        [SerializeField] protected List<AudioClip> possibleMusicAudioClips, possibleAmbientAudioClips;

        public List<AudioClip> PossibleMusicAudioClips { get => possibleMusicAudioClips; set => possibleMusicAudioClips = value; }
        public List<AudioClip> PossibleAmbientAudioClips { get => possibleAmbientAudioClips; set => possibleAmbientAudioClips = value; }

        protected const string MusicDataKey = "MusicData";
        protected const int MusicDataPriority = 1000;

        public string DataTypeKey => MusicDataKey;

        public int Order => MusicDataPriority;

        public StringPair[] Encode()
        {
            var mm = FungusManager.Instance.MusicManager;

            var data = new FungusMusicSaveDataItem()
            {
                amb = new FungusMusicSaveDataItem.MusicSourceSaveInfo()
                {
                    clipName = mm.TargetAmbClip?.name ?? string.Empty,
                    pitch = mm.TargetAmbPitch,
                    position = mm.AudioSourceAmbiance.isPlaying ? mm.AudioSourceAmbiance.time : 0,
                    vol = mm.TargetAmbVolume,
                    loop = mm.AudioSourceAmbiance.loop
                },
                music = new FungusMusicSaveDataItem.MusicSourceSaveInfo()
                {
                    clipName = mm.TargetMusicClip?.name ?? string.Empty,
                    pitch = mm.TargetAmbPitch,
                    position = mm.AudioSourceMusic.isPlaying ? mm.AudioSourceMusic.time : 0,
                    vol = mm.TargetMusicVolume,
                    loop = mm.AudioSourceMusic.loop
                }
            };
            return SaveDataItemUtils.CreateSingleElement(DataTypeKey, data);
        }

        public bool Decode(StringPair sdi)
        {
            var data = JsonUtility.FromJson<FungusMusicSaveDataItem>(sdi.val);
            if (data == null)
            {
                Debug.LogError("Failed to decode Fungus Music save data item");
                return false;
            }

            var mm = FungusManager.Instance.MusicManager;

            mm.StopMusic();
            mm.StopAmbiance();

            mm.SetAudioPitch(data.music.pitch, 0, () => { });
            mm.SetAudioVolume(data.music.vol, 0, () => { });

            mm.PlayMusic(
                possibleMusicAudioClips.FirstOrDefault(x => x.name == data.music.clipName),
                data.music.loop,
                0,
                data.music.position);

            mm.PlayAmbianceSound(
                possibleAmbientAudioClips.FirstOrDefault(x => x.name == data.amb.clipName),
                data.amb.loop,
                data.amb.vol);

            return true;
        }

        public void PreDecode()
        {
        }

        public void PostDecode()
        {
        }
    }

    /// <summary>
    /// Serializable container for encoding the variables in the GlobalVariables.
    /// </summary>
    [System.Serializable]
    public class FungusMusicSaveDataItem
    {
        [System.Serializable]
        public struct MusicSourceSaveInfo
        {
            public float vol, pitch, position;
            public bool loop;
            public string clipName;
        }

        public MusicSourceSaveInfo music, amb;
    }
}
