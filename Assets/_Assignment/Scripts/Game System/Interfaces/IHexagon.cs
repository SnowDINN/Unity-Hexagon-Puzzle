using Anonymous.Game.Block;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    public interface IHexagon
    {
        bool HasBlock();
        void SetBlock(IBlock block);
        Vector2 GetPosition();
    }
}