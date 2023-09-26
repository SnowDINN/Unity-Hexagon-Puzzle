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
        private static readonly Dictionary<BlockType, List<List<IBlock>>> matchTypes = new();

        [Header("Matching Block Position Field")] [SerializeField]
        private List<BlockPositionModel> blocks;

        private readonly Dictionary<PositionType, List<IHexagon>> systemTypes = new();
        private IBlock block;

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
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnLoadDictionary()
        {
            foreach (var type in Enum.GetValues(typeof(BlockType)))
                matchTypes.Add((BlockType)type, new List<List<IBlock>>());
        }

        private void EVT_MatchSystem(int id)
        {
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
                if (hexagons.Count < 3)
                    continue;

                var blocks = new List<List<IBlock>>();
                var block = new List<IBlock>();

                var index = 0;
                foreach (var hexagon in hexagons)
                    if (hexagon.block?.type == this.block?.type)
                    {
                        index += 1;
                        block.Add(hexagon.block);
                    }
                    else
                    {
                        if (index >= 3)
                        {
                            blocks.Add(new List<IBlock>(block));
                            block.Clear();

                            index = 0;
                        }
                        else
                        {
                            block.Clear();
                            index = 0;
                        }
                    }

                if (block.Count >= 3)
                {
                    blocks.Add(new List<IBlock>(block));
                    block.Clear();
                }
                
                if (blocks.SelectMany(block => block).Any(item => item.id < 0))
                    return;

                if (blocks.Count > 0)
                {
                    foreach (var list in blocks)
                        matchTypes[this.block.type].Add(list);
                }
            }
            
            var count = matchTypes.Select((_, type) => matchTypes[(BlockType)type]).Sum(matches =>
                matches.Where(block => block.Count != 0).Sum(match => match.Count));
            if (count == 0)
            {

            }
            else
            {
                for (var i = 0; i < systemTypes.Count; i++)
                    DeleteMatchBlocks(systemTypes[(PositionType)i]);
            }
            
            foreach (var type in Enum.GetValues(typeof(BlockType)))
                matchTypes[(BlockType)type].Clear();
        }

        public void DeleteMatchBlocks(List<IHexagon> hexagons)
        {
            for (var type = 0; type < matchTypes.Count; type++)
            {
                var blocks = matchTypes[(BlockType)type];
                foreach (var block in blocks.Where(block => block.Count != 0))
                {
                    Debug.Log($"Match Blocks : {block[0].type}, {block.Count}");
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

            GameEventSystem.EVT_DetectBlankSystemPublish();
        }
    }
}