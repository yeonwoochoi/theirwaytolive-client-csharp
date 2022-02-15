using System;
using Control.Characters.Base;
using Control.Characters.Hero;
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

        private EnemyMoveStrategy enemyMoveStrategy;
        private BoxCollider2D boxCollider2D;
        private DamageCalculator damageCalculator;
        
        private bool isSet = false;

        public void Init(Enemy enemy, WeaponType weaponType)
        {
            if (isSet) return;
            
            Enemy = enemy;
            EnemyStats = GetComponent<EnemyStats>();
            enemyMoveStrategy = GetComponent<EnemyMoveStrategy>();
            EnemyEffectController = GetComponent<EnemyEffectController>();
            boxCollider2D = GetComponent<BoxCollider2D>();

            EnemyStats.Init();
            EnemyEffectController.Init();
            enemyMoveStrategy.Init();

            WeaponSystem = new WeaponSystem(weaponType, type => enemyMoveStrategy.ChangeWeapon(type));
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

        private void Dead()
        {
            boxCollider2D.enabled = false;
            enemyMoveStrategy.Disable();
            Enemy.Disable();
        }
    }
}