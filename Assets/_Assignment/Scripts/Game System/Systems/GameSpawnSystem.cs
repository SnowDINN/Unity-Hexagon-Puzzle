using System.Collections;
using System.Collections.Generic;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game.Block
{
    public class GameSpawnSystem : MonoBehaviour, ISpawn
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
            while (true)
            {
                if (!hexagon.HasBlock())
                {
                    var go = Instantiate(blocks[Random.Range(0, blocks.Count)]);
                    var system = go.GetComponent<ISystem>();
                    system?.Setup();

                    var block = go.GetComponent<IBlock>();
                    block?.Move(hexagon);
                    
                    hexagon.SetBlock(block);
                    
                    GameSystem.Default.EVT_BlockSpawnPublish();
                }

                yield return null;
            }
        }
    }
}