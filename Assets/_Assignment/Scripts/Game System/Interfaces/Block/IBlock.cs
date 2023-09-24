using Anonymous.Game.Hexagon;

namespace Anonymous.Game.Block
{
    public interface IBlock
    {
        int id { get; set; }
        BlockType type { get; set; }
        void Spawn(int id, BlockType type, IHexagon hexagon);
        void BindHexagon(IHexagon hexagon);
        void BindHexagonNothing();
        void Dispose();
    }
}