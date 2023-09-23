using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    public class HexagonBlankSystem : MonoBehaviour, IHexagonSystem
    {
        [SerializeField] private List<HexagonPositionModel> detectedHexagons;
        
        private readonly Dictionary<PositionType, IHexagon> systems = new();
        private IHexagon hexagon;
        
        public void Setup(IHexagon hexagon)
        {
            this.hexagon = hexagon;
            
            GameSystem.Default.EVT_HexagonDetectSystem += EVT_HexagonDetectSystem;

            foreach (var detectedHexagon in detectedHexagons)
            {
                var ray = new Ray2D(GameSystem.Default.CalculateLocalPosition(transform.position, detectedHexagon.position), Vector2.zero);
                var hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                    systems.Add(detectedHexagon.type, hit.transform.GetComponent<IHexagon>());
            }
        }

        public void Teardown()
        {
            GameSystem.Default.EVT_HexagonDetectSystem -= EVT_HexagonDetectSystem;
        }
        
        private void EVT_HexagonDetectSystem()
        {
            Movement();
        }
        
        private void Movement()
        {
            for (var i = 0; i < systems.Count; i++)
            {
                var key = (PositionType)i;
                if (!systems.ContainsKey(key))
                    continue;
                
                var nextHexagon = systems[key];
                if (nextHexagon == null)
                    continue;
                    
                if (nextHexagon.HasBlock())
                    continue;

                if (hexagon.block != null)
                {
                    GameSystem.Default.EVT_BlockMovementPublish(hexagon.block.id, nextHexagon);
                    hexagon.block = null;
                }
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

                foreach (var hexagon in detectedHexagons)
                    Gizmos.DrawSphere(GameSystem.Default.CalculateLocalPosition(transform.position, hexagon.position), radius);
            }
        }
#endif
    }   
}