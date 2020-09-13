using UnityEngine;
using Fungus;

[CommandInfo("Narrative",
"Set Say End Delay",
"Sets the delay that applies after all Say commands finish executing, before the next block starts executing.")]
public class SetSayEndDelay : Command
{
    [SerializeField] FloatData newDelay;
    public override void OnEnter()
    {
        base.OnEnter();

        Say.endDelay = newDelay;

        Continue();
    }
}
