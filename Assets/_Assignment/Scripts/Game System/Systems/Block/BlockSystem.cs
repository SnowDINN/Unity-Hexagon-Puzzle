using System;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game.Block
{
    [Serializable]
    public class BlockPositionModel
    {
        public PositionType type;
        public Vector2[] positions;
    }
    
    public class BlockSystem : MonoBehaviour, ISystem, IBlock
    {
        [SerializeField] private BlockType type;
        
        public int id { get; set; }
        
        public void Setup()
        {
            var blockSystems = GetComponentsInChildren<IBlockSystem>(true);
            foreach (var blockSystem in blockSystems)
                blockSystem.Setup(this);
        }

        public void Teardown()
        {
            var blockSystems = GetComponentsInChildren<IBlockSystem>(true);
            foreach (var blockSystem in blockSystems)
                blockSystem.Teardown();
        }
        
        public void Spawn(int id, BlockType type, IHexagon hexagon)
        {
            this.id = id;
            this.type = type;

            SetHexagon(hexagon);
            hexagon.EVT_MovementPublish(id);
        }

        public void SetHexagon(IHexagon hexagon)
        {
            transform.SetParent(hexagon.GetTransform());
        }
    }
}