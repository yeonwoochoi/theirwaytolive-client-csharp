using System;
using Control.Characters.Base;
using Control.Characters.Type;
using UnityEngine;
using Util;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Control.Weapon
{
    public class WeaponTargetArea
    {
        private Transform heroTransform;
        private float attackRange;
        private Direction direction;

        public WeaponTargetArea(Transform heroTransform)
        {
            this.heroTransform = heroTransform;
        }

        public Func<Vector3, bool> IsTargetable(WeaponType weaponType, Direction dir)
        {
            direction = dir;
            SetAttackRangeFromWeapon(weaponType);

            switch (weaponType)
            {
                case WeaponType.Arrow:
                    return IsArrowAttackArea;
                case WeaponType.Spear:
                    return IsSpearAttackArea;
                case WeaponType.Sword:
                    return IsSwordAttackArea;
                case WeaponType.None:
                    return IsPunchAttackArea;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // TODO (무기 사정거리) : 나중에 합치기
        private void SetAttackRangeFromWeapon(WeaponType weaponType)
        {
            attackRange = WeaponSystem.GetWeaponAttackRange(weaponType, true);
        }

        private Vector3 GetPosition()
        {
            return heroTransform.position;
        }

        private bool IsSpearAttackArea(Vector3 target)
        {
            var leftBottomPoint = new Vector2();
            var rightTopPoint = new Vector2();
            var range = 1f;

            switch (direction)
            {
                case Direction.Left:
                    leftBottomPoint = new Vector2(GetPosition().x - attackRange, GetPosition().y - range);
                    rightTopPoint = new Vector2(GetPosition().x, GetPosition().y + range);
                    break;
                case Direction.Right:
                    leftBottomPoint = new Vector2(GetPosition().x, GetPosition().y - range);
                    rightTopPoint = new Vector2(GetPosition().x + attackRange, GetPosition().y + range);
                    break;
                case Direction.Up:
                    leftBottomPoint = new Vector2(GetPosition().x - range, GetPosition().y);
                    rightTopPoint = new Vector2(GetPosition().x + range, GetPosition().y + attackRange);
                    break;
                case Direction.Down:
                    leftBottomPoint = new Vector2(GetPosition().x - range, GetPosition().y - attackRange);
                    rightTopPoint = new Vector2(GetPosition().x + range, GetPosition().y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //Debug.Log($"{direction}: {leftBottomPoint} ~ {rightTopPoint}");

            return (target.x > leftBottomPoint.x && target.x < rightTopPoint.x) &&
                   (target.y > leftBottomPoint.y && target.y < rightTopPoint.y);
        }
        private bool IsArrowAttackArea(Vector3 target)
        {
            Vector2 leftBottomPoint;
            Vector2 rightTopPoint;
            var range = 2f;

            switch (direction)
            {
                case Direction.Left:
                    leftBottomPoint = new Vector2(GetPosition().x - attackRange, GetPosition().y - range);
                    rightTopPoint = new Vector2(GetPosition().x, GetPosition().y + range);
                    break;
                case Direction.Right:
                    leftBottomPoint = new Vector2(GetPosition().x, GetPosition().y - range);
                    rightTopPoint = new Vector2(GetPosition().x + attackRange, GetPosition().y + range);
                    break;
                case Direction.Up:
                    leftBottomPoint = new Vector2(GetPosition().x - range, GetPosition().y);
                    rightTopPoint = new Vector2(GetPosition().x + range, GetPosition().y + attackRange);
                    break;
                case Direction.Down:
                    leftBottomPoint = new Vector2(GetPosition().x - range, GetPosition().y - attackRange);
                    rightTopPoint = new Vector2(GetPosition().x + range, GetPosition().y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Debug.Log($"{direction}: {leftBottomPoint} ~ {rightTopPoint}");

            return (target.x > leftBottomPoint.x && target.x < rightTopPoint.x) &&
                   (target.y > leftBottomPoint.y && target.y < rightTopPoint.y);
        }
        private bool IsSwordAttackArea(Vector3 target)
        {
            var dir = (target - GetPosition()).normalized;
            var angle = UtilsClass.GetAngleFromVectorFloat(dir);

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
            
            Debug.Log($"{direction}: {angle} => {isInAngleRange}");

            if (!isInAngleRange) return false;

            var isInDistanceRange = Vector3.Distance(GetPosition(), target) <= attackRange;
            return isInDistanceRange;
        }
        private bool IsPunchAttackArea(Vector3 target)
        {
            var leftBottomPoint = new Vector2();
            var rightTopPoint = new Vector2();
            var range = 1f;

            switch (direction)
            {
                case Direction.Left:
                    leftBottomPoint = new Vector2(GetPosition().x - attackRange, GetPosition().y - range);
                    rightTopPoint = new Vector2(GetPosition().x, GetPosition().y + range);
                    break;
                case Direction.Right:
                    leftBottomPoint = new Vector2(GetPosition().x, GetPosition().y - range);
                    rightTopPoint = new Vector2(GetPosition().x + attackRange, GetPosition().y + range);
                    break;
                case Direction.Up:
                    leftBottomPoint = new Vector2(GetPosition().x - range, GetPosition().y);
                    rightTopPoint = new Vector2(GetPosition().x + range, GetPosition().y + attackRange);
                    break;
                case Direction.Down:
                    leftBottomPoint = new Vector2(GetPosition().x - range, GetPosition().y - attackRange);
                    rightTopPoint = new Vector2(GetPosition().x + range, GetPosition().y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            //Debug.Log($"{direction}: {leftBottomPoint} ~ {rightTopPoint}");

            return (target.x > leftBottomPoint.x && target.x < rightTopPoint.x) &&
                   (target.y > leftBottomPoint.y && target.y < rightTopPoint.y);
        }
    }
}