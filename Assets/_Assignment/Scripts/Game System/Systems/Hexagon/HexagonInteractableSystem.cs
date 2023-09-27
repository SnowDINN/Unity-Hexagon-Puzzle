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
                
            hexagon.EVT_MovementPublish(target.block.id);
            target.EVT_MovementPublish(hexagon.block.id);
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