using System;
using Anonymous.Game.Block;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    [Serializable]
    public class HexagonPositionModel
    {
        public PositionType type;
        public Vector2 position;
    }
    
    public class HexagonSystem : MonoBehaviour, ISystem, IHexagon
    {
        public IBlock block { get; set; }

        public void Setup()
        {
            var hexagonSystems = GetComponentsInChildren<IHexagonSystem>(true);
            foreach (var hexagonSystem in hexagonSystems)
                hexagonSystem.Setup(this);
        }

        public void Teardown()
        {
            var hexagonSystems = GetComponentsInChildren<IHexagonSystem>(true);
            foreach (var hexagonSystem in hexagonSystems)
                hexagonSystem.Teardown();
        }

        public bool HasBlock()
        {
            return transform.childCount > 0;
        }

        public void SetBlock(IBlock block)
        {
            this.block = block;
            GameSystem.Default.EVT_HexagonDetectSystemPublish();
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}