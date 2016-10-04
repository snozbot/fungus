// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Writer event signalling system.
    /// You can use this to be notified about various events in the writing process.
    /// </summary>
    public static class WriterSignals
    {
        #region Public members

        /// <summary>
        /// TextTagToken signal. Sent for each unique token when writing text.
        /// </summary>
        public static event TextTagTokenHandler OnTextTagToken;
        public delegate void TextTagTokenHandler(Writer writer, TextTagToken token, int index, int maxIndex);
        public static void DoTextTagToken(Writer writer, TextTagToken token, int index, int maxIndex) { if(OnTextTagToken != null) OnTextTagToken(writer, token, index, maxIndex); }

        /// <summary>
        /// WriterState signal. Sent when the writer changes state.
        /// </summary>
        public static event WriterStateHandler OnWriterState;
        public delegate void WriterStateHandler(Writer writer, WriterState writerState);
        public static void DoWriterState(Writer writer, WriterState writerState) { if (OnWriterState != null) OnWriterState(writer, writerState); }

        /// <summary>
        /// WriterInput signal. Sent when the writer receives player input.
        /// </summary>
        public static event WriterInputHandler OnWriterInput;
        public delegate void WriterInputHandler(Writer writer);
        public static void DoWriterInput(Writer writer) { if (OnWriterInput != null) OnWriterInput(writer); }

        /// <summary>
        /// WriterGlyph signal. Sent when the writer writes out a glyph.
        /// </summary>
        public delegate void WriterGlyphHandler(Writer writer);
        public static event WriterGlyphHandler OnWriterGlyph;
        public static void DoWriterGlyph(Writer writer) { if (OnWriterGlyph != null) OnWriterGlyph(writer); }

        #endregion
    }
}
