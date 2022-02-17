using System;
using Control.Characters.Base;
using Control.Characters.Enemy.Action;
using Control.Characters.Hero;
using Control.Characters.Type;
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
        private BoxCollider2D boxCollider2D;
        private DamageCalculator damageCalculator;
        
        private bool isSet = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(WeaponType.Arrow);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(WeaponType.Spear);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(WeaponType.Sword);
        }


        public void Init(EnemyActionType actionType, WeaponType weaponType)
        {
            if (isSet) return;
            
            Enemy = GetComponent<Enemy>();
            EnemyStats = GetComponent<EnemyStats>();
            enemyActionStrategySelector = GetComponent<EnemyActionStrategySelector>();
            EnemyEffectController = GetComponent<EnemyEffectController>();
            boxCollider2D = GetComponent<BoxCollider2D>();

            EnemyStats.Init();
            EnemyEffectController.Init();
            enemyActionStrategySelector.Init(actionType);

            WeaponSystem = new WeaponSystem(weaponType, type => enemyActionStrategySelector.ChangeWeapon(type));
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