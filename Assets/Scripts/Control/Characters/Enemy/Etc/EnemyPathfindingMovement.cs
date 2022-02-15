using System;
using System.Collections.Generic;
using Control.Characters.Base;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Enemy
{
    public class EnemyPathfindingMovement : BasePathFindingMovement {
        
        private EnemyMain enemyMain;
        private EnemyAnimationController enemyAnimationController;

        public override void Init(float speed)
        {
            base.Init(speed);
            
            rb2D = GetComponent<Rigidbody2D>();
            enemyMain = GetComponent<EnemyMain>();
            enemyAnimationController = GetComponent<EnemyAnimationController>();
            
            changeAnimationMovingState = flag => enemyAnimationController.ChangeMovingState(flag);
            changeAnimationDirection = offset => enemyAnimationController.ChangeDirection(offset);
            getPositionFunc = () => enemyMain.Enemy.GetPosition();
            getReachedTargetDistance = GetReachedTargetDistance;
            
            isSet = true;
        }

        // 임시방편 (target과 유지해야할 거리보다 좀더 가깝게 해주기 위한 offset 값임)
        private float GetReachedTargetDistance(float prev)
        {
            var flag = GetWeaponType() switch
            {
                WeaponType.Arrow => 0.5f,
                WeaponType.Spear => 1.5f,
                WeaponType.Sword => 1f,
                WeaponType.None => 1f,
                _ => throw new ArgumentOutOfRangeException()
            };
            return prev - flag;
        }
        
        private WeaponType GetWeaponType()
        {
            return enemyMain.WeaponSystem.GetWeaponType();
        }
    }
}
