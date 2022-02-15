using System;
using Control.Characters.Base;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Hero
{
    public class HeroAnimationController: BaseCharacterAnimationController
    {
        public override void Init()
        {
            base.Init();
            isSet = true;
        }
    }
}