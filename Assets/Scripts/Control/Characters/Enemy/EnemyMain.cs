using System;
using Control.Characters.Base;
using Control.Characters.Emoji;
using Control.Characters.Type;
using Control.Layer;
using Control.Stuff;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Enemy
{
    public class EnemyMain: MonoBehaviour
    {
        public Enemy Enemy { get; private set; }
        public EnemyStats EnemyStats { get; private set; }
        public WeaponSystem WeaponSystem { get; private set; }
        public EnemyEffectController EnemyEffectController { get; private set; }
        
        private EnemyActionStrategySelector enemyActionStrategySelector;
        private EnemyAnimationController enemyAnimationController;
        private EmojiBubbleController emojiBubbleController; 
        private BoxCollider2D boxCollider2D;
        private DamageCalculator damageCalculator;
        
        private bool isSet = false;

        /*
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(WeaponType.Arrow);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(WeaponType.Spear);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(WeaponType.Sword);
        }
        */

        public void BeforeInit(WeaponType weaponType)
        {
            enemyAnimationController = TryGetComponent<EnemyAnimationController>(out var animationController)
                ? animationController
                : gameObject.AddComponent<EnemyAnimationController>();
            EnemyEffectController = TryGetComponent<EnemyEffectController>(out var effectController)
                ? effectController
                : gameObject.AddComponent<EnemyEffectController>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            
            var emojiBubbleTransform =
                Instantiate(GameAssets.i.pfEmojiBubble, transform.position, Quaternion.identity);
            emojiBubbleTransform.SetParent(transform);
            emojiBubbleController = emojiBubbleTransform.GetComponent<EmojiBubbleController>();

            enemyAnimationController.Init();
            EnemyEffectController.Init();
            emojiBubbleController.Init(LayerType.Layer3);

            WeaponSystem = new WeaponSystem(weaponType, type => enemyAnimationController.ChangeWeapon(type));
        }


        public void Init(EnemyActionType actionType)
        {
            if (isSet) return;
            
            Enemy = GetComponent<Enemy>();
            EnemyStats = GetComponent<EnemyStats>();
            enemyActionStrategySelector = GetComponent<EnemyActionStrategySelector>();

            EnemyStats.Init();
            enemyActionStrategySelector.Init(actionType);
            
            damageCalculator = new DamageCalculator(EnemyStats);
            
            isSet = true;
        }

        public void Damaged(Enemy.IEnemyInteractable attacker)
        {
            if (!isSet) return;
            if (attacker.GetGameObject().TryGetComponent<BaseCharacterStats>(out var characterStats))
            {
                var damageInfo = damageCalculator.CalculateDamage(characterStats);
                EnemyEffectController.OnDamaged(attacker, damageInfo);
                EnemyStats.HealthSystem.Damaged(damageInfo.amount, Dead); 
            }
        }

        public void ShowEmoji(EmojiType type)
        {
            emojiBubbleController.Show(type);
        }

        /// <summary>
        /// Weapon 바꿀때 이거 호출하면 됨.
        /// </summary>
        /// <param name="type"></param>
        public void ChangeWeapon(WeaponType type)
        {
            WeaponSystem.SetWeaponType(type);
        }

        public void Heal(int amount)
        {
            if (!isSet) return;
            EnemyStats.HealthSystem.Heal(amount);
        }

        private void Dead()
        {
            boxCollider2D.enabled = false;
            enemyActionStrategySelector.Disable();
            Enemy.Disable();
        }
    }
}