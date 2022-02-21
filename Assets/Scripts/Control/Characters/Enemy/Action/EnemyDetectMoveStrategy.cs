using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Control.Characters.Enemy.Action.Base;
using Control.Characters.Enemy.Targeting;
using Control.Characters.Type;
using Control.Stuff;
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

        private Enemy.IEnemyInteractable target;

        private FieldOfView fieldOfView;

        private void Awake()
        {
            actionType = EnemyActionType.Detect;
        }

        public override void Init(float speed, float detectRange)
        {
            base.Init(speed, detectRange);
            
            enemyTargeting.Init(DetectModeType.Sector, detectableRange, () => moveDir);

            wanderRange = 2f;
            wanderCoolTime = 5f;
            
            InitMovablePositions();
            SetRandomPosition();
            
            state = State.Normal;

            moveDir = (randomPosition - GetPosition()).normalized;
            
            // field of view 설정
            fieldOfView = Instantiate(GameAssets.i.pfFieldOfView, GetPosition(), Quaternion.identity, transform).GetComponent<FieldOfView>();
            fieldOfView.Init(Vector3.zero, detectableRange, moveDir);

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }

        protected override void SetState(Enemy.IEnemyInteractable tempTarget, System.Action onStateChangedCallback = null)
        {
            base.SetState(tempTarget, onStateChangedCallback);

            var prevState = state; 
            
            // Calculate target distance for set state
            var targetDistance = 0f;
            if (tempTarget != null) targetDistance = Vector2.Distance(tempTarget.GetPosition(), GetPosition());
            
            if (tempTarget != null)
            {
                state = targetDistance > detectableRange ? State.Normal : State.Detect;
            }
            else
            {
                state = State.Normal;
            }

            if (prevState == State.Normal && state == State.Detect)
            {
                // TODO (EnemyDetectMoveStrategy): strategy 바꾸는 방식 다시 생각해보기
                foreach (var enemy in Enemy.enemyList.Where(enemy => !enemy.IsDead()))
                {
                    enemy.GetGameObject().GetComponent<EnemyActionStrategySelector>()?.SetControlStrategy(EnemyActionType.Attack, 1000f);
                }
            }
        }

        protected override void SetTarget(Enemy.IEnemyInteractable targetEnemy)
        {
            switch (state)
            {
                case State.Normal:
                    destinationSetter.SetTarget(randomPosition);
                    break;
                case State.Detect:
                    // attack move strategy로 넘어가니까
                    // target = targetEnemy;
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
                    if (moveDir.magnitude > 0.1f)
                    {
                        enemyAnimationController.ChangeDirection(moveDir);
                        fieldOfView.SetAimDirection(moveDir);
                    }
                    enemyAnimationController.ChangeMovingState(Vector3.Distance(randomPosition, GetPosition()) >= 0.1f);
                    break;
                case State.Detect:
                    // attack move strategy로 넘어가니까
                    /*
                    moveDir = (target.GetPosition() - GetPosition()).normalized;
                    if (moveDir.magnitude > 0.1f)
                    {
                        enemyAnimationController.ChangeDirection(moveDir);
                        fieldOfView.SetAimDirection(moveDir);
                    }
                    fieldOfView.SetColor(new Color(1f, 0, 0));
                    */
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Disable()
        {
            fieldOfView.Disable();
            base.Disable();
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
        
        protected override void ResetTarget()
        {
            base.ResetTarget();
            target = null;
            destinationSetter.target = null;
            enemyTargeting.Disable();
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
            SetRandomPosition();

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
            if (movablePositions == null) InitMovablePositions();
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