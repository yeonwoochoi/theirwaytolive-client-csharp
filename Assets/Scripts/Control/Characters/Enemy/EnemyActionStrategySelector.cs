using System;
using System.Collections.Generic;
using Control.Characters.Enemy.Action;
using Control.Characters.Enemy.Targeting;
using Control.Characters.Hero.Control.Strategies;
using Control.Characters.Type;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Enemy
{
    public class EnemyActionStrategySelector: MonoBehaviour
    {
        private bool isSet = false;
        
        private IEnemyMovable activeActionStrategy;
        
        private EnemyMain enemyMain;
        private EnemyTargeting enemyTargeting;
        private EnemyAnimationController enemyAnimationController;

        private List<IEnemyMovable> strategies;
        
        public void Init(EnemyActionType actionType)
        {
            if (isSet) return;

            enemyMain = GetComponent<EnemyMain>();

            enemyAnimationController = TryGetComponent<EnemyAnimationController>(out var animationController)
                ? animationController
                : gameObject.AddComponent<EnemyAnimationController>();
            enemyTargeting = TryGetComponent<EnemyTargeting>(out var targeting)
                ? targeting
                : gameObject.AddComponent<EnemyTargeting>();
            
            enemyAnimationController.Init();

            strategies = new List<IEnemyMovable>();
            
            SetControlStrategy(actionType);
            
            isSet = true;
        }

        public void ChangeWeapon(WeaponType type)
        {
            if (!isSet) return;
            enemyAnimationController.ChangeWeapon(type);
        }

        public EnemyActionType GetActiveActionType()
        {
            return activeActionStrategy.GetEnemyActionType();
        }

        private void SetControlStrategy(EnemyActionType actionType)
        {
            var speed = 0f;
            
            // 없으면 추가해주고 각 타입에 맞게 설정하기
            switch(actionType)
            {
                case EnemyActionType.Attack:
                    if (GetComponent<EnemyAttackMoveStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<EnemyAttackMoveStrategy>());
                    speed = 2f;
                    enemyTargeting.Init(DetectModeType.Circle);
                    break;
                case EnemyActionType.Escape:
                    if (GetComponent<EnemyEscapeMoveStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<EnemyEscapeMoveStrategy>());
                    speed = 2f;
                    enemyTargeting.Init(DetectModeType.Circle);
                    break;
                case EnemyActionType.Detect:
                    if (GetComponent<EnemyDetectMoveStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<EnemyDetectMoveStrategy>());
                    speed = 2f;
                    enemyTargeting.Init(DetectModeType.Sector);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
            }

            activeActionStrategy?.Disable();
            activeActionStrategy = GetMoveStrategyFromActionType(actionType);
            activeActionStrategy.Init(speed);
        }

        private IEnemyMovable GetMoveStrategyFromActionType(EnemyActionType actionType)
        {
            IEnemyMovable result = null;
            foreach (var strategy in strategies)
            {
                if (actionType == strategy.GetEnemyActionType())
                {
                    result = strategy;
                }
            }

            return result;
        }

        public void Disable()
        {
            activeActionStrategy?.Disable();
            enemyTargeting.Disable();
            // Hero와 다르게 action type에 따라서 Disable 될 때 나오는 Animation이 다를 수 있기 때문에 각자 Custom화 된 Disable 쓰려고 각 Strategy의 Disable에다가 구현함
        }
    }
}