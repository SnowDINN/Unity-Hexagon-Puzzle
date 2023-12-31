using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    public class HexagonBlankSystem : MonoBehaviour, IHexagonSystem
    {
        [SerializeField] private List<HexagonPositionModel> detectedHexagons;

        private readonly Dictionary<PositionType, IHexagon> systemTypes = new();
        private IHexagon hexagon;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var radius = 0.1f;
            if (Selection.activeGameObject == gameObject)
            {
                Gizmos.color = Color.black;

                foreach (var hexagon in detectedHexagons)
                    Gizmos.DrawSphere(transform.position.CalculateLocalPosition(hexagon.position),
                        radius);
            }
        }
#endif

        public void Setup(IHexagon hexagon)
        {
            this.hexagon = hexagon;

            GameEventSystem.EVT_DetectBlankSystem += EVT_DetectBlankSystem;

            foreach (var detectedHexagon in detectedHexagons)
            {
                var ray = new Ray2D(
                    transform.position.CalculateLocalPosition(detectedHexagon.position),
                    Vector2.zero);
                var hit2Ds = Physics2D.RaycastAll(ray.origin, ray.direction);
                foreach (var hit2D in hit2Ds)
                {
                    if (hit2D.collider != null && hit2D.collider.CompareTag("Hexagon"))
                        systemTypes[detectedHexagon.type] = hit2D.transform.GetComponent<IHexagon>();   
                }
            }
        }

        public void Teardown()
        {
            GameEventSystem.EVT_DetectBlankSystem -= EVT_DetectBlankSystem;
            
            systemTypes.Clear();   
        }

        private void EVT_DetectBlankSystem()
        {
            Movement();
        }

        private void Movement()
        {
            for (var i = 0; i < systemTypes.Count; i++)
            {
                var key = (PositionType)i;
                if (!systemTypes.ContainsKey(key))
                    continue;

                var nextHexagon = systemTypes[key];
                if (nextHexagon == null)
                    continue;

                if (nextHexagon.hasBind)
                    continue;

                if (hexagon.block?.id > 0)
                {
                    nextHexagon.EVT_MovementPublish(hexagon.block.id);
                    hexagon.BindBlock(null);
                }

                break;
            }
        }
    }
}