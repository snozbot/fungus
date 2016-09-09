using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Writes text using a typewriter effect to a UI text object.
    /// </summary>
    public interface IWriter
    {
        /// <summary>
        /// This property is true when the writer is writing text or waiting (i.e. still processing tokens).
        /// </summary>
        bool IsWriting { get; }

        /// <summary>
        /// This property is true when the writer is waiting for user input to continue.
        /// </summary>
        bool IsWaitingForInput { get; }

        /// <summary>
        /// Stop writing text.
        /// </summary>
        void Stop();

        /// <summary>
        /// Writes text using a typewriter effect to a UI text object.
        /// </summary>
        /// <param name="content">Text to be written</param>
        /// <param name="clear">If true clears the previous text.</param>
        /// <param name="waitForInput">Writes the text and then waits for player input before calling onComplete.</param>
        /// <param name="stopAudio">Stops any currently playing audioclip.</param>
        /// <param name="audioClip">Audio clip to play when text starts writing.</param>
        /// <param name="onComplete">Callback to call when writing is finished.</param>
        IEnumerator Write(string content, bool clear, bool waitForInput, bool stopAudio, AudioClip audioClip, System.Action onComplete);

        /// <summary>
        /// Sets the color property of the text UI object.
        /// </summary>
        void SetTextColor(Color textColor);

        /// <summary>
        /// Sets the alpha component of the color property of the text UI object.
        /// </summary>
        void SetTextAlpha(float textAlpha);
    }

    /// <summary>
    /// Implement this interface to be notified about Writer events.
    /// </summary>
    public interface IWriterListener
    {
        ///
        /// Called when a user input event (e.g. a click) has been handled by the Writer.
        ///
        void OnInput();

        /// Called when the Writer starts writing new text
        /// <param name="audioClip">An optional audioClip sound effect can be supplied (e.g. for voiceover)</param>
        void OnStart(AudioClip audioClip);

        /// Called when the Writer has paused writing text (e.g. on a {wi} tag).
        void OnPause();

        /// Called when the Writer has resumed writing text.
        void OnResume();

        /// Called when the Writer has finshed writing text.
        /// <param name="stopAudio">Controls whether audio should be stopped when writing ends.</param>
        void OnEnd(bool stopAudio);

        /// Called every time the Writer writes a new character glyph.
        void OnGlyph();
    }
}