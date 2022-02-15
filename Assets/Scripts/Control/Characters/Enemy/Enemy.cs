using System.Collections.Generic;
using Control.Weapon;
using UnityEngine;
using Logger = Util.Logger;

namespace Control.Characters.Enemy
{
    public class Enemy: MonoBehaviour, Hero.Hero.IHeroInteractable
    {
        public interface IEnemyInteractable: ICharacterInteractable
        {
            public void Interact(Hero.Hero.IHeroInteractable attacker);
        }

        public static List<Enemy> enemyList = new List<Enemy>();
        
        public static Enemy Create(Vector3 position, Transform enemyPrefab, WeaponType weaponType, bool activate = false) {
            var enemyTransform = Instantiate(enemyPrefab, position, Quaternion.identity);

            var enemyHandler = enemyTransform.GetComponent<Enemy>();
            
            if (activate) enemyHandler.Init(weaponType);
            else enemyHandler.initWeaponType = weaponType;
            
            return enemyHandler;
        }

        public WeaponType initWeaponType;
        private EnemyMain enemyMain;
        private bool isSet = false;

        public void Init(WeaponType weaponType)
        {
            if (isSet) return;
            enemyMain = GetComponent<EnemyMain>();
            enemyMain.Init(this, weaponType);
            enemyList.Add(this);
            isSet = true;
        }
        public void Init()
        {
            if (isSet) return;
            enemyMain = GetComponent<EnemyMain>();
            enemyMain.Init(this, initWeaponType);
            enemyList.Add(this);
            isSet = true;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public bool IsDead()
        {
            return enemyMain.EnemyStats.HealthSystem.IsDead();
        }


        public void Interact(IEnemyInteractable attacker)
        {
            if (!isSet) return;
            enemyMain.Damaged(attacker);
        }

        public void Disable()
        {
            if (enemyList.Contains(this))
            {
                enemyList.Remove(this);
            }
        }
    }
}