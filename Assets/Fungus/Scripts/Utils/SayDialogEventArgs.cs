using UnityEngine.UI;
using UnityEngine;
using System.Text;

namespace Fungus
{
    public class SayDialogEventArgs
    {
        public SayDialog SayDialog { get; set; }
        public Character Character { get { return SayDialog.SpeakingCharacter; } }
        public Image CharacterImage { get { return SayDialog.CharacterImage; } }
        public Sprite ImageSprite { get { return CharacterImage.sprite; } }
        public Writer Writer { get; set; }

        public WriterAudio WriterAudio { get { return Writer.AttachedWriterAudio; } }

        public string TextToWrite { get; set; }
        public bool ClearPrevious { get; set; }
        public bool WaitForInput { get; set; }
        public bool FadeWhenDone { get; set; }
        public bool StopVoiceover { get; set; }
        public bool WaitForVO { get; set; }
        public AudioClip VoiceOverClip { get; set; }
        public System.Action OnComplete { get; set; }

        public override string ToString()
        {
            forToString.Clear();
            StringBuilder result = forToString;
            result.AppendLine("Say Dialog: " + SayDialog.name);
            result.AppendLine("Character: " + Character.name);
            result.AppendLine("Character image: " + CharacterImage.name);
            result.AppendLine("Image Sprite: " + ImageSprite.name);
            result.AppendLine("Text to Write: " + TextToWrite);
            result.AppendLine("Voiceover Clip: " + VoiceOverClip?.name);

            return result.ToString();
        }

        protected StringBuilder forToString = new StringBuilder();

    }
}