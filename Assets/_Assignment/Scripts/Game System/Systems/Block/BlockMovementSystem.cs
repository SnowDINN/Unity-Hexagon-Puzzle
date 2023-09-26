using System.Collections;
using Anonymous.Game.Hexagon;
using UnityEngine;
using UnityEngine.Serialization;

namespace Anonymous.Game.Block
{
    public class BlockMovementSystem : MonoBehaviour, IBlockSystem
    {
        [FormerlySerializedAs("animationMaxSpeed")]
        [Header("Movement Animation Field")]
        [SerializeField] private float animationSpeed;
        private IBlock block;

        private Coroutine movementBlock;

        public void Setup(IBlock block)
        {
            this.block = block;

            GameEventSystem.EVT_MovementSystem += EVT_MovementSystem;
        }

        public void Teardown()
        {
            GameEventSystem.EVT_MovementSystem -= EVT_MovementSystem;
        }

        private void EVT_MovementSystem(IHexagon hexagon, int id)
        {
            if (block.id == id)
                Movement(block, hexagon);
        }

        private void Movement(IBlock block, IHexagon hexagon)
        {
            block.BindHexagon(hexagon);
            
            if (movementBlock != null)
                StopCoroutine(movementBlock);
            movementBlock = StartCoroutine(co_movementBlock(block, hexagon));
        }

        private IEnumerator co_movementBlock(IBlock block, IHexagon hexagon)
        {
            var time = 0f;

            hexagon.SetCollider(false);
            while (Vector2.Distance(transform.localPosition, Vector2.zero) > 0)
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, animationSpeed * Time.deltaTime);
                yield return null;
            }
            hexagon.BindBlock(block);
            hexagon.SetCollider(true);
            
            GameEventSystem.EVT_DetectBlankSystemPublish();
            GameEventSystem.EVT_MatchPublish(block.id);
        }
    }
}