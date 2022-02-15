using System.Collections;
using Control.Characters.Base;
using UnityEngine;

namespace Control.Characters.Hero.Control.Strategies
{
    public interface IHeroMovable
    {
        public void Init(float speed = 0f);
        public IEnumerator Move();
        public void Disable();

        public ControlType GetHeroRole();
        public Direction GetMoveDirection();
    }
}