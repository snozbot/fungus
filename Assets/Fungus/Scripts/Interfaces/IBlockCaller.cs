// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Interface for indicating that the class holds a reference to and may call a block
    /// </summary>
    public interface IBlockCaller : IStringLocationIdentifier
    {
        bool MayCallBlock(Block block);
    }
}