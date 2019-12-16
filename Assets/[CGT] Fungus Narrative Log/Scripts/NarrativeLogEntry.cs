using UnityEngine;

namespace CGTUnity.Fungus.NarrativeLogSystem
{
	/// <summary>
	/// As the class name suggests, it holds data for a single entry within the NarrativeLog. If you
	/// want the entry displays to display more than what the default has to offer, subclassing this is
	/// a good idea.
	/// </summary>
    [System.Serializable]
    public class Entry
    {
        [SerializeField] string storyText;
        [SerializeField] string nameText;
        public virtual string StoryText             { get { return storyText; } set { storyText = value; } }
        public virtual string NameText              { get { return nameText; } set { nameText = value; } }

        public Entry()                              {}
        public Entry(string text, string name)
        {
            this.storyText =                        text;
            this.nameText =                         name;
        }
    }
}