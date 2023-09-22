using System;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    public class GameSystem : MonoBehaviour
    {
        public delegate void Delegate_BlockSpawn();
        public event Delegate_BlockSpawn EVT_BlockSpawn;
        
        public delegate void Delegate_BlockResolve();
        public event Delegate_BlockResolve EVT_BlockResolve;

        private static GameSystem _default;
        public static GameSystem Default => _default ??= new GameSystem();

        private void OnEnable()
        {
            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Setup();
            
            var spawns = GetComponentsInChildren<ISpawn>(true);
            foreach (var spawn in spawns)
                spawn.Setup();
        }

        private void OnDisable()
        {
            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Teardown();
            
            var spawns = GetComponentsInChildren<ISpawn>(true);
            foreach (var spawn in spawns)
                spawn.Teardown();
        }

        public void EVT_BlockSpawnPublish()
        {
            EVT_BlockSpawn();
        }
        
        public void EVT_BlockResolvePublish()
        {
            EVT_BlockResolve();
        }
    }
}