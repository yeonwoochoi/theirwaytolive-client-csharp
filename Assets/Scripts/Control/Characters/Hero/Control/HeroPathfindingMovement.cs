using System;
using Control.Characters.Base;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Hero.Control
{
    public class HeroPathfindingMovement: BasePathFindingMovement
    {
        private HeroAnimationController heroAnimationController;
        private HeroMain heroMain;
        public override void Init(float speed)
        {
            base.Init(speed);
            heroAnimationController = GetComponent<HeroAnimationController>();
            rb2D = GetComponent<Rigidbody2D>();
            heroMain = GetComponent<HeroMain>();
            changeAnimationMovingState = flag => heroAnimationController.ChangeMovingState(flag);
            changeAnimationDirection = offset => heroAnimationController.ChangeDirection(offset);
            getPositionFunc = () => transform.position;
            getReachedTargetDistance = GetReachedTargetDistance;
            isSet = true;
        }
        
        private float GetReachedTargetDistance(float prev)
        {
            var flag = GetWeaponType() switch
            {
                WeaponType.Arrow => 0.5f,
                WeaponType.Spear => 1.5f,
                WeaponType.Sword => 1f,
                WeaponType.None => 1f,
                _ => throw new ArgumentOutOfRangeException()
            };
            return prev - flag;
        }

        private WeaponType GetWeaponType()
        {
            return heroMain.WeaponSystem.GetWeaponType();
        }
    }
}