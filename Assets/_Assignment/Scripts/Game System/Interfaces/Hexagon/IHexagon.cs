using Anonymous.Game.Block;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    public interface IHexagon
    {
        IBlock block { get; set; }
        bool HasBlock();
        void SetBlock(IBlock block);
        Transform GetTransform();
    }
}