// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using Fungus.Utils;

namespace Fungus
{
    public interface ITextTagParser
    {
        List<TextTagToken> Tokenize(string storyText);
    }
}