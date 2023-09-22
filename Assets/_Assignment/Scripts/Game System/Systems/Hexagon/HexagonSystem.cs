using System;
using System.Collections.Generic;
using Anonymous.Game.Block;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonymous.Game.Hexagon
{
    [Serializable]
    public class HexagonPositionModel
    {
        public HexagonPositionType type;
        public Vector2 position;
    }
    
    public class HexagonSystem : MonoBehaviour, ISystem, IHexagon
    {
        [SerializeField] private List<HexagonPositionModel> hexagons;

        private readonly Dictionary<HexagonPositionType, IHexagon> systems = new();
        public IBlock Block;

        public void Setup()
        {
            GameSystem.Default.EVT_BlockSpawn += EVT_BlockSpawn;
            GameSystem.Default.EVT_BlockResolve += EVT_BlockResolve;
            
            foreach (var hexagon in hexagons)
            {
                var ray = new Ray2D(calculateLocalPosition(hexagon.position), Vector2.zero);
                var hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                    systems.Add(hexagon.type, hit.transform.GetComponent<IHexagon>());
            }
        }

        public void Teardown()
        {
            GameSystem.Default.EVT_BlockSpawn -= EVT_BlockSpawn;
            GameSystem.Default.EVT_BlockResolve -= EVT_BlockResolve;
        }
        
        public bool HasBlock()
        {
            return Block != null;
        }

        public void SetBlock(IBlock block)
        {
            Block = block;
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        private void EVT_BlockSpawn()
        {
            Movement();
        }
        
        private void EVT_BlockResolve()
        {
            Movement();
        }

        private void Movement()
        {
            foreach (var system in systems)
            {
                if (Block == null)
                    return;
                
                var hexagon = system.Value;
                if (hexagon == null)
                    return;
                
                if (!hexagon.HasBlock())
                {
                    hexagon.SetBlock(Block);
                    
                    Block.Move(hexagon);
                    Block = null;
                    break;
                }
            }
        }

        private Vector2 calculateLocalPosition(Vector2 position)
        {
            return transform.position + new Vector3(position.x, position.y, 0);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var radius = 0.1f;

            if (Selection.activeGameObject == gameObject)
            {
                Gizmos.color = Color.black;

                foreach (var hexagon in hexagons)
                    Gizmos.DrawSphere(calculateLocalPosition(hexagon.position), radius);
            }
        }
#endif
    }
}