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
        int correctTagCount = 0;
        bool receivedInput = false;
        int glyphCount = 0;

        void OnEnable() 
        {
            WriterSignals.OnTextTagToken += OnTextTagToken;
            WriterSignals.OnWriterState += OnWriterState;
            WriterSignals.OnWriterInput += OnWriterInput;
            WriterSignals.OnWriterGlyph += OnWriterGlyph;
    	}
    	
        void OnDisable()
        {
            WriterSignals.OnTextTagToken -= OnTextTagToken;
            WriterSignals.OnWriterState -= OnWriterState;
            WriterSignals.OnWriterInput -= OnWriterInput;
            WriterSignals.OnWriterGlyph -= OnWriterGlyph;
        }

        void OnTextTagToken(Writer writer, TextTagToken token, int index, int maxIndex)
        {
            if (index == 0 && token.type == TokenType.BoldStart)
            {
                correctTagCount++;
            }
            else if (index == 1 && token.type == TokenType.Wait)
            {
                correctTagCount++;
            }
            else if (index == 2 && token.type == TokenType.Words)
            {
                correctTagCount++;
            }
            else if (index == 3 && token.type == TokenType.BoldEnd)
            {
                correctTagCount++;
            }
        }

        void OnWriterState(Writer writer, WriterState writerState)
        {
            if (writerState == WriterState.Start && correctTagCount != 0)
            {
                IntegrationTest.Fail();
            }
            if (writerState == WriterState.Pause && correctTagCount != 2)
            {
                IntegrationTest.Fail();
            }
            if (writerState == WriterState.Resume && correctTagCount != 2)
            {
                IntegrationTest.Fail();
            }
            else if (writerState == WriterState.End && correctTagCount != 4)
            {
                IntegrationTest.Fail();
            }

            if (writerState == WriterState.End)
            {
                if (!receivedInput)
                {
                    IntegrationTest.Fail();
                }

                if (glyphCount != 6)
                {
                    IntegrationTest.Fail();
                }

                IntegrationTest.Pass();
            }
        }

        void OnWriterInput(Writer writer)
        {
            receivedInput = true;
        }

        void OnWriterGlyph(Writer writer)
        {
            glyphCount++;
        }
    }
}