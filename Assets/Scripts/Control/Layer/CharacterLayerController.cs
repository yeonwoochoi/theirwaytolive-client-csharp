using UnityEngine;

namespace Control.Layer
{
    public class CharacterLayerController: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        public void ChangeLayer(LayerType sortingLayer)
        {
            foreach (var sr in spriteRenderers)
            {
                sr.sortingLayerName = sortingLayer.LayerTypeToString();
            }
        }
    }
}