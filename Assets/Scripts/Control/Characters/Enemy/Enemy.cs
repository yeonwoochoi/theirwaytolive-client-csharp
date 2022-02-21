using System;
using System.Collections.Generic;
using System.Linq;
using Control.Characters.Emoji;
using Control.Characters.Hero;
using Control.Characters.Type;
using Control.Layer;
using Control.Stuff;
using Control.Weapon;
using UnityEngine;
using Logger = Util.Logger;

namespace Control.Characters.Enemy
{
    public class Enemy: MonoBehaviour, Hero.Hero.IHeroInteractable
    {
        public interface IEnemyInteractable: ICharacterInteractable
        {
            public void Interact(Hero.Hero.IHeroInteractable subject);
            public HeroControlType GetControlType();
        }

        public static List<Enemy> enemyList = new List<Enemy>();
        
        // TODO (Hero, Enemy): Creator를 따로 만들자..
        public static Enemy Create(Vector3 position, Transform enemyPrefab, WeaponType weaponType, EnemyActionType enemyActionType, List<HeroControlType> targetableTypeList, bool activate = false) {
            var enemyTransform = Instantiate(enemyPrefab, position, Quaternion.identity);

            var enemyHandler = enemyTransform.GetComponent<Enemy>();

            enemyHandler.enemyActionType = enemyActionType;
            enemyHandler.initWeaponType = weaponType;
            enemyHandler.targetableTypeList = targetableTypeList;

            if (activate) enemyHandler.Init();
            
            return enemyHandler;
        }
        
        public EnemyActionType enemyActionType;
        public WeaponType initWeaponType;
        public List<HeroControlType> targetableTypeList;
        
        private EnemyMain enemyMain;
        private bool isSet = false;

        public void Init()
        {
            if (isSet) return;
            enemyMain = GetComponent<EnemyMain>();
            enemyMain.Init(enemyActionType, initWeaponType);
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
            if (!IsTargetable(attacker)) return;
            enemyMain.Damaged(attacker);
        }
        
        public void Heal(int amount)
        {
            if (!isSet) return;
            enemyMain.Heal(amount);
        }

        public void Disable()
        {
            if (enemyList.Contains(this))
            {
                enemyList.Remove(this);
            }
        }

        /// <summary>
        /// Emoji 보여주고 싶으면 이거 Call 하면 됨
        /// 숨기고 싶으면 EmojiType.None 을 넣어주면 됨.
        /// </summary>
        /// <param name="type"></param>
        public void ShowEmoji(EmojiType type)
        {
            if (!isSet) return;
            enemyMain.ShowEmoji(type);
        }

        public bool IsTargetable(IEnemyInteractable target)
        {
            return targetableTypeList.Any(type => type == target.GetControlType());
        }
    }
}