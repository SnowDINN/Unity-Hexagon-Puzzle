using System.Collections;
using System.Linq;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    public class HexagonInteractableSystem : MonoBehaviour, IHexagonSystem
    {
        private IHexagon hexagon;
        private Coroutine interactableTarget;
        private Coroutine isNotMatched;

        private GameSystem system => GameSystem.Default;

        private void OnMouseDown()
        {
            if (system.count > 0)
                update_interactable();
        }

        private void OnMouseUp()
        {
            stop_interactable();
        }

        public void Setup(IHexagon hexagon)
        {
            this.hexagon = hexagon;
        }

        public void Teardown()
        {
            if (interactableTarget != null)
                StopCoroutine(interactableTarget);

            if (isNotMatched != null)
                StopCoroutine(isNotMatched);
        }

        private void update_interactable()
        {
            if (!system.canInteractable)
                return;

            system.isNotMatchedArray.Clear();

            if (interactableTarget != null)
                StopCoroutine(interactableTarget);
            interactableTarget = StartCoroutine(co_interactableTarget());
        }

        private void stop_interactable()
        {
            if (interactableTarget != null)
                StopCoroutine(interactableTarget);
        }

        private IEnumerator co_interactableTarget()
        {
            IHexagon target = null;
            while (target == null || target.block?.id == hexagon.block?.id)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);

                target = raycastHit2D.transform.GetComponent<IHexagon>();
                yield return null;
            }

            if (isNotMatched != null)
                StopCoroutine(isNotMatched);
            isNotMatched = StartCoroutine(co_isNotMatched(hexagon, target));

            if (target.block?.id > 0 && hexagon.block?.id > 0)
            {
                hexagon.EVT_MovementPublish(target.block.id);
                target.EVT_MovementPublish(hexagon.block.id);
            }

            GameEventSystem.EVT_MovementSuccessPublish();
        }

        private IEnumerator co_isNotMatched(IHexagon current, IHexagon target)
        {
            while (system.isNotMatchedArray.Count < 2)
                yield return null;

            var result = true;
            foreach (var _ in system.isNotMatchedArray.Where(notMatched => !notMatched))
                result = false;

            if (!result)
                yield break;

            current.EVT_MovementPublish(target.block.id);
            target.EVT_MovementPublish(current.block.id);
        }
    }
}