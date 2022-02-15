using System.Collections;
using System.Collections.Generic;
using Control.Layer;
using UnityEngine;
using TMPro;
using Util;

namespace UI
{
    public class DamagePopup : MonoBehaviour {

        // Create a Damage Popup
        // TODO (Damage popup create) : prefab을 어떻게 받아와서 넣어줄건지..
        public static DamagePopup Create(Transform prefab, Vector3 position, string damageAmount, bool isCriticalHit)
        {
            Transform damagePopupTransform = Instantiate(prefab, position, Quaternion.identity);

            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(damageAmount, isCriticalHit);

            return damagePopup;
        }

        private static int sortingOrder = 10000;

        private const float DISAPPEAR_TIMER_MAX = 1f;

        private TextMeshPro textMesh;
        private float disappearTimer;
        private Color textColor;
        private Vector3 moveVector;

        private void Awake() {
            textMesh = transform.GetComponent<TextMeshPro>();
        }

        public void Setup(string damageAmount, bool isCriticalHit) {
            textMesh.SetText(damageAmount);
            if (!isCriticalHit) {
                // Normal hit
                textMesh.fontSize = 6;
                textColor = UtilsClass.GetColorFromString("FFC500");
            } else {
                // Critical hit
                textMesh.fontSize = 8;
                textColor = UtilsClass.GetColorFromString("FF2B00");
            }
            textMesh.color = textColor;
            disappearTimer = DISAPPEAR_TIMER_MAX;

            SetLayer();
            sortingOrder++;
            textMesh.sortingOrder = sortingOrder;

            moveVector = new Vector3(.7f, 1) * 6f;
        }

        private void SetLayer()
        {
            textMesh.sortingLayerID = SortingLayer.NameToID(LayerType.Layer1.LayerTypeToString());
        }
        
        private void Update() {
            transform.position += moveVector * Time.deltaTime;
            moveVector -= moveVector * 8f * Time.deltaTime;

            if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
                // First half of the popup lifetime
                float increaseScaleAmount = 1f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            } else {
                // Second half of the popup lifetime
                float decreaseScaleAmount = 1f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }

            disappearTimer -= Time.deltaTime;
            if (disappearTimer < 0) {
                // Start disappearing
                float disappearSpeed = 3f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0) {
                    Destroy(gameObject);
                }
            }
        }
    }
}