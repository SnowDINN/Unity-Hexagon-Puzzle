using System.Collections;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public class BlockMovementSystem : MonoBehaviour, IBlockSystem
    {
        [Header("Movement Animation Field")] [SerializeField]
        private float animationSpeed;

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
            {
                StopCoroutine(movementBlock);

                if (GameSystem.Default.isMovementArray.Contains(block.id))
                    GameSystem.Default.isMovementArray.Remove(block.id);
            }

            movementBlock = StartCoroutine(co_movementBlock(block, hexagon));
        }

        private IEnumerator co_movementBlock(IBlock block, IHexagon hexagon)
        {
            var time = 0f;


            GameSystem.Default.isMovementArray.Add(block.id);
            while (Vector2.Distance(transform.localPosition, Vector2.zero) > 0)
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero,
                    animationSpeed * Time.deltaTime);
                yield return null;
            }

            hexagon.BindBlock(block);

            if (!GameSystem.Default.isMovementArray.Remove(block.id))
                yield break;

            GameEventSystem.EVT_DetectBlankSystemPublish();
            GameEventSystem.EVT_MatchPublish(block.id);
        }
    }
}