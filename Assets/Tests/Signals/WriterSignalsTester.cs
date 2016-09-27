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
        int step = 0;

        void OnEnable() 
        {
            WriterSignals.OnTextTagToken += OnTextTagToken;
    	}
    	
        void OnDisable()
        {
            WriterSignals.OnTextTagToken -= OnTextTagToken;
        }

        void OnTextTagToken(Writer writer, TextTagToken token)
        {
            if (step == 0 && token.type == TokenType.BoldStart)
            {
                step = 1;
            }
            else if (step == 1 && token.type == TokenType.Words)
            {
                step = 2;
            }
            else if (step == 2 && token.type == TokenType.BoldEnd)
            {
                IntegrationTest.Pass();
            }
        }
    }
}