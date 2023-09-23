namespace Anonymous.Game.Block
{
    public interface IBlockSystem
    {
        void Setup(IBlock block);
        void Teardown();
    }
}