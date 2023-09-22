using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public interface IBlock
    {
        void Spawn(IHexagon hexagon);
        void Move(IHexagon hexagon);
    }
}