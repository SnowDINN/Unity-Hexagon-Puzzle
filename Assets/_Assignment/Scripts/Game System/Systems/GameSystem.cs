using System.Collections.Generic;
using Anonymous.Game.Block;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    internal partial class GameSystem : MonoBehaviour
    {
        public static GameSystem Default;

        public Dictionary<int, IBlock> Blocks = new();

        private void OnEnable()
        {
            Default = this;

#if UNITY_EDITOR
            Application.runInBackground = true;
#endif

            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Setup();

            var spawn = BindHexagon.GetComponent<IHexagon>();
            if (spawn != null)
                co_spawn = StartCoroutine(update_spawn(spawn));

            co_interactable = StartCoroutine(update_interactable());
        }

        private void OnDisable()
        {
            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Teardown();

            if (co_spawn != null)
                StopCoroutine(co_spawn);

            if (co_interactable != null)
                StopCoroutine(co_interactable);
        }
    }
}