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
                    Gizmos.DrawSphere(transform.position.CalculateLocalPosition(position), radius);
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

        private void EVT_MatchSystem()
        {
            Match();
        }

        private void Match()
        {
            foreach (var block in blocks)
            {
                var hexagons = new List<IHexagon>();
                foreach (var position in block.positions)
                {
                    var ray = new Ray2D(transform.position.CalculateLocalPosition(position),
                        Vector2.zero);
                    var hit2Ds = Physics2D.RaycastAll(ray.origin, ray.direction);
                    foreach (var hit2D in hit2Ds)
                    {
                        if (hit2D.collider != null && hit2D.collider.CompareTag("Hexagon"))
                            hexagons.Add(hit2D.transform.GetComponent<IHexagon>());   
                    }
                }
                
                systems[block.type] = hexagons;
            }

            for (var i = 0; i < systems.Count; i++)
            {
                var match = systems[(PositionType)i];
                if (match.Count != 3)
                    continue;

                if (match[0].block?.type != block.type || match[1].block?.type != block.type || match[2].block?.type != block.type)
                    continue;
                
                var blocks = new List<IBlock> { match[0].block, match[1].block, match[2].block };
                if (blocks.Any(block => block.id < 0))
                    return;

                GameSystem.Default.MatchedBlock[block.type].AddRange(blocks);
                DeleteMatch(systems[(PositionType)i]);
            }
        }

        public void DeleteMatch(List<IHexagon> hexagons)
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

                foreach (var hexagon in hexagons)
                {
                    hexagon.block.BindHexagonNothing();
                    hexagon.BindBlock(null);
                }
            }
            
            GameEventSystem.EVT_DetectBlankSystemPublish();
        }
    }
}