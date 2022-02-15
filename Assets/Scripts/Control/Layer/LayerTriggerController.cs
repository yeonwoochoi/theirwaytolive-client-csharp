using System;
using Control.Characters;
using UnityEngine;

namespace Control.Layer
{
    public enum LayerType
    {
        Layer1, Layer2, Layer3
    }
    
    public static class LayerTypeUtils
    {
        #region Static Variables

        private const string Layer1Name = "Layer1";
        private const string Layer2Name = "Layer2";
        private const string Layer3Name = "Layer3";

        #endregion

        #region Static Method

        public static string LayerTypeToString(this LayerType type)
        {
            return type switch
            {
                LayerType.Layer1 => Layer1Name,
                LayerType.Layer2 => Layer2Name,
                LayerType.Layer3 => Layer3Name,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static bool IsSameLayer(this GameObject target, LayerType layerType)
        {
            return LayerMask.LayerToName(target.layer) == layerType.LayerTypeToString();
        }

        #endregion
    }
    
    public class LayerTriggerController: MonoBehaviour
    {
        #region Private Variables
        
        [SerializeField] private LayerType sortingLayer;

        #endregion

        #region Event Method

        private void OnTriggerExit2D(Collider2D other)
        {
            var controller = other.gameObject.GetComponent<CharacterLayerController>();
            if (controller == null) return;
            controller.ChangeLayer(sortingLayer);
        }

        #endregion
    }
}