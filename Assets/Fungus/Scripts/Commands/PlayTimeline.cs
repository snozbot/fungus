// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Playables;

namespace Fungus
{
    /// <summary>
    /// Plays a state of an animator according to the state name.
    /// </summary>
    [CommandInfo("Timeline", 
                 "Play Timeline", 
                 "Plays playable director from the specified timeline game object")]
    [AddComponentMenu("")]
    public class PlayTimeline : Command 
    {
        [Tooltip("Reference to a timeline game object")]
        [SerializeField] protected PlayableDirector timeline = new PlayableDirector();

        [Tooltip("Wait until timeline has stopped.")]
        [SerializeField] protected bool waitUntilStopped = true;

        public override void OnEnter()
        {   
            if (timeline != null)
            {
                timeline.Play();

                if (waitUntilStopped)
                {
                    timeline.stopped += delegate {
                        Continue();
                    };
                }
                else
                {
                    Continue();
                }
            }
            else
            {
                Continue();
            }
        }

        public override Color GetButtonColor()
        {
            return new Color32(170, 204, 169, 255);
        }
        
        public override string GetSummary()
        {
            if (timeline == null)
            {
                return "Error: No timeline selected";
            }

            return timeline.name;
        }
    }
}