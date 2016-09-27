using UnityEngine;
using System.Collections;
using Fungus.Utils;

namespace Fungus
{
    /// <summary>
    /// Writer event signalling system.
    /// </summary>
    public static class WriterSignals
    {
        #region Public members

        /// <summary>
        /// TextTagToken signal. Sent for each unique token when writing text.
        /// </summary>
        public delegate void TextTagTokenHandler(Writer writer, TextTagToken token, int index, int maxIndex);
        public static event TextTagTokenHandler OnTextTagToken;
        public static void DoTextTagToken(Writer writer, TextTagToken token, int index, int maxIndex) { if(OnTextTagToken != null) OnTextTagToken(writer, token, index, maxIndex); }

        /// <summary>
        /// WriterState signal. Sent when the writer changes state.
        /// </summary>
        public delegate void WriterStateHandler(Writer writer, WriterState writerState);
        public static event WriterStateHandler OnWriterState;
        public static void DoWriterState(Writer writer, WriterState writerState) { if (OnWriterState != null) OnWriterState(writer, writerState); }

        #endregion
    }
}
