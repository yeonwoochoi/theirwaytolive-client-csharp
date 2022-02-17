using System.Collections;
using Control.Characters.Type;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Enemy.Action
{
    public class EnemyDetectMoveStrategy: MonoBehaviour, IEnemyMovable
    {
        public void Init(float speed = 0)
        {
            
        }

        private IEnumerator Move()
        {
            yield return null;
        }

        public void Disable()
        {
            
        }
        
        public EnemyActionType GetEnemyActionType()
        {
            return EnemyActionType.Detect;
        }

        public void ChangeWeapon(WeaponType type)
        {
            
        }
    }
}