using System.Collections;
using Control.Characters.Enemy.Action;
using Control.Characters.Enemy.Targeting;
using Control.Characters.Type;
using Pathfinding;
using UnityEngine;

namespace Control.Characters.Enemy.Base
{
    public abstract class BaseEnemyMoveStrategy: MonoBehaviour, IEnemyMovable
    {
        protected EnemyMain enemyMain;
        protected EnemyTargeting enemyTargeting;
        protected EnemyAnimationController enemyAnimationController;
        protected Rigidbody2D rb2D;

        protected AIPath aiPath;
        protected AIDestinationSetter destinationSetter;

        protected EnemyActionType actionType;
        
        protected Coroutine moveCoroutine;
        protected Vector3 randomPosition;
        
        protected float wanderRange = 2f;
        protected float wanderCoolTime = 3.5f;
        protected bool isWanderCool = false;
        protected float detectableRange;

        public virtual void Init(float speed)
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
            detectableRange = enemyTargeting.GetDetectableRange();
            isWanderCool = false;
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

        public virtual EnemyActionType GetEnemyActionType()
        {
            return actionType;
        }

        protected IEnumerator Move()
        {
            while (true)
            {
                yield return null;

                // Set target
                var tempTarget = enemyTargeting.GetTarget();

                // Set current state
                SetState(tempTarget, OnStateChangedCallback);

                // Set target or targetPosition
                // 여기서 설정하면 pathfinding 해서 그쪽으로 이동한다.
                SetTarget(tempTarget);
                
                // Set animation according to state
                // target을 설정한 후에 와야함
                SetAnimation();

                // Do action according to state
                DoAction();
            }
        }
        
        protected virtual void SetState(Enemy.IEnemyInteractable tempTarget, System.Action onStateChangedCallback = null) {}

        protected virtual void SetTarget(Enemy.IEnemyInteractable tempTarget) { }
        protected virtual void SetAnimation() {}
        protected virtual void DoAction() {}
        protected virtual void OnStateChangedCallback() {}
        protected virtual void ResetTarget() {}

        protected Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}