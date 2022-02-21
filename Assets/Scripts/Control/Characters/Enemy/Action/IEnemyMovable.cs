using System.Collections;
using Control.Characters.Type;
using Control.Weapon;

namespace Control.Characters.Enemy.Action
{
    public interface IEnemyMovable
    {
        public void Init(float speed, float detectRange);
        public void Disable();
        public EnemyActionType GetEnemyActionType();
    }
}