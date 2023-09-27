using System;
using System.Collections.Generic;
using System.Linq;
using Anonymous.Game.Hexagon;
using UnityEditor;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public class BlockMatchSystem : MonoBehaviour, IBlockSystem
    {
        [Header("Matching Block Position Field")]
        [SerializeField] private List<BlockPositionModel> blocks;

        private readonly Dictionary<PositionType, List<IHexagon>> systemTypes = new();
        private IBlock block;
        
        private GameSystem system => GameSystem.Default;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var radius = 0.1f;
            if (Selection.activeGameObject == gameObject)
            {
                Gizmos.color = Color.black;

                foreach (var block in blocks)
                    Gizmos.DrawRay(transform.position.CalculateLocalPosition(block.positions[0]),
                        block.positions[1]);
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
            
            systemTypes.Clear();
        }

        private void EVT_MatchSystem(int id)
        {
            foreach (var type in Enum.GetValues(typeof(BlockType)))
                system.matchTypes[(BlockType)type].Clear();
            
            if (block.id == id)
                Match();
        }

        private void Match()
        {
            foreach (var block in blocks)
            {
                var hexagons = new List<IHexagon>();
                var ray = new Ray2D(transform.position.CalculateLocalPosition(block.positions[0]),
                    block.positions[1]);
                var hit2Ds = Physics2D.RaycastAll(ray.origin, ray.direction, 20);
                foreach (var hit2D in hit2Ds)
                    if (hit2D.collider != null && hit2D.collider.CompareTag("Hexagon"))
                        hexagons.Add(hit2D.transform.GetComponent<IHexagon>());

                systemTypes[block.type] = hexagons;
            }

            AddMatchBlocks();
        }

        private void AddMatchBlocks()
        {
            for (var i = 0; i < systemTypes.Count; i++)
            {
                var hexagons = systemTypes[(PositionType)i];
                if (hexagons.Count <= 2)
                    continue;

                var lists = new List<List<IBlock>>();
                var index = 0;
                foreach (var hexagon in hexagons)
                {
                    if (lists.Count >= index)
                        lists.Add(new List<IBlock>());
                    
                    if (hexagon.block?.type == block?.type)
                    {
                        lists[index].Add(hexagon.block);
                    }
                    else
                    {
                        if (lists[index].Count > 0)
                            index += 1;
                    }
                }
                
                if (lists.SelectMany(list => list).Any(block => block.id < 0))
                    return;

                foreach (var list in lists.Where(list => list.Count >= 3))
                {
                    var refine = list.GroupBy(i => i.id).Select(i => i.FirstOrDefault()).ToList();
                    if (refine.Count >= 3)
                        system.matchTypes[block.type].Add(refine);
                }
            }
            
            var count = system.matchTypes.Select((_, type) => system.matchTypes[(BlockType)type]).Sum(matches =>
                matches.Where(block => block.Count != 0).Sum(match => match.Count));
            if (count == 0)
            {
                system.isNotMatchedArray.Add(true);
            }
            else
            {
                system.isNotMatchedArray.Add(false);
                
                for (var i = 0; i < systemTypes.Count; i++)
                    DeleteMatchBlocks(systemTypes[(PositionType)i]);
            }
            
            GameEventSystem.EVT_DetectBlankSystemPublish();
        }

        public void DeleteMatchBlocks(List<IHexagon> hexagons)
        {
            for (var type = 0; type < system.matchTypes.Count; type++)
            {
                var blocks = system.matchTypes[(BlockType)type];
                foreach (var block in blocks.Where(block => block.Count != 0))
                {
                    GameEventSystem.EVT_MatchSuccessPublish(block.Count);
                    while (block.Count > 0)
                    {
                        foreach (var hexagon in hexagons.Where(hexagon => hexagon.block?.id == block[0]?.id))
                        {
                            hexagon.block.BindHexagonNothing();
                            hexagon.BindBlock(null);
                        }

                        if (block[0].id > 0)
                            block[0].Dispose();
                        block.RemoveAt(0);
                    }
                }
            }
        }
    }
}