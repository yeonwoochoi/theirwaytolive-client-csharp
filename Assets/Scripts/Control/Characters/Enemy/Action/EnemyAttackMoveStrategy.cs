using System;
using System.Collections;
using Control.Characters.Enemy.Base;
using Control.Characters.Enemy.Targeting;
using Control.Characters.Type;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using Util;

namespace Control.Characters.Enemy.Action
{
    public class EnemyAttackMoveStrategy: BaseEnemyMoveStrategy
    {
        private enum State
        {
            Normal, Chasing, Attacking, Busy
        }
        private State state;
        private WeaponType weaponType;

        private const float attackCoolTime = 2f;
        private bool isAttackCool = false;

        private Enemy.IEnemyInteractable target;
        
        private void Awake()
        {
            actionType = EnemyActionType.Attack;
        }

        public override void Init(float speed)
        {
            base.Init(speed);
            
            randomPosition = GetPosition() + UtilsClass.GetRandomDir() * wanderRange;

            state = State.Normal;
            weaponType = enemyMain.WeaponSystem.GetWeaponType();
            isAttackCool = false;
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }

        protected override void SetState(Enemy.IEnemyInteractable tempTarget, System.Action onStateChangedCallback = null)
        {
            base.SetState(tempTarget, onStateChangedCallback);
            
            // Calculate target distance for set state
            var targetDistance = 0f;
            if (tempTarget != null) targetDistance = Vector2.Distance(tempTarget.GetPosition(), GetPosition());
            
            var prevState = state;
            if (tempTarget != null)
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
            
            if (prevState != state) onStateChangedCallback?.Invoke();
        }

        protected override void SetTarget(Enemy.IEnemyInteractable targetEnemy)
        {
            base.SetTarget(targetEnemy);
            switch (state)
            {
                case State.Normal:
                    ResetTarget();
                    destinationSetter.SetTarget(randomPosition);
                    break;
                case State.Chasing:
                    target = targetEnemy;
                    destinationSetter.SetTarget(targetEnemy?.GetGameObject().transform);
                    break;
                case State.Attacking:
                    target = targetEnemy;
                    destinationSetter.SetTarget(targetEnemy?.GetGameObject().transform);
                    break;
                case State.Busy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void SetAnimation()
        {
            base.SetAnimation();
            Vector3 dir;
            switch (state)
            {
                case State.Normal:
                    dir = (randomPosition - GetPosition()).normalized;
                    if (dir.magnitude > 0.01f) enemyAnimationController.ChangeDirection(dir);
                    enemyAnimationController.ChangeMovingState(Vector3.Distance(randomPosition, GetPosition()) >= 0.1f);
                    enemyAnimationController.ChangeAttackState(false);
                    break;
                case State.Chasing:
                    dir = (target.GetPosition() - GetPosition()).normalized;
                    enemyAnimationController.ChangeDirection(dir);
                    enemyAnimationController.ChangeMovingState();
                    enemyAnimationController.ChangeAttackState(false);
                    break;
                case State.Attacking:
                    dir = (target.GetPosition() - GetPosition()).normalized;
                    enemyAnimationController.ChangeDirection(dir);
                    enemyAnimationController.ChangeMovingState(false);
                    enemyAnimationController.ChangeAttackState(!isAttackCool, Attack, AttackCoolTime);   
                    break;
                case State.Busy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void DoAction()
        {
            // Do action according to state
            switch (state)
            {
                case State.Normal:
                    Wandering();
                    break;
                case State.Chasing:
                    break;
                case State.Attacking:
                    break;
                case State.Busy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void ResetTarget()
        {
            base.ResetTarget();
            target = null;
            destinationSetter.target = null;
        }

        protected override void OnStateChangedCallback()
        {
            base.OnStateChangedCallback();
            aiPath.InitPath();
            
            var reachedDistance = state == State.Normal ? 0f : GetAttackRange();
            aiPath.slowdownDistance = reachedDistance + 1f;
            aiPath.endReachedDistance = reachedDistance;
        }

        private void Wandering()
        {
            if (isWanderCool) return;
            StartCoroutine(StartWanderCoolTime());
        }

        private void Attack()
        {
            if (isAttackCool || target == null) return;
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
            
            // Set new random position
            var randomDir = UtilsClass.GetRandomDir();
            randomPosition = GetPosition() + randomDir * wanderRange;

            isWanderCool = false;
        }

        private IEnumerator StartAttackCoolTime()
        {
            if (isAttackCool) yield break;
            isAttackCool = true;
            yield return new WaitForSeconds(attackCoolTime);
            isAttackCool = false;
        }

        private float GetAttackRange()
        {
            var prev = weaponType;
            weaponType = enemyMain.WeaponSystem.GetWeaponType();
            var attackRange = WeaponSystem.GetWeaponAttackRange(weaponType);
            if (state != State.Normal && prev != weaponType)
            {
                aiPath.slowdownDistance = attackRange + 1f;
                aiPath.endReachedDistance = attackRange;   
            }
            return attackRange;
        }
    }
}