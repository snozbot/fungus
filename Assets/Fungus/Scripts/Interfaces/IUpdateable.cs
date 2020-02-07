// This code is part of the Fungus library (http://fungusgames.com)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Interface for Flowchart components which can be updated when the
    /// scene loads in the editor. This is used to maintain backwards
    /// compatibility with earlier versions of Fungus.
    /// </summary>
    public interface IUpdateable
    {
        void UpdateToVersion(int oldVersion, int newVersion);
    }
}