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
        public int id { get; set; }
        public BlockType type { get; set; }

        public void Dispose()
        {
            var blockSystems = GetComponentsInChildren<IBlockSystem>(true);
            foreach (var blockSystem in blockSystems)
                blockSystem.Teardown();

            id = -1;
            Destroy(gameObject);
        }

        public void Spawn(int id, BlockType type, IHexagon hexagon)
        {
            this.id = id;
            this.type = type;

            BindHexagon(hexagon);
            hexagon.EVT_MovementPublish(id);
        }

        public void BindHexagon(IHexagon hexagon)
        {
            transform.SetParent(hexagon.transform);
        }

        public void BindHexagonNothing()
        {
            transform.SetParent(transform.root);
        }

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
    }
}