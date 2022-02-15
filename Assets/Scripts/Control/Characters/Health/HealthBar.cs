using Control.Layer;
using UnityEngine;
using Util;

namespace Control.Characters.Health
{
    public class HealthBar
    {
        private Outline outline;
        private Transform background;
        private Transform bar;
        
        private GameObject gameObject;
        private Transform transform;

        public class Outline
        {
            public float size = 1f;
            public Color color = Color.black;
        }

        public HealthBar(Transform parent, Vector3 localPosition, Vector3 localScale, Color? backgroundColor, Color barColor, float sizeRatio, LayerType layerType, int sortingOrder, Outline outline = null)
        {
            this.outline = outline;
            SetupParent(parent, localPosition);
            if (outline != null) SetupOutline(outline, localScale, layerType, sortingOrder);
            SetupBar(barColor, localScale, layerType,sortingOrder + 1);
            SetSize(sizeRatio);
        }

        private void SetupParent(Transform parent, Vector3 localPosition)
        {
            gameObject = new GameObject("Health Bar");
            transform = gameObject.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
        }

        private void SetupOutline(Outline outline, Vector3 localScale, LayerType layerType, int sortingOrder)
        {
            UtilsClass.CreateWorldSprite(transform, "Outline", GetWhiteBarSprite(), Vector3.zero,
                localScale + new Vector3(outline.size, outline.size), layerType, sortingOrder, outline.color);
        }

        private void SetBackground(Color backgroundColor, Vector3 localScale, LayerType layerType, int sortingOrder)
        {
            background = UtilsClass.CreateWorldSprite(transform, "Background", GetWhiteBarSprite(), Vector3.zero,
                localScale, layerType, sortingOrder, backgroundColor).transform;
        }

        private void SetupBar(Color barColor, Vector3 localScale, LayerType layerType, int sortingOrder)
        {
            var barObject = new GameObject("Bar");
            bar = barObject.transform;
            bar.SetParent(transform);
            bar.localPosition = new Vector3(-localScale.x / 2f, 0, 0);
            bar.localScale = Vector3.one;
            var innerBar = UtilsClass.CreateWorldSprite(bar, "Inner Bar", GetWhiteBarSprite(),
                new Vector3(localScale.x / 2f, 0f), localScale, layerType, sortingOrder, barColor).transform;
        }

        // HP 닳았을 때 called
        public void SetSize(float sizeRatio)
        {
            bar.localScale = new Vector3(sizeRatio, 1, 1);
        }

        public void SetLocalScale(Vector3 localScale)
        {
            // Outline
            if (transform.Find("Outline") != null)
            {
                transform.Find("Outline").localScale = localScale + new Vector3(outline.size, outline.size);
            }

            // Background
            background.localScale = localScale;
            
            // Bar scale
            bar.localPosition = new Vector3(-localScale.x / 2f, 0, 0);
            var innerBar = bar.Find("Inner Bar");
            innerBar.localScale = localScale;
            innerBar.localPosition = new Vector3(localScale.x / 2f, 0);
        }

        private Sprite GetWhiteBarSprite()
        {
            return Sprite.Create(Texture2D.whiteTexture, Rect.MinMaxRect(0,0,1,1), new Vector2(0.5f, 0.5f), 1f);
        }
        
        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}