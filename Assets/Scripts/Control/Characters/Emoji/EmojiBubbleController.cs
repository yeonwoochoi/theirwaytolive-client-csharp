using System;
using System.Collections.Generic;
using System.Linq;
using Control.Layer;
using UnityEngine;
using Random = System.Random;

namespace Control.Characters.Emoji
{
    public class EmojiBubbleController: MonoBehaviour
    {
        [SerializeField] private List<Emoji> emojiList;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;
        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;

        public void Init(LayerType sortingLayer, bool isLeft = false)
        {
            transform.localPosition = new Vector3(!isLeft ? 1.1f : -1.1f, 1.5f, 0f);
            backgroundSpriteRenderer.flipX = isLeft;
            
            SetSortingLayer(sortingLayer);
            Hide();
        }

        public void Show(EmojiType type)
        {
            SetEmoji(type);
            backgroundSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }

        public void Hide()
        {
            backgroundSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            SetEmoji(EmojiType.None);
        }

        private void SetEmoji(EmojiType type)
        {
            if (type == EmojiType.None)
            {
                iconSpriteRenderer.sprite = null;
                return;
            }
            
            foreach (var emoji in emojiList.Where(emoji => emoji.type == type))
            {
                iconSpriteRenderer.sprite = emoji.sprite;
            }
        }
        
        private void SetSortingLayer(LayerType sortingLayer)
        {
            iconSpriteRenderer.sortingLayerName = sortingLayer.LayerTypeToString();
            backgroundSpriteRenderer.sortingLayerName = sortingLayer.LayerTypeToString();

            iconSpriteRenderer.sortingOrder = 32767;
            backgroundSpriteRenderer.sortingOrder = 32766;
        }
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Show((EmojiType) UnityEngine.Random.Range(0, Enum.GetValues(typeof(EmojiType)).Length - 1));
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                Hide();
            }
        }

    }
}