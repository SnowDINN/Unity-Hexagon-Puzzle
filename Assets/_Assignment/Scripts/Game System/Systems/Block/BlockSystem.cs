using System.Collections;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public class BlockSystem : MonoBehaviour, ISystem, IBlock
    {
        [SerializeField] private float animationSpeed;
        
        private Coroutine move_Coroutine;
        
        public void Setup()
        {
            
        }

        public void Teardown()
        {
            if (move_Coroutine != null)
                StopCoroutine(move_Coroutine);
        }

        public void Move(IHexagon hexagon)
        {
            move_Coroutine = StartCoroutine(moveBlock_Coroutine(hexagon.GetPosition()));
        }

        private IEnumerator moveBlock_Coroutine(Vector2 position)
        {
            while (Vector2.Distance(transform.position, position) > 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, position, animationSpeed);
                yield return null;
            }
        }
    }
}