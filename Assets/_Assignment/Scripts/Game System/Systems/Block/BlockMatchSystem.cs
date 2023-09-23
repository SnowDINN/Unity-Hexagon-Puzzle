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

        private readonly Dictionary<PositionType, List<IBlock>> systems = new();
        private IBlock block;
        
        public void Setup(IBlock block)
        {
            this.block = block;

            GameEventSystem.EVT_MatchSystem += EvtMatchSystem;
        }

        public void Teardown()
        {
            GameEventSystem.EVT_MatchSystem -= EvtMatchSystem;
        }

        private void EvtMatchSystem(IHexagon hexagon)
        {
            // Match();
        }

        private void Match()
        {
            foreach (var block in blocks)
            {
                var list = new List<IBlock>();
                foreach (var position in block.positions)
                {
                    var ray = new Ray2D(GameSystem.Default.CalculateLocalPosition(transform.position, position),
                        Vector2.zero);
                    var hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null)
                        list.Add(hit.transform.GetComponent<IBlock>());
                }

                systems[block.type] = list;
            }

            foreach (var system in systems)
            {
                var match = system.Value;
            }
        }
        
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
    }
}