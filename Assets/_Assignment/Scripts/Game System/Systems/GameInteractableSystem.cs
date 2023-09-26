using System.Collections;
using System.Linq;
using Anonymous.Game.Hexagon;
using UnityEngine;

namespace Anonymous.Game
{
    internal partial class GameSystem
    {
        private Coroutine interactable;
        private Coroutine interactableTarget;
        private Coroutine isNotMatched;

        private IEnumerator update_interactable()
        {
            while (true)
            {
                yield return null;
                
                if (!canInteractable)
                    continue;
                
#if UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
#elif UNITY_ANDROID
                if (Input.touchCount > 0)
                if (Input.GetTouch(0).phase == TouchPhase.Began)
#endif
                {
                    isNotMatchedArray.Clear();
                    
                    if (interactableTarget != null)
                        StopCoroutine(interactableTarget);
                    interactableTarget = StartCoroutine(co_interactableTarget());
                }
            }
        }

        private IEnumerator co_interactableTarget()
        {
            IHexagon current = null;
            IHexagon target = null;
            
            var isMouseButtonDown = true;
            while (isMouseButtonDown)
            {
                yield return null;
                        
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
                        current = hit2D.transform.GetComponent<IHexagon>();
                    else
                        target = hit2D.transform.GetComponent<IHexagon>();
                }

                if (target?.block?.id == current?.block?.id)
                    continue;

                if (target?.block == null || current?.block == null)
                    continue;

                current.EVT_MovementPublish(target.block.id);
                target.EVT_MovementPublish(current.block.id);

                if (isNotMatched != null)
                    StopCoroutine(isNotMatched);
                isNotMatched = StartCoroutine(co_isNotMatched(current, target));

                isMouseButtonDown = false;
            }
        }

        private IEnumerator co_isNotMatched(IHexagon current, IHexagon target)
        {
            while (true)
            {
                yield return null;

                if (isNotMatchedArray.Count < 2)
                    continue;

                var result = true;
                foreach (var notMatched in isNotMatchedArray.Where(notMatched => !notMatched))
                    result = false;

                if (result)
                {
                    current.EVT_MovementPublish(target.block.id);
                    target.EVT_MovementPublish(current.block.id);
                    break;
                }
            }
        }
    }
}