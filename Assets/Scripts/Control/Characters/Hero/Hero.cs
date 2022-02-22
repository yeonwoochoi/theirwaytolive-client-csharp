using System;
using System.Collections.Generic;
using Control.Characters.Emoji;
using Control.Characters.Type;
using Control.Weapon;
using UnityEngine;
using Logger = Util.Logger;

namespace Control.Characters.Hero
{
    public class Hero: MonoBehaviour, Enemy.Enemy.IEnemyInteractable
    {
        public interface IHeroInteractable: ICharacterInteractable
        {
            public void Interact(Enemy.Enemy.IEnemyInteractable attacker);
        }

        public static List<Hero> heroList = new List<Hero>();

        public static Hero Create(Vector3 position, Transform heroPrefab, HeroControlType heroControlType, WeaponType weaponType, bool activate = false)
        {
            var heroTransform = Instantiate(heroPrefab, position, Quaternion.identity);
            var heroHandler = heroTransform.GetComponent<Hero>();

            heroHandler.BeforeInit(weaponType, heroControlType);
            
            // Create 되자마자 Activate 되냐 마냐
            if (activate) heroHandler.Init();

            return heroHandler;
        }
        
        /// <summary>
        /// Equipment manager 때문에 있는거임..
        /// </summary>
        [SerializeField] private MainCharacterType mainCharacterType = MainCharacterType.Etc;
        
        private HeroControlType initHeroControlType;
        private WeaponType initWeaponType;
        private HeroMain heroMain;
        private bool isSet = false;
        
        private void BeforeInit(WeaponType weaponType, HeroControlType heroControlType)
        {
            initHeroControlType = heroControlType;
            initWeaponType = weaponType;
            heroMain = GetComponent<HeroMain>();
            heroMain.BeforeInit(weaponType);
        }

        /// <summary>
        /// 우선 Instantiate 하고 Activate는 나중에 해야하는 경우
        /// </summary>
        public void Init()
        {
            if (isSet) return;
            heroMain = GetComponent<HeroMain>();
            SetHeroControlType(initHeroControlType, type =>
            {
                heroMain.Init(this, type, initWeaponType);
            });
            heroList.Add(this);
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

        /// <summary>
        /// 공격을 하는 경우엔 target이 enemy겠지
        /// </summary>
        /// <param name="subject">말 그대로 Hero와 상호작용할 대상</param>
        public void Interact(IHeroInteractable subject)
        {
            if (!isSet) return;
            heroMain.Damaged(subject);
        }

        public void Heal(int amount)
        {
            if (!isSet) return;
            heroMain.Heal(amount);
        }

        public void ChangeWeapon(WeaponType weaponType)
        {
            if (!isSet) return;
            heroMain.ChangeWeapon(weaponType);
        }

        public void ShowEmoji(EmojiType type)
        {
            if (!isSet) return;
            heroMain.ShowEmoji(type);
        }

        public void ChangeControlType(HeroControlType heroControlType)
        {
            if (!isSet) return;
            heroMain.ChangeControlType(heroControlType);
        }

        public HeroControlType GetControlType()
        {
            return isSet ? heroMain.HeroControlStrategySelector.GetActiveControlType() : initHeroControlType;
        }

        public MainCharacterType GetMainCharacterType()
        {
            return mainCharacterType;
        }

        public bool IsDead()
        {
            if (!isSet)
            {
                Logger.Warn(this, "is not set");
                return true;
            }
            return heroMain.HeroStats.HealthSystem.IsDead();
        }

        private void SetHeroControlType(HeroControlType role, Action<HeroControlType> setControlType)
        {
            if (role == HeroControlType.Joystick)
            {
                foreach (var hero in heroList)
                {
                    if (hero.GetControlType() == HeroControlType.Joystick)
                    {
                        setControlType.Invoke(HeroControlType.Auto);
                    }
                }
                setControlType.Invoke(role);
            }
            else
            {
                setControlType.Invoke(role);
            }
        }
    }
}