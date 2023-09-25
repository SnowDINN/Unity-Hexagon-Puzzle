using System;
using System.Collections;
using System.Collections.Generic;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    public class GameSystem : MonoBehaviour
    {
        public static GameSystem Default;
        public List<IHexagon> targetHexagon = new();

        private void OnEnable()
        {
            Default = this;

            var systems = GetComponentsInChildren<ISystem>(true);
            foreach (var hexagon in systems)
                hexagon.Setup();

            var spawns = GetComponentsInChildren<ISpawner>(true);
            foreach (var spawn in spawns)
                spawn.Setup();

            StartCoroutine(update_SelectPoint());
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

        private IEnumerator update_SelectPoint()
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    IHexagon current = null;

                    var isMouseButtonDown = true;
                    while (isMouseButtonDown)
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            current = null;
                            isMouseButtonDown = false;
                        }

                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        var hit2Ds = Physics2D.RaycastAll(ray.origin, ray.direction);
                        foreach (var hit2D in hit2Ds)
                        {
                            if (hit2D.collider == null || !hit2D.collider.CompareTag("Hexagon"))
                                continue;

                            if (current == null)
                            {
                                current = hit2D.transform.GetComponent<IHexagon>();
                            }
                            else
                            {
                                var target = hit2D.transform.GetComponent<IHexagon>();
                                if (target?.block?.id == current?.block?.id)
                                    continue;

                                if (target?.block == null || current?.block == null)
                                    continue;

                                targetHexagon.Add(current);
                                targetHexagon.Add(target);
                                
                                current.EVT_MovementPublish(target.block.id);
                                target.EVT_MovementPublish(current.block.id);
                                
                                isMouseButtonDown = false;
                            }
                        }

                        yield return null;
                    }
                }

                yield return null;
            }
        }
    }
}