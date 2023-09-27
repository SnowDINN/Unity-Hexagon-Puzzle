using System.Linq;
using TMPro;
using UnityEngine;

namespace Anonymous.Game.UiUx.Popup
{
    public class PopupSystem : MonoBehaviour
    {
        [Header("Description Field")]
        public TextMeshProUGUI uiTextDescription;

        private GameSystem system => GameSystem.Default;
        
        public void Reset()
        {
            var list = system.BlockManagement.ToList();
            while (list.Count > 0)
            {
                list[0].Value.Dispose();
                list.RemoveAt(0);
            }
            
            GameSystem.Default.StartedApplication();
            Destroy(gameObject);
        }

        public void Setup(string message)
        {
            uiTextDescription.text = message;
        }
    }
}