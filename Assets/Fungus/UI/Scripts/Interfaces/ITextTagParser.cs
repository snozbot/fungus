using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    public interface ITextTagParser
    {
        List<TextTagToken> Tokenize(string storyText);
    }
}