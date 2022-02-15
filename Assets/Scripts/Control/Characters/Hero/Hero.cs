using System;
using System.Collections.Generic;
using Control.Weapon;
using UnityEngine;
using Logger = Util.Logger;

namespace Control.Characters.Hero
{
    public enum ControlType
    {
        Joystick, Auto, Disable
    }

    public enum MainCharacterType
    {
        Bachi, Duju, Panno, Etc
    }
    
    public class Hero: MonoBehaviour, Enemy.Enemy.IEnemyInteractable
    {
        public interface IHeroInteractable: ICharacterInteractable
        {
            public void Interact(Enemy.Enemy.IEnemyInteractable attacker);
        }

        public static List<Hero> heroList = new List<Hero>();

        public static Hero Create(Vector3 position, Transform heroPrefab, ControlType controlType, WeaponType weaponType, bool activate = false)
        {
            var heroTransform = Instantiate(heroPrefab, position, Quaternion.identity);

            var heroHandler = heroTransform.GetComponent<Hero>();

            // Create 되자마자 Activate 되느냐 마냐
            if (activate) heroHandler.Init(controlType, weaponType);
            else
            {
                heroHandler.initControlType = controlType;
                heroHandler.initWeaponType = weaponType;
            }
            
            return heroHandler;
        }
        
        [SerializeField] private MainCharacterType mainCharacterType = MainCharacterType.Etc;
        
        private ControlType initControlType;
        private WeaponType initWeaponType;
        private HeroMain heroMain;
        private bool isSet = false;

        /// <summary>
        /// Instantiate 하자마자 바로 Activate 하는 경우
        /// </summary>
        private void Init(ControlType controlType, WeaponType weaponType)
        {
            if (isSet) return;
            heroMain = GetComponent<HeroMain>();
            SetHeroControlType(controlType, type =>
            {
                heroMain.Init(this, type, weaponType);
            });
            heroList.Add(this);
            isSet = true;
        }

        /// <summary>
        /// 우선 Instantiate 하고 Activate는 나중에 해야하는 경우
        /// </summary>
        public void Init()
        {
            if (isSet) return;
            heroMain = GetComponent<HeroMain>();
            SetHeroControlType(initControlType, type =>
            {
                heroMain.Init(this, type, initWeaponType);
            });
            heroList.Add(this);
            isSet = true;
        }

        /// <summary>
        /// 이걸로 함수를 실행하면 Hero role 변경 관련 모든 script 실행.
        /// </summary>
        public void SetHeroControlType(ControlType role, Action<ControlType> setControlType)
        {
            if (role == ControlType.Joystick)
            {
                foreach (var hero in heroList)
                {
                    if (hero.GetActiveControlType() == ControlType.Joystick)
                    {
                        setControlType.Invoke(ControlType.Auto);
                    }
                }
                setControlType.Invoke(role);
            }
            else
            {
                setControlType.Invoke(role);
            }
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void Interact(IHeroInteractable attacker)
        {
            if (!isSet) return;
            heroMain.Damaged(attacker);
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

        public ControlType GetActiveControlType()
        {
            return isSet ? heroMain.HeroControlStrategySelector.GetActiveControlType() : initControlType;
        }

        public MainCharacterType GetMainCharacterType()
        {
            return mainCharacterType;
        }
    }
}