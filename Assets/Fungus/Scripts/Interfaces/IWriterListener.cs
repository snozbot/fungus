// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Implement this interface to be notified about Writer events.
    /// </summary>
    public interface IWriterListener
    {
        ///
        /// Called when a user input event (e.g. a click) has been handled by the Writer.
        ///
        void OnInput();

        /// <summary>
        /// Called when the Writer starts writing new text.
        /// </summary>
        /// <param name="audioClip">An optional audioClip sound effect can be supplied (e.g. for voiceover)</param>
        void OnStart(AudioClip audioClip);

        /// Called when the Writer has paused writing text (e.g. on a {wi} tag).
        void OnPause();

        /// Called when the Writer has resumed writing text.
        void OnResume();

        /// Called when the Writer has finished.
        /// <param name="stopAudio">Controls whether audio should be stopped when writing ends.</param>
        void OnEnd(bool stopAudio);

        /// <summary>
        /// Called when the Writer has no more Words remaining, but may have waits or other tokens still pending.
        /// Will not be called if there is NO Words for the writer to process in the first place. e.g. Audio only says
        /// do not trigger this.
        /// 
        /// Note that the writer does not know what may happen after it's job is done. If a following Say does
        /// not clear the existing, you'll get what looks like AllWordsWritten and then more words written.
        /// </summary>
        void OnAllWordsWritten();

        /// Called every time the Writer writes a new character glyph.
        void OnGlyph();

        /// <summary>
        /// Called when voiceover should start.
        /// </summary>
        void OnVoiceover(AudioClip voiceoverClip);
    }
}