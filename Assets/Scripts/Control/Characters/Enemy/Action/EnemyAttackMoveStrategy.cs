using System;
using System.Collections;
using Control.Characters.Enemy.Targeting;
using Control.Characters.Type;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using Util;

namespace Control.Characters.Enemy.Action
{
    public class EnemyAttackMoveStrategy: MonoBehaviour, IEnemyMovable
    {
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
        private WeaponType weaponType;

        private Coroutine moveCoroutine;
        private Vector3 randomPosition;
        private const float wanderRange = 2f;
        private const float wanderCoolTime = 3.5f;
        private const float attackCoolTime = 2f;
        private bool isWanderCool = false;
        private bool isAttackCool = false;
        private float detectableRange;

        private Enemy.IEnemyInteractable target;

        public void Init(float speed = 2f)
        {
            enemyMain = GetComponent<EnemyMain>();
            
            enemyAnimationController = GetComponent<EnemyAnimationController>();
            enemyTargeting = GetComponent<EnemyTargeting>();
            
            aiPath = TryGetComponent<AIPath>(out var pathController)
                ? pathController
                : gameObject.AddComponent<AIPath>();
            destinationSetter = TryGetComponent<AIDestinationSetter>(out var aiDestination)
                ? aiDestination
                : gameObject.AddComponent<AIDestinationSetter>();

            aiPath.enabled = true;
            destinationSetter.enabled = true;
            
            aiPath.orientation = OrientationMode.YAxisForward;
            aiPath.radius = 0.5f;
            aiPath.gravity = Vector3.zero;
            aiPath.slowdownDistance = 1f;
            aiPath.endReachedDistance = 0f;
            aiPath.maxSpeed = speed;
            aiPath.enableRotation = false;

            rb2D = GetComponent<Rigidbody2D>();
            
            randomPosition = GetPosition() + UtilsClass.GetRandomDir() * wanderRange;

            detectableRange = EnemyTargeting.detectableRange;
            state = State.Normal;
            weaponType = enemyMain.WeaponSystem.GetWeaponType();
            isWanderCool = false;
            isAttackCool = false;
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }

        public void Disable()
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);

            rb2D.velocity = Vector3.zero;

            ResetTarget();
            aiPath.InitPath();

            aiPath.enabled = false;
            destinationSetter.enabled = false;

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

        public EnemyActionType GetEnemyActionType()
        {
            return EnemyActionType.Attack;
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

        private void SetState(Enemy.IEnemyInteractable tempTarget, System.Action onStateChangedCallback)
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