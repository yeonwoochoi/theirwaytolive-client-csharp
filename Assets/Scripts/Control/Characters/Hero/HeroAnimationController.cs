using System;
using Control.Characters.Base;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Hero
{
    public class HeroAnimationController: BaseCharacterAnimationController
    {
        public override void Init(bool isSpawned = false)
        {
            base.Init(isSpawned);
            if (isSpawned) StartSpawnAnimation();
            ChangeDirection(Vector3.down);
            isSet = true;
        }
    }
}