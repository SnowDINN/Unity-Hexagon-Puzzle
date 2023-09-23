using System;
using System.Collections;
using Anonymous.Game.Block;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    public class GameSpawnerSystem : MonoBehaviour, ISpawner
    {
        [SerializeField] private GameObject hexagongGameObject;
        
        private Coroutine spawn_Coroutine;
        private int index;

        public void Setup()
        {
            var hexagon = hexagongGameObject.GetComponent<IHexagon>();
            if (hexagon != null)
                spawn_Coroutine = StartCoroutine(spawnBlock_Coroutine(hexagon));
        }

        public void Teardown()
        {
            if (spawn_Coroutine != null)
                StopCoroutine(spawn_Coroutine);
        }

        private IEnumerator spawnBlock_Coroutine(IHexagon hexagon)
        {
            while (true)
            {
                if (hexagon.HasBlock())
                {
                    yield return null;
                    continue;
                }

                var type = (BlockType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BlockType)).Length);
                var resouce = Resources.Load($"Block {type}") as GameObject;
                var gameObject = Instantiate(resouce, transform.position, transform.rotation);
                
                var system = gameObject.GetComponent<ISystem>();
                system?.Setup();

                var block = gameObject.GetComponent<IBlock>();
                block?.Spawn(index++, type, hexagon);
                    
                hexagon.SetBlock(block);

                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}