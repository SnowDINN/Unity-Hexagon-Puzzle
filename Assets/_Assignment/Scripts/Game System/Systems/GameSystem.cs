using System;
using System.Collections.Generic;
using Anonymous.Game.Block;
using UnityEngine;

namespace Anonymous.Game
{
    public class GameSystem : MonoBehaviour
    {
        private static GameSystem _default;
        public static GameSystem Default => _default ??= new GameSystem();
        
        public Dictionary<BlockType, List<IBlock>> MatchedBlock = new();

        private void OnEnable()
        {
            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Setup();
            
            var spawns = GetComponentsInChildren<ISpawner>(true);
            foreach (var spawn in spawns)
                spawn.Setup();

            foreach (var type in Enum.GetValues(typeof(BlockType)))
                MatchedBlock.Add((BlockType)type, new List<IBlock>());
        }

        private void OnDisable()
        {
            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Teardown();
            
            var spawns = GetComponentsInChildren<ISpawner>(true);
            foreach (var spawn in spawns)
                spawn.Teardown();
        }
        
        public Vector2 CalculateLocalPosition(Vector3 a, Vector2 b)
        {
            return a + new Vector3(b.x, b.y, 0);
        }
    }
}