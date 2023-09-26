using System;
using System.Collections;
using Anonymous.Game.Block;
using Anonymous.Game.Hexagon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Anonymous.Game
{
    internal partial class GameSystem
    {
        [Header("First Binding Hexagon")] [SerializeField]
        private GameObject BindHexagon;

        [Header("Spawn Options")] [SerializeField]
        private Transform SpawnTransform;

        [SerializeField] private float spawnTime;

        private Coroutine co_spawn;
        private int index;

        private IEnumerator update_spawn(IHexagon hexagon)
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
                var gameObject = Instantiate(resouce, SpawnTransform.position, SpawnTransform.rotation);

                var system = gameObject.GetComponent<ISystem>();
                system?.Setup();

                var block = gameObject.GetComponent<IBlock>();
                block?.Spawn(index, type, hexagon);

                yield return new WaitForSeconds(spawnTime);
            }
        }
    }
}