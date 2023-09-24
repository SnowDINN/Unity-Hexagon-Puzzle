using System;
using System.Collections;
using System.Collections.Generic;
using Anonymous.Game.Block;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    public class GameSystem : MonoBehaviour
    {
        public static GameSystem Default;

        public Dictionary<BlockType, List<IBlock>> MatchedBlock = new();

        private void OnEnable()
        {
            Default = this;

            foreach (var type in Enum.GetValues(typeof(BlockType)))
                MatchedBlock.Add((BlockType)type, new List<IBlock>());

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
            var isClick = ActivateType.Enable;
            IHexagon current = null;

            while (true)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    current = null;
                    isClick = ActivateType.Enable;
                }

                if (Input.GetMouseButtonDown(0))
                    while (isClick == ActivateType.Enable)
                    {
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
                                if (target.block?.id == current.block?.id)
                                    continue;

                                if (target.block == null || current.block == null)
                                {
                                    isClick = ActivateType.Disable;
                                    continue;
                                }

                                current.EVT_MovementPublish(target.block.id);
                                current.block.BindHexagon(target);

                                target.EVT_MovementPublish(current.block.id);
                                target.block.BindHexagon(current);

                                isClick = ActivateType.Disable;
                            }
                        }

                        yield return null;
                    }

                yield return null;
            }
        }
    }
}