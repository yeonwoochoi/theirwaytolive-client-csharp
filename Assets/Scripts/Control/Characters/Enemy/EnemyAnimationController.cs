using Control.Characters.Base;

namespace Control.Characters.Enemy
{
    public class EnemyAnimationController: BaseCharacterAnimationController
    {
        public override void Init()
        {
            base.Init();
            isSet = true;
        }
    }
}