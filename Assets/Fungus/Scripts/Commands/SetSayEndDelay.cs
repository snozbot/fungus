using UnityEngine;
using Fungus;

[CommandInfo("Narrative",
"Set Say End Delay",
"Each Say command has a hidden delay at the end of it (0 by default). This sets its duration.")]
public class SetSayEndDelay : Command
{
    [Tooltip("Negative values are treated as 0.")]
    [SerializeField] FloatData newDelay;
    public override void OnEnter()
    {
        base.OnEnter();

        Say.EndDelay = newDelay;
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(184, 210, 235, 255);
    }
}
