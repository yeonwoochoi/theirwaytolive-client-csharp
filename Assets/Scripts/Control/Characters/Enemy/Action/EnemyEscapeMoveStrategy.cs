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
    /// <summary>
    /// 주인공을 보면 적들이 도망감 (쫓아가서 처치해야하는 미션)
    /// </summary>
    public class EnemyEscapeMoveStrategy: BaseEnemyMoveStrategy
    {
        private enum State
        {
            Normal, Escape    
        }
        
        private State state;
        
        private const float escapeRange = 2f;
        private bool isEscapeToNormal = false;
        private Vector3 recentEscapeDir = Vector3.zero;

        private void Awake()
        {
            actionType = EnemyActionType.Escape;
        }

        public override void Init(float speed)
        {
            base.Init(speed);
            
            randomPosition = GetPosition() + UtilsClass.GetRandomDir() * wanderRange;
            state = State.Normal;

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }


        protected override void SetState(Enemy.IEnemyInteractable tempTarget, System.Action onStateChangedCallback = null)
        {
            base.SetState(tempTarget, onStateChangedCallback);
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

        protected override void SetTarget(Enemy.IEnemyInteractable tempTarget)
        {
            base.SetTarget(tempTarget);
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

        protected override void SetAnimation()
        {
            base.SetAnimation();
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

        protected override void DoAction()
        {
            base.DoAction();
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

        protected override void OnStateChangedCallback()
        {
            base.OnStateChangedCallback();
            randomPosition = GetPosition() + (isEscapeToNormal ? recentEscapeDir : UtilsClass.GetRandomDir()) * wanderRange;
            isEscapeToNormal = false;
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
    }
}