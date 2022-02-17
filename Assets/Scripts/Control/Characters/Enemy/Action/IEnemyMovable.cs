using System.Collections;
using Control.Characters.Type;
using Control.Weapon;

namespace Control.Characters.Enemy.Action
{
    public interface IEnemyMovable
    {
        public void Init(float speed = 0f);
        public void Disable();
        public EnemyActionType GetEnemyActionType();
    }
}