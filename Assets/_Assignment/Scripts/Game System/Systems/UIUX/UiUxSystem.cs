using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Anonymous.Game.UiUx
{
    public class UiUxSystem : MonoBehaviour, IUiUxSystem
    {
        [Header("Last Count")] 
        [SerializeField] private TextMeshProUGUI uiTextCount;

        [Header("Current Score")] 
        [SerializeField] private Slider uiSlider;
        [SerializeField] private TextMeshProUGUI uiTextScore;

        private GameSystem system => GameSystem.Default;

        public void Setup()
        {
            uiTextCount.text = $"{system.installer.MoveCount}";
            uiSlider.maxValue = system.installer.MaxScore;
            uiSlider.onValueChanged.AddListener(value =>
            {
                uiTextScore.text = $"{value}";
            });

            GameEventSystem.EVT_MovementSuccessSystem += EvtMovementSuccessSystem;
            GameEventSystem.EVT_MatchSuccessSystem += EVT_MatchSuccessSystem;
        }

        public void Teardown()
        {
            GameEventSystem.EVT_MovementSuccessSystem -= EvtMovementSuccessSystem;
            GameEventSystem.EVT_MatchSuccessSystem -= EVT_MatchSuccessSystem;
        }

        private void EvtMovementSuccessSystem()
        {
            uiTextCount.text = $"{int.Parse(uiTextCount.text) - 1}";
        }
        
        private void EVT_MatchSuccessSystem(int score)
        {
            uiSlider.value = int.Parse(uiTextScore.text) + score * 20;
        }
    }
}