using System;
using UnityEngine;

namespace Anonymous.SafeArea
{
    [ExecuteInEditMode]
    public class SafeArea : MonoBehaviour
    {
        private void OnEnable()
        {
            Canvas.willRenderCanvases += willRenderCanvases;
        }

        private void OnDisable()
        {
            Canvas.willRenderCanvases -= willRenderCanvases;
        }
        
        private void willRenderCanvases()
        {
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;

            var minAnchor = safeArea.position;
            var maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}