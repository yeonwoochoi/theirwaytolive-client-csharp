using System;
using System.Collections;
using System.Collections.Generic;
using Control.Characters.Enemy.Base;
using Control.Characters.Enemy.Targeting;
using Control.Characters.Type;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Control.Characters.Enemy.Action
{
    public class EnemyDetectMoveStrategy: BaseEnemyMoveStrategy
    {
        private enum State
        {
            Normal, Detect
        }
        private State state;
        
        private Vector3 initPosition;
        private List<Vector3> movablePositions;

        private void Awake()
        {
            actionType = EnemyActionType.Detect;
        }

        public override void Init(float speed)
        {
            base.Init(speed);

            wanderRange = 4f;
            wanderCoolTime = 3.5f;
            
            InitMovablePositions();
            
            state = State.Normal;
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }

        protected override void SetState(Enemy.IEnemyInteractable tempTarget, System.Action onStateChangedCallback = null)
        {
            base.SetState(tempTarget, onStateChangedCallback);
            // Calculate target distance for set state
            var targetDistance = 0f;
            if (tempTarget != null) targetDistance = Vector2.Distance(tempTarget.GetPosition(), GetPosition());
            
            if (tempTarget != null)
            {
                if (targetDistance > detectableRange)
                {
                    state = State.Normal;
                }
                else
                {
                    state = State.Detect;
                }
            }
            else
            {
                state = State.Normal;
            }
            
        }

        protected override void SetTarget(Enemy.IEnemyInteractable targetEnemy)
        {
            switch (state)
            {
                case State.Normal:
                    SetRandomPosition();
                    destinationSetter.SetTarget(randomPosition);
                    break;
                case State.Detect:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void SetAnimation()
        {
            base.SetAnimation();
            switch (state)
            {
                case State.Normal:
                    var dir = (randomPosition - GetPosition()).normalized;
                    if (dir.magnitude > 0.01f) enemyAnimationController.ChangeDirection(dir);
                    enemyAnimationController.ChangeMovingState(Vector3.Distance(randomPosition, GetPosition()) >= 0.1f);
                    break;
                case State.Detect:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void DoAction()
        {
            base.DoAction();
            switch (state)
            {
                case State.Normal:
                    Wandering();
                    break;
                case State.Detect:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnStateChangedCallback()
        {
            base.OnStateChangedCallback();
        }

        protected override void ResetTarget()
        {
            base.ResetTarget();
            
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

        private void InitMovablePositions()
        {
            movablePositions = new List<Vector3>();
            var minX = (int) (GetPosition().x - wanderRange);
            var minY = (int) (GetPosition().y - wanderRange);
            var maxX = (int) (GetPosition().x + wanderRange);
            var maxY = (int) (GetPosition().y + wanderRange);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    movablePositions.Add(new Vector3(x, y));
                }
            }
        }

        private void SetRandomPosition()
        {
            while (true)
            {
                var randomIndex = Random.Range(0, movablePositions.Count);
                var tempPosition = movablePositions[randomIndex];
                if (Vector3.Distance(tempPosition, GetPosition()) < 0.001f) continue;
                randomPosition = tempPosition;
                break;
            }
        }
    }
}