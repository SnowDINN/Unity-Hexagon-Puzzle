using System.Collections.Generic;
using System.Linq;
using Anonymous.Game.Hexagon;
using UnityEditor;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public class BlockMatchSystem : MonoBehaviour, IBlockSystem
    {
        [Header("Matching Block Position Field")] [SerializeField]
        private List<BlockPositionModel> blocks;

        private readonly Dictionary<PositionType, List<IHexagon>> systems = new();
        private IBlock block;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var radius = 0.1f;
            if (Selection.activeGameObject == gameObject)
            {
                Gizmos.color = Color.black;

                foreach (var position in blocks.SelectMany(block => block.positions))
                    Gizmos.DrawSphere(GameSystem.Default.CalculateLocalPosition(transform.position, position), radius);
            }
        }
#endif

        public void Setup(IBlock block)
        {
            this.block = block;

            GameEventSystem.EVT_MatchSystem += EVT_MatchSystem;
        }

        public void Teardown()
        {
            GameEventSystem.EVT_MatchSystem -= EVT_MatchSystem;
        }

        private void EVT_MatchSystem(IHexagon hexagon)
        {
            Match();
        }

        private void Match()
        {
            foreach (var block in blocks)
            {
                var list = new List<IHexagon>();
                foreach (var position in block.positions)
                {
                    var ray = new Ray2D(GameSystem.Default.CalculateLocalPosition(transform.position, position),
                        Vector2.zero);
                    var hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null)
                        list.Add(hit.transform.GetComponent<IHexagon>());
                }

                systems[block.type] = list;
            }

            for (var i = 0; i < systems.Count; i++)
            {
                var match = systems[(PositionType)i];
                if (match.Count != 2)
                    continue;

                if (match[0].block?.type == block.type && match[1].block?.type == block.type)
                {
                    var blocks = new List<IBlock> { block, match[0].block, match[1].block };
                    foreach (var block in blocks)
                        if (block.id < 0)
                            return;

                    GameSystem.Default.MatchedBlock[block.type].AddRange(blocks);
                }
            }

            DeleteMatch();
        }
        
        public void DeleteMatch()
        {
            for (var type = 0; type < GameSystem.Default.MatchedBlock.Count; type++)
            {
                var blocks = GameSystem.Default.MatchedBlock[(BlockType)type];
                if (blocks.Count <= 2)
                    continue;

                Debug.Log($"Match Blocks : {blocks[0].type}, {blocks.Count}");
                while (blocks.Count > 0)
                {
                    if (blocks[0].id > 0)
                        blocks[0].Dispose();
                    blocks.RemoveAt(0);
                }
            }
        }
    }
}