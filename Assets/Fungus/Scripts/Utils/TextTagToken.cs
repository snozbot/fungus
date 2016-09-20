// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;

namespace Fungus.Utils
{
    public class TextTagToken
    {
        public enum TokenType
        {
            Invalid,
            Words,                  // A string of words
            BoldStart,              // b
            BoldEnd,                // /b
            ItalicStart,            // i
            ItalicEnd,              // /i
            ColorStart,             // color=red
            ColorEnd,               // /color
            SizeStart,              // size=20
            SizeEnd,                // /size
            Wait,                   // w, w=0.5
            WaitForInputNoClear,    // wi
            WaitForInputAndClear,   // wc
            WaitOnPunctuationStart, // wp, wp=0.5
            WaitOnPunctuationEnd,   // /wp
            Clear,                  // c
            SpeedStart,             // s, s=60
            SpeedEnd,               // /s
            Exit,                   // x
            Message,                // m=MessageName
            VerticalPunch,          // {vpunch=0.5}
            HorizontalPunch,        // {hpunch=0.5}
            Punch,                  // {punch=0.5}
            Flash,                  // {flash=0.5}
            Audio,                  // {audio=Sound}
            AudioLoop,              // {audioloop=Sound}
            AudioPause,             // {audiopause=Sound}
            AudioStop               // {audiostop=Sound}
        }

        public TokenType type = TokenType.Invalid;
        public List<string> paramList;
    }
}