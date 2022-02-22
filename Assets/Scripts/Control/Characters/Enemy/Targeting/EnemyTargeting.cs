using System;
using System.Collections;
using Control.Characters.Type;
using UnityEngine;
using Util;

namespace Control.Characters.Enemy.Targeting
{
    public class EnemyTargeting: MonoBehaviour
    {
        private Enemy.IEnemyInteractable activeEnemyTarget;
        private Func<Enemy.IEnemyInteractable, bool> isTargetableObjectTypeFunc; 
        private Func<Vector3> getDirectionFunc; 
        private System.Action findTargetInArea;
        private Coroutine detectCoroutine;

        private float detectableRange;
        
        public void Init(DetectModeType type, float detectableRange, Func<Vector3> getDirection)
        {
            enabled = true;
            if (detectCoroutine != null) StopCoroutine(detectCoroutine);
            detectCoroutine = StartCoroutine(FindTarget());
            isTargetableObjectTypeFunc = GetComponent<EnemyMain>().Enemy.IsTargetable;
            this.detectableRange = detectableRange;
            getDirectionFunc = getDirection;
            SetDetectMode(type);
        }

        public Enemy.IEnemyInteractable GetTarget()
        {
            return activeEnemyTarget;
        }

        public void Disable()
        {
            if (detectCoroutine != null) StopCoroutine(detectCoroutine);
            activeEnemyTarget = null;
            enabled = false;
        }
        
        /// <summary>
        /// 이걸 호출하면 Detect Mode가 바뀜
        /// </summary>
        /// <param name="type"></param>
        public void ChangeDetectMode(DetectModeType type, float detectableRange)
        {
            activeEnemyTarget = null;
            this.detectableRange = detectableRange;
            SetDetectMode(type);
        }

        private IEnumerator FindTarget()
        {
            while (true)
            {
                yield return null;
                CheckEnemyDead();
                if (Hero.Hero.heroList.Count <= 0) continue;
                findTargetInArea?.Invoke();
            }
        }

        private void CheckEnemyDead()
        {
            if (activeEnemyTarget != null && activeEnemyTarget.IsDead())
            {
                activeEnemyTarget = null;
            }
        }

        private void SetDetectMode(DetectModeType type)
        {
            findTargetInArea = type switch
            {
                DetectModeType.Circle => FindTargetInCircleArea,
                DetectModeType.Sector => FindTargetInSectorArea,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private Vector3 GetPosition()
        {
            return transform.position;
        }

        private void FindTargetInCircleArea()
        {
            if (Hero.Hero.heroList.Count <= 0) return;
            foreach (var target in Hero.Hero.heroList)
            {
                if (target.IsDead()) continue;
                if (!isTargetableObjectTypeFunc(target)) continue;
                if (Vector3.Distance(GetPosition(), target.GetPosition()) < detectableRange)
                {
                    if (activeEnemyTarget == null)
                    {
                        activeEnemyTarget = target;
                    }
                    else
                    {
                        if (Vector3.Distance(GetPosition(),activeEnemyTarget.GetPosition()) > 
                            Vector3.Distance(GetPosition(), target.GetPosition()))
                        {
                            activeEnemyTarget = target;
                        }
                    }
                }
            }
        }

        private void FindTargetInSectorArea()
        {
            if (Hero.Hero.heroList.Count <= 0) return;
            foreach (var target in Hero.Hero.heroList)
            {
                if (target.IsDead()) continue;
                if (!isTargetableObjectTypeFunc(target)) continue;
                
                var dir = getDirectionFunc();
                var angle = UtilsClass.GetAngleFromVectorFloat((target.GetPosition() - GetPosition()).normalized);
                var direction = UtilsClass.GetMoveDirectionFromVector(dir);

                var isInAngleRange = false;

                switch (direction)
                {
                    case Direction.Left:
                        isInAngleRange = (angle >= 135 && angle <= 225); 
                        break;
                    case Direction.Right:
                        isInAngleRange = (angle >= 0 && angle <= 45) || (angle >= 315 && angle <= 360);
                        break;
                    case Direction.Up:
                        isInAngleRange = (angle >= 45 && angle <= 135);
                        break;
                    case Direction.Down:
                        isInAngleRange = (angle >= 225 && angle <= 315);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!isInAngleRange) continue;

                if (Vector3.Distance(GetPosition(), target.GetPosition()) <= detectableRange)
                {
                    if (activeEnemyTarget == null)
                    {
                        activeEnemyTarget = target;
                    }
                    else
                    {
                        if (Vector3.Distance(GetPosition(),activeEnemyTarget.GetPosition()) > 
                            Vector3.Distance(GetPosition(), target.GetPosition()))
                        {
                            activeEnemyTarget = target;
                        }
                    }
                }
            }
        }
    }
}