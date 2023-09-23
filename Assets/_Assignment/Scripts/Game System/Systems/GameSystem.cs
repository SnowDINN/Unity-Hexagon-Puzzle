using System;
using System.Collections.Generic;
using Anonymous.Game.Block;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    public class GameSystem : MonoBehaviour
    {
        private static GameSystem _default;
        public static GameSystem Default => _default ??= new GameSystem();
        
        public delegate void Delegate_BlockMovementSystem(int id, IHexagon hexagon);
        public event Delegate_BlockMovementSystem EVT_BlockMovementSystem;
        
        public delegate void Delegate_BlockMatchSystem(IHexagon hexagon);
        public event Delegate_BlockMatchSystem EVT_BlockMatchSystem;
        
        public delegate void Delegate_HexagonDetectSystem();
        public event Delegate_HexagonDetectSystem EVT_HexagonDetectSystem;
        
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

        public void EVT_BlockMovementPublish(int id, IHexagon hexagon)
        {
            EVT_BlockMovementSystem?.Invoke(id, hexagon);
        }
        
        public void EVT_HexagonDetectSystemPublish()
        {
            EVT_HexagonDetectSystem?.Invoke();
        }
        
        public Vector2 CalculateLocalPosition(Vector3 a, Vector2 b)
        {
            return a + new Vector3(b.x, b.y, 0);
        }
    }
}