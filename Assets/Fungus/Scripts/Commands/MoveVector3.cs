using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting", "MoveWithVector3", "Moves the current object for a specified amount of time with the specified Vector3 direction")]
public class MoveVector3 : Command {

    [SerializeField]
    protected Vector3 Direction;

    [SerializeField]
    protected float Duration;

    private bool Running = false;

    private bool Infinite = false;

    public override void OnEnter()
    {
        if (Direction == null)
        {
            return;
        }

        Running = true;

        if (Duration == 0)
        {
            Infinite = true;
        }
    }

    void FixedUpdate()
    {
        if (Running)
        {
            if (Infinite)
            {
                this.gameObject.transform.position += Direction * Time.deltaTime;
                Continue();
            }
            else
            {
                if (Duration > 0)
                {
                    this.gameObject.transform.position += Direction * Time.deltaTime;
                    Duration -= Time.deltaTime;
                }
                else
                {
                    Continue();
                }
            }
        }
    }

    public override string GetSummary()
    {
        if (Direction == Vector3.zero)
        {
            return "Error: Command requires a direction for GameObject to travel";
        }

        return "Move current GameObject in direction of " + Direction + " for " + Duration + " seconds";
    }

}
