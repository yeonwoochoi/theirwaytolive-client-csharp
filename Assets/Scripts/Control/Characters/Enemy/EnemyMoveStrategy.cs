using System;
using System.Collections;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using Util;

namespace Control.Characters.Enemy
{
    public class EnemyMoveStrategy: MonoBehaviour
    {
        private bool isSet = false;

        private EnemyMain enemyMain;
        private EnemyTargeting enemyTargeting;
        private EnemyAnimationController enemyAnimationController;
        private Rigidbody2D rb2D;
        
        private AIPath aiPath;
        private AIDestinationSetter destinationSetter;
        
        private enum State
        {
            Normal, Chasing, Attacking, Busy
        }
        private State state;

        private Coroutine moveCoroutine;
        private Vector3 randomPosition;
        private const float wanderRange = 2f;
        private const float wanderCoolTime = 3.5f;
        private const float attackCoolTime = 2f;
        private bool isWanderCool = false;
        private bool isAttackCool = false;
        private float detectableRange;

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
            
            aiPath = TryGetComponent<AIPath>(out var pathController)
                ? pathController
                : gameObject.AddComponent<AIPath>();
            destinationSetter = TryGetComponent<AIDestinationSetter>(out var aiDestination)
                ? aiDestination
                : gameObject.AddComponent<AIDestinationSetter>();

            aiPath.orientation = OrientationMode.YAxisForward;
            aiPath.radius = 0.5f;
            aiPath.gravity = Vector3.zero;
            aiPath.slowdownDistance = 1f;
            aiPath.endReachedDistance = 0f;
            aiPath.maxSpeed = GetSpeed();
            aiPath.enableRotation = false;

            rb2D = GetComponent<Rigidbody2D>();
            
            randomPosition = GetPosition() + UtilsClass.GetRandomDir() * wanderRange;

            enemyTargeting.Init();
            enemyAnimationController.Init();
            
            detectableRange = EnemyTargeting.detectableRange;
            state = State.Normal;
            isWanderCool = false;
            isAttackCool = false;
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());

            isSet = true;
        }

        public void Disable()
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);

            rb2D.velocity = Vector3.zero;

            ResetTarget();
            aiPath.InitPath();

            aiPath.enabled = false;
            destinationSetter.enabled = false;
            
            enemyTargeting.Disable();
            
            if (enemyMain.Enemy.IsDead())
            {
                enemyAnimationController.ChangeDeathState(() => {}, enemyMain.EnemyEffectController.OnDead);
            }
            else
            {
                enemyAnimationController.ChangeDirection(0, 1);
                enemyAnimationController.ChangeMovingState(false);
            }
        }
        
        public void ChangeWeapon(WeaponType type)
        {
            if (!isSet) return;
            enemyAnimationController.ChangeWeapon(type);
            if (state != State.Normal)
            {
                aiPath.slowdownDistance = GetAttackRange() + 1f;
                aiPath.endReachedDistance = GetAttackRange();   
            }
        }

        private IEnumerator Move()
        {
            while (true)
            {
                yield return null;

                // Set target
                var tempTarget = enemyTargeting.GetTarget();

                // Set current state
                SetState(tempTarget, () =>
                {
                    aiPath.InitPath();
            
                    var reachedDistance = state == State.Normal ? 0f : GetAttackRange();
                    aiPath.slowdownDistance = reachedDistance + 1f;
                    aiPath.endReachedDistance = reachedDistance;
                });

                // Set target or targetPosition
                // 여기서 설정하면 pathfinding 해서 그쪽으로 이동한다.
                SetTarget(tempTarget);
                
                // Set animation according to state
                // target을 설정한 후에 와야함
                SetAnimation();

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
        }

        private void SetState(Enemy.IEnemyInteractable tempTarget, Action onStateChangedCallback)
        {
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

        private void SetAnimation()
        {
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

        private void Wandering()
        {
            if (isWanderCool) return;
            StartCoroutine(StartWanderCoolTime());
        }
        
        private void Attack()
        {
            if (isAttackCool || target == null) return;
            target.Interact(enemyMain.Enemy);
            // Knockback 효과같은게 있으면 공격과 동시에 적과 거리가 멀어져서
            // State가 chasing으로 바뀌게 되고 중간에 animation이 다 실행되지 않고 끊긴다
            // 그래서 OnEndEvent가 실행되지 않는 경우가 생겨 연속공격을 하기 시작함. => 그래서 여기다 Cooltime
            AttackCoolTime();
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

        private float GetSpeed()
        {
            return enemyMain.EnemyStats.GetSpeed();
        }
        
        private float GetAttackRange()
        {
            return WeaponSystem.GetWeaponAttackRange(enemyMain.WeaponSystem.GetWeaponType());
        }

        private void SetTarget(Enemy.IEnemyInteractable targetEnemy = null)
        {
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

        private void ResetTarget()
        {
            target = null;
            destinationSetter.target = null;
        }

        private Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}