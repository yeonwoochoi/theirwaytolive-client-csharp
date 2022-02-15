using System;
using System.Collections.Generic;
using Control.Characters.Hero;
using UnityEngine;
using Logger = Util.Logger;

namespace Control.Characters.Base
{
    public enum Direction
    {
        Left, Right, Up, Down
    }
    
    public abstract class BasePathFindingMovement: MonoBehaviour
    {
        protected bool isSet = false;
        protected float speed;
        protected Rigidbody2D rb2D;
        protected Action<bool> changeAnimationMovingState;
        protected Action<Vector3> changeAnimationDirection;
        protected Func<Vector3> getPositionFunc;
        protected Func<float, float> getReachedTargetDistance;

        private List<Vector3> pathVectorList;
        private int currentPathIndex;
        private float pathfindingTimer;
        private float reachedTargetDistance;
        private Vector3 moveDir;
        private Vector3 lastMoveDir;
        private Direction direction;
        private GridPathfinding.GridPathfinding gridPathfinding;

        public virtual void Init(float speed)
        {
            if (isSet) return;
            this.speed = speed;
            direction = Direction.Down;
            Vector3 pathfindingLowerLeft = GameObject.Find("PathfindingLowerLeft").transform.position;
            Vector3 pathfindingUpperRight = GameObject.Find("PathfindingUpperRight").transform.position;
        
            gridPathfinding = new GridPathfinding.GridPathfinding(pathfindingLowerLeft, pathfindingUpperRight, 2f);
            // (+1) << 9 : 9번 레이어만 감지
            // (-1) << 9 : 9번 레이어 빼고 다 감지
            // 밑에 내용은 9, 10, 11번 레이어만 감지
            gridPathfinding.RaycastWalkable(1 << 9 | 1 << 10 | 1 << 11);
        }

        private void Update() {
            if (!isSet) return;
            pathfindingTimer -= Time.deltaTime;
            HandleMovement();
        }

        private void FixedUpdate() {
            if (!isSet) return;
            rb2D.velocity = moveDir * speed;
        }

        private void HandleMovement() {
            PrintPathfindingPath();
            if (pathVectorList != null) {
                Vector3 targetPosition = pathVectorList[currentPathIndex];
                if (Vector3.Distance(getPositionFunc(), targetPosition) >= reachedTargetDistance) {
                    moveDir = (targetPosition - getPositionFunc()).normalized;
                    lastMoveDir = moveDir;
                    
                    StartMoving();
                    changeAnimationDirection?.Invoke(moveDir);
                    SetMoveDirection(moveDir);
                    changeAnimationMovingState?.Invoke(true);
                } else {
                    currentPathIndex++;
                    if (currentPathIndex >= pathVectorList.Count) {
                        StopMoving();
                        changeAnimationMovingState?.Invoke(false);
                    }
                }
            } else {
                changeAnimationMovingState?.Invoke(false);
            }
        }

        private void StopMoving() {
            pathVectorList = null;
            moveDir = Vector3.zero;
            rb2D.velocity = Vector3.zero;
        }
    
        private void StartMoving()
        {
            rb2D.velocity = moveDir * speed * Time.fixedDeltaTime;    
        }

        public List<Vector3> GetPathVectorList() {
            return pathVectorList;
        }

        private void PrintPathfindingPath() {
            if (pathVectorList != null) {
                for (int i = 0; i<pathVectorList.Count - 1; i++) {
                    Debug.DrawLine(pathVectorList[i], pathVectorList[i + 1]);
                }
            }
        }

        public void MoveToTimer(Vector3 targetPosition, float targetDistance = 5f, bool isChasing = false) {
            if (pathfindingTimer <= 0f) {
                SetTargetPosition(targetPosition, targetDistance, isChasing);
            }
        }

        private void SetTargetPosition(Vector3 targetPosition, float targetDistance, bool isChasing) {
            currentPathIndex = 0;
            reachedTargetDistance = targetDistance;
            if (isChasing)
            {
                // TODO: 임시방편으로 target과 유지해야할 거리 임계값을 더 낮게 잡음
                reachedTargetDistance = getReachedTargetDistance(reachedTargetDistance);
            }
            pathVectorList = GridPathfinding.GridPathfinding.instance.GetPathRouteWithShortcuts(getPositionFunc(), targetPosition).pathVectorList;
            pathfindingTimer = .1f;

            if (pathVectorList != null && pathVectorList.Count > 1) {
                pathVectorList.RemoveAt(0);
            }
        }

        public Vector3 GetLastMoveDir() {
            return lastMoveDir;
        }
        
        public Direction GetMoveDirection()
        {
            return direction;
        }
        
        private void SetMoveDirection(Vector3 dir)
        {
            dir *= 10f;
            direction = GetDirectionFromVector(dir.x, dir.y);
        }

        private Direction GetDirectionFromVector(float x, float y)
        {
            if (Math.Abs(x) > Math.Abs(y))
            {
                return x >= 0 ? Direction.Right : Direction.Left;
            }

            return y > 0 ? Direction.Up : Direction.Down;
        }
    
        public void Enable() {
            enabled = true;
            isSet = true;
        }

        public void Disable() {
            isSet = false;
            enabled = false;
            rb2D.velocity = Vector3.zero;
        }
    }
}