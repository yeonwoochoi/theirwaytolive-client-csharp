using System;
using System.Collections;
using Control.Characters.Base;
using Control.Characters.Enemy;
using Control.Characters.Type;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using UnityEngine.Rendering;
using Util;

namespace Control.Characters.Hero.Control.Strategies
{
    public class HeroNpcControlStrategy: MonoBehaviour, IHeroMovable
    {
        public void Init(float speed = 0)
        {
            throw new NotImplementedException();
        }

        private IEnumerator Move()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public HeroControlType GetHeroControlType()
        {
            return HeroControlType.Disable;
        }

        public Direction GetMoveDirection()
        {
            throw new NotImplementedException();
        }
    }
}