using System.Collections.Generic;
using Anonymous.Game.Hexagon;
using Anonymous.Game.UiUx;
using UnityEngine;

namespace Anonymous.Game
{
    internal partial class GameSystem : MonoBehaviour
    {
        public static GameSystem Default;
        [HideInInspector] public List<bool> isNotMatchedArray = new();
        [HideInInspector] public List<int> isMovementArray = new();
        [HideInInspector] public Installer.Installer installer;

        public bool canInteractable => isMovementArray.Count <= 0;

        private void OnEnable()
        {
            Default = this;
            installer = Resources.Load("Installer") as Installer.Installer;
            
#if UNITY_EDITOR
            Application.runInBackground = true;
#endif

            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var system in systems)
                system.Setup();
            
            var uiUxSystems = GetComponentsInChildren<IUiUxSystem>(true);
            foreach (var system in uiUxSystems)
                system.Setup();

            var spawn = BindHexagon.GetComponent<IHexagon>();
            if (spawn != null)
                co_spawn = StartCoroutine(update_spawn(spawn));
        }

        private void OnDisable()
        {
            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Teardown();
            
            var uiUxSystems = GetComponentsInChildren<IUiUxSystem>(true);
            foreach (var system in uiUxSystems)
                system.Teardown();

            if (co_spawn != null)
                StopCoroutine(co_spawn);
        }
    }
}