using System.Collections;
using System.Collections.Generic;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public class GameSpawnerSystem : MonoBehaviour, ISpawner
    {
        [SerializeField] private List<GameObject> blocks;

        private Coroutine spawn_Coroutine;

        public void Setup()
        {
            var hexagon = GetComponent<IHexagon>();
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
            var first = true;
            while (true)
            {
                if (hexagon.HasBlock())
                {
                    yield return null;
                    continue;
                }
                
                var go = Instantiate(blocks[Random.Range(0, blocks.Count)]);
                var system = go.GetComponent<ISystem>();
                system?.Setup();

                var block = go.GetComponent<IBlock>();
                block?.Spawn(hexagon);
                    
                hexagon.SetBlock(block);

                yield return new WaitForSeconds(first ? 0.25f : 0.15f);
                first = false;
            }
        }
    }
}