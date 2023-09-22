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
        public PositionType type;
        public Vector2 position;
    }
    
    public class HexagonSystem : MonoBehaviour, ISystem, IHexagon
    {
        [SerializeField] private List<HexagonPositionModel> hexagons;
        
        private readonly Dictionary<PositionType, IHexagon> systems = new();
        private IBlock block;

        public void Setup()
        {
            GameSystem.Default.EVT_BlockSpawn += EVT_BlockSpawn;
            GameSystem.Default.EVT_BlockResolve += EVT_BlockResolve;
            
            foreach (var hexagon in hexagons)
            {
                var ray = new Ray2D(GameSystem.Default.CalculateLocalPosition(transform.position, hexagon.position), Vector2.zero);
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
            return transform.childCount > 0;
        }

        public void SetBlock(IBlock block)
        {
            this.block = block;
            Movement();
        }

        public Transform GetTransform()
        {
            return transform;
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
            for (var i = 0; i < systems.Count; i++)
            {
                if (block == null)
                    continue;

                var key = (PositionType)i;
                if (!systems.ContainsKey(key))
                    continue;
                
                var hexagon = systems[key];
                if (hexagon == null)
                    continue;
                    
                if (hexagon.HasBlock())
                    continue;
                        
                block.Move(hexagon);
                block = null;
                break;
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var radius = 0.1f;
            if (Selection.activeGameObject == gameObject)
            {
                Gizmos.color = Color.black;

                foreach (var hexagon in hexagons)
                    Gizmos.DrawSphere(GameSystem.Default.CalculateLocalPosition(transform.position, hexagon.position), radius);
            }
        }
#endif
    }
}