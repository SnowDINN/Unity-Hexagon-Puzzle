using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Anonymous.Game.Hexagon;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonymous.Game.Block
{
    [Serializable]
    public class BlockPositionModel
    {
        public PositionType type;
        public Vector2[] positions;
    }
    
    public class BlockSystem : MonoBehaviour, ISystem, IBlock
    {
        public List<Collider2D> list;
        
        [SerializeField] private List<BlockPositionModel> blocks;
        
        [SerializeField] private float animationMaxSpeed;
        [SerializeField] private float animationVelocitySpeed;
        
        private readonly Dictionary<PositionType, List<IBlock>> systems = new();
        
        private Coroutine move_Coroutine;
        
        public void Setup()
        {
            GameSystem.Default.EVT_BlockSpawn += EVT_BlockSpawn;
            GameSystem.Default.EVT_BlockResolve += EVT_BlockResolve;
        }

        public void Teardown()
        {
            GameSystem.Default.EVT_BlockSpawn -= EVT_BlockSpawn;
            GameSystem.Default.EVT_BlockResolve -= EVT_BlockResolve;
            
            if (move_Coroutine != null)
                StopCoroutine(move_Coroutine);
        }
        
        public void Spawn(IHexagon hexagon)
        {
            transform.SetParent(hexagon.GetTransform());
            transform.localPosition = Vector2.zero;
        }

        public void Move(IHexagon hexagon)
        {
            transform.SetParent(hexagon.GetTransform());
            
            if (move_Coroutine != null)
                StopCoroutine(move_Coroutine);
            move_Coroutine = StartCoroutine(moveBlock_Coroutine(hexagon));
        }
        
        private void EVT_BlockSpawn()
        {

        }
        
        private void EVT_BlockResolve()
        {
            this.list.Clear();
            foreach (var block in blocks)
            {
                var list = new List<IBlock>();
                foreach (var position in block.positions)
                {
                    var ray = new Ray2D(GameSystem.Default.CalculateLocalPosition(transform.position, position), Vector2.zero);
                    var hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null)
                    {
                        list.Add(hit.transform.GetComponent<IBlock>());
                        this.list.Add(hit.collider);
                    }
                }
                
                systems[block.type] =  list;
            }
        }

        private IEnumerator moveBlock_Coroutine(IHexagon hexagon)
        {
            var time = 0f;
            while (Vector2.Distance(transform.localPosition, Vector2.zero) > 0)
            {
                time = Mathf.MoveTowards(time, animationMaxSpeed, Time.deltaTime * animationVelocitySpeed);
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, time);
                yield return null;
            }
            
            hexagon.SetBlock(this);
            
            GameSystem.Default.EVT_BlockResolvePublish();
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var radius = 0.1f;
            if (Selection.activeGameObject == gameObject)
            {
                Gizmos.color = Color.black;

                foreach (var position in blocks.SelectMany(block => block.positions))
                    Gizmos.DrawSphere(GameSystem.Default.CalculateLocalPosition(transform.position, position), radius);
            }
        }
#endif
    }
}