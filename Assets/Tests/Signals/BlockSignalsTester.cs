// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Checks if Block signals are being sent correctly.
    /// </summary>
    [AddComponentMenu("")]
    public class BlockSignalsTester : MonoBehaviour 
    {
        bool started = false;
        bool commandExecuted = false;

        void OnEnable() 
        {
            BlockSignals.OnBlockStart += OnBlockStart;
            BlockSignals.OnBlockEnd += OnBlockEnd;
            BlockSignals.OnCommandExecute += OnCommandExecute;
        }

        void OnDisable()
        {
            BlockSignals.OnBlockStart -= OnBlockStart;
            BlockSignals.OnBlockEnd -= OnBlockEnd;
            BlockSignals.OnCommandExecute -= OnCommandExecute;
        }

        void OnBlockStart(Block block)
        {
            IntegrationTest.Assert(block.BlockName == "Start");

            started = true;
        }

        void OnBlockEnd(Block block)
        {
            IntegrationTest.Assert(started);
            IntegrationTest.Assert(commandExecuted);

            IntegrationTest.Pass();
        }

        void OnCommandExecute(Block block, Command command, int commandIndex, int maxCommandIndex)
        {
            IntegrationTest.Assert(commandIndex == 0);
            IntegrationTest.Assert(maxCommandIndex == 1);
            IntegrationTest.Assert(command.GetType() == typeof(Wait));
            commandExecuted = true;
        }
    }
}