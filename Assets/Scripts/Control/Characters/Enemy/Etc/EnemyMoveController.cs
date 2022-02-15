using System;
using System.Collections;
using Control.Characters.Base;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using Util;

namespace Control.Characters.Enemy
{
    public class EnemyMoveController: MonoBehaviour
    {
        private bool isSet = false;
        private enum State
        {
            Normal, Chasing, Attacking, Busy
        }
        private State state;
        
        private EnemyMain enemyMain;
        private EnemyTargeting enemyTargeting;
        private EnemyAnimationController enemyAnimationController;
        private EnemyPathfindingMovement pathfindingMovement;

        private float detectableRange;
        private const float wanderRange = 3f;
        private const float attackCoolTime = 2f;
        private const float wanderCoolTime = 3.5f;
        private bool isWanderCool = false;
        private bool isAttackCool = false;
        
        private Coroutine moveCoroutine;
        
        private Enemy.IEnemyInteractable target;

        public void Init()
        {
            if (isSet) return;
            
            enemyMain = GetComponent<EnemyMain>();
            
            enemyAnimationController = TryGetComponent<EnemyAnimationController>(out var animationController)
                ? animationController
                : gameObject.AddComponent<EnemyAnimationController>();
            enemyTargeting = TryGetComponent<EnemyTargeting>(out var targeting)
                ? targeting
                : gameObject.AddComponent<EnemyTargeting>();
            pathfindingMovement = GetComponent<EnemyPathfindingMovement>() == null 
                ? gameObject.AddComponent<EnemyPathfindingMovement>() 
                : GetComponent<EnemyPathfindingMovement>();
            
            enemyTargeting.Init();
            enemyAnimationController.Init();
            pathfindingMovement.Init(GetSpeed());
            
            detectableRange = EnemyTargeting.detectableRange;
            state = State.Normal;
            target = null;
            isWanderCool = false;
            isAttackCool = false;
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());

            isSet = true;
        }

        public void ChangeWeapon(WeaponType weaponType)
        {
            if (!isSet) return;
            enemyAnimationController.ChangeWeapon(weaponType);
        }

        public void Disable()
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            pathfindingMovement.Disable();
            
            enemyTargeting.Disable();
            
            if (enemyMain.Enemy.IsDead())
            {
                enemyAnimationController.ChangeDeathState();
            }
            else
            {
                enemyAnimationController.ChangeDirection(0, 1);
                enemyAnimationController.ChangeMovingState(false);
            }
        }

        private IEnumerator Move()
        {
            while (true)
            {
                yield return null;

                target = enemyTargeting.GetTarget();
                var targetDistance = 0f;
                if (target != null) targetDistance = Vector3.Distance(target.GetPosition(), GetPosition());
                
                SetState(targetDistance);
                SetAnimation();
                
                switch (state)
                {
                    case State.Normal:
                        Wandering();
                        break;
                    case State.Chasing:
                        Chasing();
                        break;
                    case State.Attacking:
                        break;
                    case State.Busy:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SetState(float targetDistance)
        {
            if (target != null)
            {
                if (targetDistance > detectableRange)
                {
                    state = State.Normal;
                }
                else if (targetDistance > GetAttackRange())
                {
                    state = State.Chasing;
                }
                else
                {
                    state = State.Attacking;
                }
            }
            else
            {
                state = State.Normal;
            }
        }

        private void SetAnimation()
        {
            if (state == State.Attacking && !isAttackCool)
            {
                var dir = (target.GetPosition() - GetPosition()).normalized;
                enemyAnimationController.ChangeDirection(dir.x, dir.y);
                enemyAnimationController.ChangeAttackState(true, Attack, AttackCoolTime);   
            }
            else
            {
                enemyAnimationController.ChangeAttackState(false);
            }
        }

        private void Wandering()
        {
            if (isWanderCool) return;
            var dir = UtilsClass.GetRandomDir();
            var randomPosition = GetPosition() + dir * wanderRange;
            pathfindingMovement.MoveToTimer(randomPosition,1f);
            StartCoroutine(StartWanderCoolTime());
        }

        // TODO: 정확하게 안간다.. 오차 발생함.

        private void Chasing()
        {
            pathfindingMovement.MoveToTimer(target.GetPosition(), GetAttackRange(), true);
        }

        private void Attack()
        {
            if (isAttackCool) return;
            target.Interact(enemyMain.Enemy);
        }

        private void AttackCoolTime()
        {
            if (isAttackCool) return;
            StartCoroutine(StartAttackCoolTime());
        }

        private IEnumerator StartWanderCoolTime()
        {
            if (isWanderCool) yield break;
            isWanderCool = true;
            yield return new WaitForSeconds(wanderCoolTime);
            isWanderCool = false;
        }

        private IEnumerator StartAttackCoolTime()
        {
            if (isAttackCool) yield break;
            isAttackCool = true;
            yield return new WaitForSeconds(attackCoolTime);
            isAttackCool = false;
        }
        
        private float GetSpeed()
        {
            return enemyMain.EnemyStats.GetSpeed();
        }

        private Vector3 GetPosition()
        {
            return transform.position;
        }
        
        private float GetAttackRange()
        {
            return WeaponSystem.GetWeaponAttackRange(GetWeaponType());
        }
        
        private WeaponType GetWeaponType()
        {
            return enemyMain.WeaponSystem.GetWeaponType();
        }
    }
}