using System.Collections;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public class BlockMovementSystem : MonoBehaviour, IBlockSystem
    {
        [Header("Movement Animation Field")]
        [SerializeField] private float animationMaxSpeed;
        [SerializeField] private float animationVelocitySpeed;
        
        private Coroutine move_Coroutine;
        private IBlock block;
        
        public void Setup(IBlock block)
        {
            this.block = block;

            GameSystem.Default.EVT_BlockMovementSystem += EVT_BlockMovementSystem;
        }

        public void Teardown()
        {
            GameSystem.Default.EVT_BlockMovementSystem -= EVT_BlockMovementSystem;
        }
        
        private void EVT_BlockMovementSystem(int id, IHexagon hexagon)
        {
            if (block.id == id)
                Movement(block, hexagon);
        }

        private void Movement(IBlock block, IHexagon hexagon)
        {
            if (move_Coroutine != null)
                StopCoroutine(move_Coroutine);
            move_Coroutine = StartCoroutine(movementBlock_Coroutine(block, hexagon));
        }

        private IEnumerator movementBlock_Coroutine(IBlock block, IHexagon hexagon)
        {
            block.SetHexagon(hexagon);
            var time = 0f;
            while (Vector2.Distance(transform.localPosition, Vector2.zero) > 0)
            {
                time = Mathf.MoveTowards(time, animationMaxSpeed, Time.deltaTime * animationVelocitySpeed);
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, time);
                yield return null;
            }
            hexagon.SetBlock(block);

            GameSystem.Default.EVT_HexagonDetectSystemPublish();
        }
    }
}