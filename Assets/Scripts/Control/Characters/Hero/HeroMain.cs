using System;
using System.Collections;
using Control.Characters.Base;
using Control.Characters.Enemy;
using Control.Characters.Hero.Control;
using Control.Weapon;
using UnityEngine;
using Logger = Util.Logger;

namespace Control.Characters.Hero
{
    public class HeroMain: MonoBehaviour
    {
        public Hero Hero { get; private set; }
        public HeroStats HeroStats { get; private set; }
        public HeroControlStrategySelector HeroControlStrategySelector { get; private set; }
        public HeroEffectController HeroEffectController { get; private set; }
        public WeaponSystem WeaponSystem { get; private set; }
        private BoxCollider2D boxCollider2D;
        private DamageCalculator damageCalculator;
        
        private bool isSet = false;

        public void Init(Hero hero, ControlType role, WeaponType weaponType)
        {
            if (isSet) return;
            
            Hero = hero;
            HeroStats = GetComponent<HeroStats>();
            HeroControlStrategySelector = GetComponent<HeroControlStrategySelector>();
            HeroEffectController = GetComponent<HeroEffectController>();
            boxCollider2D = GetComponent<BoxCollider2D>();

            HeroControlStrategySelector.Init(role);
            HeroEffectController.Init();
            HeroStats.Init();
            
            WeaponSystem = new WeaponSystem(weaponType, type => HeroControlStrategySelector.ChangeWeapon(type));
            damageCalculator = new DamageCalculator(HeroStats);
            
            isSet = true;
        }

        public void Damaged(Hero.IHeroInteractable attacker)
        {
            if (!isSet) return;
            if (attacker.GetGameObject().TryGetComponent<BaseCharacterStats>(out var characterStats))
            {
                var damageInfo = damageCalculator.CalculateDamage(characterStats);
                HeroEffectController.OnDamagedEffect(attacker, damageInfo);
                HeroStats.HealthSystem.Damaged(damageInfo.amount, Dead);   
            }
        }

        /// <summary>
        /// 역할 바꿀때 이거 호출하면 됨.
        /// </summary>
        /// <param name="type"></param>
        public void ChangeControlType(ControlType type)
        {
            if (!isSet) return;
            HeroControlStrategySelector.ChangeControlRole(type);
        }

        /// <summary>
        /// Weapon 바꿀때 이거 호출하면 됨.
        /// </summary>
        /// <param name="type"></param>
        public void ChangeWeapon(WeaponType type)
        {
            WeaponSystem.SetWeaponType(type);
        }

        private void Dead()
        {
            boxCollider2D.enabled = false;
            HeroControlStrategySelector.Disable();
        }
    }
}