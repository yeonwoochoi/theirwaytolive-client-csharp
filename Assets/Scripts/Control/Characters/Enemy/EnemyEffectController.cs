using Control.Characters.Base;
using Control.Characters.Hero;
using Control.Characters.Type;
using UI;
using UnityEngine;

namespace Control.Characters.Enemy
{
    public class EnemyEffectController: BaseCharacterEffect
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
        public void OnDamaged(Enemy.IEnemyInteractable attacker, DamageCalculator.DamageInfo damageInfo)
        { 
            if (attacker.GetControlType() == HeroControlType.Joystick) KnockBackEffect(attacker.GetPosition());
            BloodEffect(attacker.GetPosition());
            DamagePopupEffect(damageInfo.isCritical, damageInfo.isMiss, damageInfo.amount);
        }

        public void OnDead()
        {
            FadeOutEffect();
        }
    }
}