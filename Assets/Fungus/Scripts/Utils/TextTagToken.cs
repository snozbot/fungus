// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Supported token types for use in Say / Write text.
    /// </summary>
    public enum TokenType
    {
        /// <summary> Invalid token type. </summary>
        Invalid,
        /// <summary> A string of words. </summary>
        Words,
        /// <summary> b </summary>
        BoldStart,              // 
        /// <summary> /b </summary>
        BoldEnd,
        /// <summary> i </summary>
        ItalicStart,
        /// <summary> /i </summary>
        ItalicEnd,
        /// <summary> color=red </summary>
        ColorStart,
        /// <summary> /color </summary>
        ColorEnd,
        /// <summary> size=20 </summary>
        SizeStart,
        /// <summary> /size </summary>
        SizeEnd,
        /// <summary> w, w=0.5 </summary>
        Wait,
        /// <summary> wi </summary>
        WaitForInputNoClear,
        /// <summary> wc </summary>
        WaitForInputAndClear,
        /// <summary> wp, wp=0.5 </summary>
        WaitOnPunctuationStart,
        /// <summary> /wp </summary>
        WaitOnPunctuationEnd,
        /// <summary> c </summary>
        Clear,
        /// <summary> s, s=60 </summary>
        SpeedStart,
        /// <summary> /s </summary>
        SpeedEnd,
        /// <summary> x </summary>
        Exit,
        /// <summary> m=MessageName </summary>
        Message,
        /// <summary> vpunch=0.5 </summary>
        VerticalPunch,
        /// <summary> hpunch=0.5 </summary>
        HorizontalPunch,
        /// <summary> punch=0.5 </summary>
        Punch,
        /// <summary> flash=0.5 </summary>
        Flash,
        /// <summary> audio=Sound </summary>
        Audio,
        /// <summary> audioloop=Sound </summary>
        AudioLoop,
        /// <summary> audiopause=Sound </summary>
        AudioPause,
        /// <summary> audiostop=Sound </summary>
        AudioStop
    }

    /// <summary>
    /// Represents a token of story text. The text is broken into a list of tokens.
    /// </summary>
    public class TextTagToken
    {
        #region Public members

        /// <summary>
        /// The type of the token.
        /// </summary>
        public TokenType type = TokenType.Invalid;

        /// <summary>
        /// List of comma separated parameters.
        /// </summary>
        public List<string> paramList;

        #endregion
    }
}