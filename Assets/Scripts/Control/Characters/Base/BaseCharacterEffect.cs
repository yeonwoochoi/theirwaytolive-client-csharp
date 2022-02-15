using System;
using System.Collections;
using Control.Characters.Hero;
using Control.Stuff;
using MeshParticleSystem.Scripts;
using UI;
using UnityEngine;

namespace Control.Characters.Base
{
    public abstract class BaseCharacterEffect: MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        protected bool isSet = false;
        private bool isKnockBacked = false;
        private bool isBlinking = false;
        private bool isFadeOut = false;

        private readonly float fadeOutSpeed = 1.5f;
        private readonly int blinkTimes = 2;
        private readonly float knockBackDistance = 0.5f;
        
        protected Func<Vector3> getPositionFunc;

        public virtual void Init()
        {
            if (isSet) return;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected void KnockBackEffect(Vector3 attackerPosition)
        {
            if (!isSet || isKnockBacked) return;
            var knockBackDir = (getPositionFunc() - attackerPosition).normalized * knockBackDistance;
            StartCoroutine(KnockBack(knockBackDir));
        }

        protected void BloodEffect(Vector3 attackerPosition)
        {
            if (!isSet) return;
            var bloodDir = (getPositionFunc() - attackerPosition).normalized;
            BloodParticleSystemHandler.Instance.SpawnBlood(5, getPositionFunc(), bloodDir);
        }
        
        protected void BlinkingDamagedEffect()
        {
            if (!isSet || isBlinking) return;
            StartCoroutine(BlinkingEffect());
        }
        
        protected void DamagePopupEffect(bool isCritical, bool isMiss, int damage)
        {
            if (isMiss)
            {
                DamagePopup.Create(GameAssets.i.pfDamagePopup, getPositionFunc(), $"Miss", isCritical);
                return;
            }
            DamagePopup.Create(GameAssets.i.pfDamagePopup, getPositionFunc(), $"{damage}", isCritical);
        }

        protected void DamagePopupEffect(DamageCalculator.DamageInfo damageInfo)
        {
            DamagePopupEffect(damageInfo.isCritical, damageInfo.isMiss, damageInfo.amount);
        }

        private IEnumerator BlinkingEffect()
        {
            if (isBlinking) yield break;
            isBlinking = true;
            for (var i = 0; i < blinkTimes; i++)
            {
                yield return new WaitForSeconds(0.3f);
                spriteRenderer.color = Color.grey;
                yield return new WaitForSeconds(0.15f);
                spriteRenderer.color = Color.white;
            }
            isBlinking = false;
        }

        private IEnumerator KnockBack(Vector3 offset)
        {
            if (isKnockBacked) yield break;
            isKnockBacked = true;
            var from = getPositionFunc();
            var to = getPositionFunc() + offset;
            var distance = Vector3.SqrMagnitude(to - getPositionFunc());
            while (distance > 0.01f)
            {
                distance = Vector3.SqrMagnitude(to - getPositionFunc());
                transform.position = Vector3.Lerp(to, getPositionFunc(), 0.75f);
                yield return null;
            }
            isKnockBacked = false;
        }

        protected void FadeOutEffect()
        {
            if (!isSet) return;
            StartCoroutine(FadeOut());
        }
        
        private IEnumerator FadeOut()
        {
            if (isFadeOut) yield break;
            isFadeOut = true;
            
            yield return new WaitForSeconds(1f);
            for (float i = 1; i >= 0; i -= Time.deltaTime * fadeOutSpeed)
            {
                spriteRenderer.color = new Color(1, 1, 1, i);
                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}