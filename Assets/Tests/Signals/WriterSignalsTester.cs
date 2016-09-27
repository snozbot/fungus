using UnityEngine;
using Fungus.Utils;

namespace Fungus
{
    /// <summary>
    /// Checks if Writer signals are being sent correctly.
    /// </summary>
    [AddComponentMenu("")]
    public class WriterSignalsTester : MonoBehaviour 
    {
        void OnEnable() 
        {
            WriterSignals.OnTextTagToken += OnTextTagToken;
    	}
    	
        void OnDisable()
        {
            WriterSignals.OnTextTagToken -= OnTextTagToken;
        }

        void OnTextTagToken(Writer writer, TextTagToken token, int index, int maxIndex)
        {
            if (index == 0 && token.type != TokenType.BoldStart)
            {
                IntegrationTest.Fail();
            }
            else if (index == 1 && token.type != TokenType.Words)
            {
                IntegrationTest.Fail();
            }
            else if (index == 2 && token.type == TokenType.BoldEnd)
            {
                IntegrationTest.Pass();
            }
        }
    }
}