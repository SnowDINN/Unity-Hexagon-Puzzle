using Anonymous.Game.Block;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    public interface IHexagon
    {
        IBlock block { get; set; }
        Transform transform { get; set; }
        bool hasBind { get; set; }
        void BindBlock(IBlock block);
        void SetCollider(bool enabled);
    }
}