using System.Collections.Generic;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    internal partial class GameSystem : MonoBehaviour
    {
        public static GameSystem Default;
        public List<bool> isNotMatchedArray = new();
        public List<int> isMovementArray = new();

        private bool canInteractable => isMovementArray.Count <= 0;

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

            interactable = StartCoroutine(update_interactable());
        }

        private void OnDisable()
        {
            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Teardown();

            if (co_spawn != null)
                StopCoroutine(co_spawn);

            if (interactable != null)
                StopCoroutine(interactable);
        }
    }
}