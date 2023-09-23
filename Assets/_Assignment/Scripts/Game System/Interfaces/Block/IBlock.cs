using Anonymous.Game.Hexagon;

namespace Anonymous.Game.Block
{
    public interface IBlock
    {
        int id { get; set; }
        void Spawn(int id, BlockType type, IHexagon hexagon);
        void SetHexagon(IHexagon hexagon);
    }
}