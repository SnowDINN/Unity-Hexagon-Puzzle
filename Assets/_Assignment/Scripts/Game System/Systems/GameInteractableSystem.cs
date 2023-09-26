using System.Collections;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    internal partial class GameSystem
    {
        private Coroutine co_interactable;

        private IEnumerator update_interactable()
        {
            while (true)
            {
#if UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
#elif UNITY_ANDROID
                if (Input.touchCount > 0)
                if (Input.GetTouch(0).phase == TouchPhase.Began)
#endif
                {
                    IHexagon current = null;

                    var isMouseButtonDown = true;
                    while (isMouseButtonDown)
                    {
#if UNITY_EDITOR
                        if (Input.GetMouseButtonUp(0))
#elif UNITY_ANDROID
                        if (Input.GetTouch(0).phase == TouchPhase.Ended)
#endif
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