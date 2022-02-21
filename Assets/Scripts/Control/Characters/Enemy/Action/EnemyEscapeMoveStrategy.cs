using System;
using System.Collections;
using Control.Characters.Enemy.Action.Base;
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

        private void Awake()
        {
            actionType = EnemyActionType.Escape;
        }

        public override void Init(float speed, float detectRange)
        {
            base.Init(speed, detectRange);
            
            enemyTargeting.Init(DetectModeType.Circle, detectableRange, () => moveDir);
            
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
                    if (tempTarget != null) moveDir = (GetPosition() - tempTarget.GetPosition()).normalized;
                    destinationSetter.SetTarget(moveDir * escapeRange + GetPosition());
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
                    moveDir = (randomPosition - GetPosition()).normalized;
                    // 제자리에 있는 경우엔 안바꿔
                    if (moveDir.magnitude > 0.001f) enemyAnimationController.ChangeDirection(moveDir);
                    enemyAnimationController.ChangeMovingState(Vector3.Distance(randomPosition, GetPosition()) >= 0.1f);
                    break;
                case State.Escape:
                    // 도망가는거라 target 반대방향이니
                    enemyAnimationController.ChangeDirection(moveDir);
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
        
        protected override void ResetTarget()
        {
            base.ResetTarget();
            destinationSetter.target = null;
            enemyTargeting.Disable();
        }

        protected override void OnStateChangedCallback()
        {
            base.OnStateChangedCallback();
            randomPosition = GetPosition() + (isEscapeToNormal ? moveDir : UtilsClass.GetRandomDir()) * wanderRange;
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