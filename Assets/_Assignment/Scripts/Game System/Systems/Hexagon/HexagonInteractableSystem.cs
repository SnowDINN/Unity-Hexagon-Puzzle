using System.Collections;
using System.Linq;
using UnityEngine;

namespace Anonymous.Game.Hexagon
{
    public class HexagonInteractableSystem : MonoBehaviour, IHexagonSystem
    {
        private Coroutine interactableTarget;
        private Coroutine isNotMatched;
        private IHexagon hexagon;

        private GameSystem system => GameSystem.Default;

        public void Setup(IHexagon hexagon)
        {
            this.hexagon = hexagon;
        }

        public void Teardown()
        {

        }
        
        private void OnMouseDown()
        {
            update_interactable();
        }

        private void OnMouseUp()
        {
            stop_interactable();
        }
        
        private void update_interactable()
        {
            if (!system.canInteractable)
                return;

            GameEventSystem.EVT_MovementSuccessPublish();
            system.isNotMatchedArray.Clear();

            if (interactableTarget != null)
                StopCoroutine(interactableTarget);
            interactableTarget = StartCoroutine(co_interactableTarget());
        }

        private void stop_interactable()
        {
            if (interactableTarget != null)
                StopCoroutine(interactableTarget);

            if (isNotMatched != null)
                StopCoroutine(isNotMatched);
        }

        private IEnumerator co_interactableTarget()
        {
            var processEnd = true;
            while (processEnd)
            {
                yield return null;

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);

                var target = raycastHit2D.transform.GetComponent<IHexagon>();
                if (target?.block?.id == hexagon?.block?.id)
                    continue;

                if (target?.block == null || hexagon?.block == null)
                    continue;

                hexagon.EVT_MovementPublish(target.block.id);
                target.EVT_MovementPublish(hexagon.block.id);

                if (isNotMatched != null)
                    StopCoroutine(isNotMatched);
                isNotMatched = StartCoroutine(co_isNotMatched(hexagon, target));
                
                processEnd = false;
            }
        }

        private IEnumerator co_isNotMatched(IHexagon current, IHexagon target)
        {
            while (true)
            {
                yield return null;

                if (system.isNotMatchedArray.Count < 2)
                    continue;

                var result = true;
                foreach (var _ in system.isNotMatchedArray.Where(notMatched => !notMatched))
                    result = false;

                if (!result)
                    continue;
                
                current.EVT_MovementPublish(target.block.id);
                target.EVT_MovementPublish(current.block.id);
                break;
            }
        }
    }
}