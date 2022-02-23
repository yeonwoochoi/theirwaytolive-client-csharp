using Control.Characters.Base;
using UnityEngine;

namespace Control.Characters.Enemy
{
    public class EnemyAnimationController: BaseCharacterAnimationController
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