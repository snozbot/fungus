using UnityEngine;

namespace Fungus
{
    public enum ScaleType
    {
        Increase,
        Decrease
    }
    /// <summary>
    /// Scales the current GameObject with magnitude for duration specified
    /// </summary>
    [CommandInfo("Scripting", "ScaleObject", "Scale Object with magnitude x for the duration specified.")]
    public class ScaleObject : Command
    {

        [SerializeField]
        protected ScaleType Scale;

        [SerializeField]
        protected float Magnitude;

        [SerializeField]
        protected float Duration;

        private bool Running = false;
        private bool Infinite = false;

        public override void OnEnter()
        {
            if (Magnitude == 0)
            {
                return;
            }

            Running = true;

            if (Duration == 0)
            {
                Infinite = true;
            }
        }

        public override string GetSummary()
        {
            if (Magnitude == 0)
            {
                return "Error: No magnitude is selected.";
            }

            if (Duration == 0)
            {
                return "GameObject will scale at a magnitude of " + Magnitude + " infinitely.";
            }
            else
            {
                return "GameObject will scale at a magnitude of " + Magnitude + " for a duration of " + Duration + " seconds.";
            }
        }

        void FixedUpdate()
        {
            if (Running)
            {
                if (Infinite)
                {
                    if (Scale == ScaleType.Increase)
                    {
                        this.gameObject.transform.localScale *= 1 + Magnitude * Time.deltaTime;
                    }
                    else
                    {
                        this.gameObject.transform.localScale /= 1 + Magnitude * Time.deltaTime;
                    }
                    Continue();
                }
                else
                {
                    if (Duration > 0)
                    {
                        if (Scale == ScaleType.Increase)
                        {
                            this.gameObject.transform.localScale *= 1 + Magnitude * Time.deltaTime;
                        }
                        else
                        {
                            this.gameObject.transform.localScale /= 1 + Magnitude * Time.deltaTime;
                        }
                        Duration -= Time.deltaTime;
                    }
                    else
                    {
                        Continue();
                    }
                }
            }
        }
    }
}