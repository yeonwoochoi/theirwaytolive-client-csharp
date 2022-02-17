using System.Collections;
using Control.Characters.Base;
using Control.Characters.Type;
using UnityEngine;

namespace Control.Characters.Hero.Control.Strategies
{
    public interface IHeroMovable
    {
        public void Init(float speed = 0f);
        public void Disable();
        public Direction GetMoveDirection();
        public HeroControlType GetHeroControlType();
    }
}