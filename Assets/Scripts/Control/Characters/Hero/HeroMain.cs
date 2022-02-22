using System;
using System.Collections;
using Control.Characters.Base;
using Control.Characters.Emoji;
using Control.Characters.Enemy;
using Control.Characters.Health;
using Control.Characters.Hero.Control;
using Control.Characters.Type;
using Control.Layer;
using Control.Stuff;
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

        private HeroAnimationController heroAnimationController;
        private BoxCollider2D boxCollider2D;
        private DamageCalculator damageCalculator;
        private EmojiBubbleController emojiBubbleController;
        
        private bool isSet = false;
        
        public void BeforeInit(WeaponType weaponType)
        {
            heroAnimationController = TryGetComponent<HeroAnimationController>(out var animationController)
                ? animationController
                : gameObject.AddComponent<HeroAnimationController>();
            HeroEffectController = TryGetComponent<HeroEffectController>(out var effectController)
                ? effectController
                : gameObject.AddComponent<HeroEffectController>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            
            var emojiBubbleTransform =
                Instantiate(GameAssets.i.pfEmojiBubble, transform.position, Quaternion.identity);
            emojiBubbleTransform.SetParent(transform);
            emojiBubbleController = emojiBubbleTransform.GetComponent<EmojiBubbleController>();

            heroAnimationController.Init();
            HeroEffectController.Init();
            emojiBubbleController.Init(LayerType.Layer3);

            WeaponSystem = new WeaponSystem(weaponType, type => heroAnimationController.ChangeWeapon(type));
        }

        public void Init(Hero hero, HeroControlType role, WeaponType weaponType)
        {
            if (isSet) return;
            
            Hero = hero;
            HeroStats = GetComponent<HeroStats>();
            HeroControlStrategySelector = GetComponent<HeroControlStrategySelector>();

            HeroControlStrategySelector.Init(role);
            HeroStats.Init();
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
        public void ChangeControlType(HeroControlType type)
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
        
        public void ShowEmoji(EmojiType type)
        {
            emojiBubbleController.Show(type);
        }

        /// <summary>
        /// Heal 할때 이거 호출하면 됨.
        /// </summary>
        /// <param name="amount"></param>
        public void Heal(int amount)
        {
            if (!isSet) return;
            HeroStats.HealthSystem.Heal(amount);
        }

        private void Dead()
        {
            boxCollider2D.enabled = false;
            HeroControlStrategySelector.Disable();
        }
    }
}