using System;
using Anonymous.Game.Block;
using UnityEngine;

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
        public IBlock block { get; set; }
        
        private Collider2D collider;

        public Transform transform
        {
            get => gameObject.transform;
            set { }
        }

        public bool hasBind
        {
            get => transform.childCount > 0;
            set { }
        }

        public void BindBlock(IBlock block)
        {
            this.block = block;
        }

        public void SetCollider(bool enabled)
        {
            if (collider != null)
                collider.enabled = enabled;
        }

        public void Setup()
        {
            var hexagonSystems = GetComponentsInChildren<IHexagonSystem>(true);
            foreach (var hexagonSystem in hexagonSystems)
                hexagonSystem.Setup(this);
            
            collider = GetComponent<Collider2D>();
        }

        public void Teardown()
        {
            var hexagonSystems = GetComponentsInChildren<IHexagonSystem>(true);
            foreach (var hexagonSystem in hexagonSystems)
                hexagonSystem.Teardown();
        }
    }
}