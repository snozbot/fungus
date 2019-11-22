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