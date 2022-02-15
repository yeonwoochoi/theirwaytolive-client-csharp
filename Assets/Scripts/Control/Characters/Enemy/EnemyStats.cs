using Control.Characters.Base;
using Control.Characters.Health;

namespace Control.Characters.Enemy
{
    public class EnemyStats: BaseCharacterStats
    {
        private bool isSet = false;
        
        private EnemyMain enemyMain;
        private HealthSystem healthSystem;
        public HealthSystem HealthSystem => healthSystem;

        public void Init()
        {
            if (isSet) return;
            
            enemyMain = GetComponent<EnemyMain>();
            healthSystem = new HealthSystem(transform, maxHp.GetValue());
            
            isSet = true;
        }

        public float GetSpeed()
        {
            // TODO (EnemyStats): Speed 이거 나중에 수정하기
            return enemyMain.Enemy.IsDead() ? 0f : 2f;
        }
    }
}