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
    /// <summary>
    /// 주인공을 보면 적들이 도망감 (쫓아가서 처치해야하는 미션)
    /// </summary>
    public class EnemyEscapeMoveStrategy: MonoBehaviour, IEnemyMovable
    {
        private enum State
        {
            Normal, Escape    
        }
        
        private State state;
        
        private EnemyMain enemyMain;
        private EnemyTargeting enemyTargeting;
        private EnemyAnimationController enemyAnimationController;
        private Rigidbody2D rb2D;
        
        private AIPath aiPath;
        private AIDestinationSetter destinationSetter;
        
        private Coroutine moveCoroutine;
        private Vector3 randomPosition;
        private const float wanderRange = 2f;
        private const float escapeRange = 2f;
        private const float wanderCoolTime = 3.5f;
        private bool isWanderCool = false;
        private float detectableRange;

        private bool isEscapeToNormal = false;
        private Vector3 recentEscapeDir = Vector3.zero;
        
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
            isWanderCool = false;
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (true)
            {
                yield return null;

                if (isEscapeToNormal)
                {
                    Debug.Log(recentEscapeDir);
                }

                var tempTarget = enemyTargeting.GetTarget();
                
                SetState(tempTarget, () =>
                {
                    randomPosition = GetPosition() + (isEscapeToNormal ? recentEscapeDir : UtilsClass.GetRandomDir()) * wanderRange;
                    isEscapeToNormal = false;
                });
                
                SetTarget(tempTarget);

                SetAnimation(tempTarget);

                switch (state)
                {
                    case State.Normal:
                        Wandering();
                        break;
                    case State.Escape:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Disable()
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);

            rb2D.velocity = Vector3.zero;

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
            return EnemyActionType.Escape;
        }

        private void SetState(Enemy.IEnemyInteractable tempTarget, System.Action onStateChangedCallback)
        {
            var targetDistance = 0f;
            if (tempTarget != null) targetDistance = Vector2.Distance(tempTarget.GetPosition(), GetPosition());

            var prevState = state;
            if (tempTarget != null)
            {
                state = targetDistance > detectableRange ? State.Normal : State.Escape;
            }
            else
            {
                state = State.Normal;
            }

            if (prevState != state)
            {
                if (prevState == State.Escape && state == State.Normal) isEscapeToNormal = true;
                onStateChangedCallback?.Invoke();
            }
        }

        private void SetTarget(Enemy.IEnemyInteractable tempTarget = null)
        {
            switch (state)
            {
                case State.Normal:
                    destinationSetter.SetTarget(randomPosition);
                    break;
                case State.Escape:
                    if (tempTarget != null) recentEscapeDir = (GetPosition() - tempTarget.GetPosition()).normalized;
                    destinationSetter.SetTarget(recentEscapeDir * escapeRange + GetPosition());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetAnimation(Enemy.IEnemyInteractable target)
        {
            switch (state)
            {
                case State.Normal:
                    var dir = (randomPosition - GetPosition()).normalized;
                    // 제자리에 있는 경우엔 안바꿔
                    if (dir.magnitude > 0.001f) enemyAnimationController.ChangeDirection(dir);
                    enemyAnimationController.ChangeMovingState(Vector3.Distance(randomPosition, GetPosition()) >= 0.1f);
                    break;
                case State.Escape:
                    // 도망가는거라 target 반대방향이니
                    enemyAnimationController.ChangeDirection(recentEscapeDir);
                    enemyAnimationController.ChangeMovingState();
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


        private Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}