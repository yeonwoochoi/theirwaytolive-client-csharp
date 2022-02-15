using Control.Characters.Base;
using UnityEngine;

namespace Control.Characters.Hero
{
    public class HeroEffectController: BaseCharacterEffect
    {
        public override void Init()
        {
            base.Init();
            getPositionFunc = () => transform.position;
            isSet = true;
        }

        /// <summary>
        /// 여기다 공격 받았을 때 원하는 효과 추가하면 됨
        /// </summary>
        public void OnDamagedEffect(Hero.IHeroInteractable attacker, DamageCalculator.DamageInfo damageInfo)
        {
            BlinkingDamagedEffect();
            DamagePopupEffect(damageInfo);
        }

        public void OnDeadEffect()
        {
            FadeOutEffect();
        }
    }
}