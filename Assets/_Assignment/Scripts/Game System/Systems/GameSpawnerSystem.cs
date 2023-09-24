using System;
using System.Collections;
using Anonymous.Game.Block;
using Anonymous.Game.Hexagon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Anonymous.Game
{
    public class GameSpawnerSystem : MonoBehaviour, ISpawner
    {
        [SerializeField] private GameObject hexagongGameObject;
        private int index;

        private Coroutine spawn_Coroutine;

        public void Setup()
        {
            var hexagon = hexagongGameObject.GetComponent<IHexagon>();
            if (hexagon != null)
                spawn_Coroutine = StartCoroutine(update_Spawn(hexagon));
        }

        public void Teardown()
        {
            if (spawn_Coroutine != null)
                StopCoroutine(spawn_Coroutine);
        }

        private IEnumerator update_Spawn(IHexagon hexagon)
        {
            while (true)
            {
                if (hexagon.hasBind)
                {
                    yield return null;
                    continue;
                }

                index += 1;

                var type = (BlockType)Random.Range(0, Enum.GetValues(typeof(BlockType)).Length);
                var resouce = Resources.Load($"Block {type}") as GameObject;
                var gameObject = Instantiate(resouce, transform.position, transform.rotation);

                var system = gameObject.GetComponent<ISystem>();
                system?.Setup();

                var block = gameObject.GetComponent<IBlock>();
                block?.Spawn(index, type, hexagon);

                hexagon.BindBlock(block);

                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}