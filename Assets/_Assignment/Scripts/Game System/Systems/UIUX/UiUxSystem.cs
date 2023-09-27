using System.Collections;
using System.Linq;
using Anonymous.Game.UiUx.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Anonymous.Game.UiUx
{
    public class UiUxSystem : MonoBehaviour, IUiUxSystem
    {
        [Header("Last Count")] [SerializeField]
        private TextMeshProUGUI uiTextCount;

        [Header("Current Score")] [SerializeField]
        private Slider uiSlider;

        [SerializeField] private TextMeshProUGUI uiTextScore;
        [SerializeField] private Image uiImageTrophy;

        [Header("Popup GameObject")] [SerializeField]
        private GameObject popupGameObject;

        private bool isAllChecking;

        private GameSystem system => GameSystem.Default;

        public void Setup()
        {
            system.count = system.installer.MoveCount;
            system.score = 0;

            uiTextCount.text = $"{system.count}";

            uiSlider.value = system.score;
            uiSlider.maxValue = system.installer.MaxScore;
            uiSlider.onValueChanged.AddListener(value =>
            {
                system.score = (int)value;
                uiTextScore.text = $"{value:N0}";

                if (value >= uiSlider.maxValue)
                {
                    StartCoroutine(co_endedApplication("Game Complete !!"));
                    uiImageTrophy.color = Color.white;
                    
                    uiSlider.onValueChanged.RemoveAllListeners();
                }
            });
            uiImageTrophy.color = Color.black;

            GameEventSystem.EVT_MovementSuccessSystem += EvtMovementSuccessSystem;
            GameEventSystem.EVT_MatchSuccessSystem += EVT_MatchSuccessSystem;
        }

        public void Teardown()
        {
            GameEventSystem.EVT_MovementSuccessSystem -= EvtMovementSuccessSystem;
            GameEventSystem.EVT_MatchSuccessSystem -= EVT_MatchSuccessSystem;
            
            isAllChecking = false;
        }

        public void Retry()
        {
            system.EndedApplication();

            var list = system.BlockManagement.ToList();
            while (list.Count > 0)
            {
                list[0].Value.Dispose();
                list.RemoveAt(0);
            }

            system.StartedApplication();
        }

        private void EvtMovementSuccessSystem()
        {
            system.count -= 1;
            uiTextCount.text = $"{system.count}";

            if (system.count < 1)
                StartCoroutine(co_endedApplication("Game Over !!"));
        }

        private void EVT_MatchSuccessSystem(int score)
        {
            uiSlider.value = system.score + score * 20;
        }

        private IEnumerator co_waitSpawn()
        {
            while (system.HexagonCount != system.BlockManagement.Count)
            {
                isAllChecking = true;
                yield return null;
            }
        }

        private IEnumerator co_waitInteractable()
        {
            while (!system.canInteractable)
            {
                isAllChecking = true;
                yield return null;
            }
        }

        private IEnumerator co_endedApplication(string message)
        {
            isAllChecking = true;
            while (isAllChecking)
            {
                isAllChecking = false;

                yield return co_waitSpawn();
                yield return co_waitInteractable();
            }

            var go = Instantiate(popupGameObject);
            go.GetComponent<PopupSystem>().Setup(message);

            system.EndedApplication();
        }
    }
}