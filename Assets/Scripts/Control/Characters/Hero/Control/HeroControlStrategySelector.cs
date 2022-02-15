using System;
using System.Collections.Generic;
using Control.Characters.Base;
using Control.Characters.Hero.Control.Strategies;
using Control.Weapon;
using UnityEngine;
using Logger = Util.Logger;

namespace Control.Characters.Hero.Control
{
    public class HeroControlStrategySelector: MonoBehaviour
    {
        private bool isSet = false;
        
        private HeroMain heroMain;
        private HeroTargeting heroTargeting;
        private HeroAnimationController heroAnimationController;
        
        private ControlType activeControlType;
        private IHeroMovable activeStrategy;
        
        private List<IHeroMovable> strategies;

        public void Init(ControlType initControlType)
        {
            if (isSet) return;
            
            heroMain = GetComponent<HeroMain>();
            
            heroAnimationController = TryGetComponent<HeroAnimationController>(out var animationController)
                ? animationController
                : gameObject.AddComponent<HeroAnimationController>();
            heroTargeting = TryGetComponent<HeroTargeting>(out var targeting)
                ? targeting
                : gameObject.AddComponent<HeroTargeting>();


            heroTargeting.Init(this);
            heroAnimationController.Init();
            
            strategies = new List<IHeroMovable>();
            SetControlStrategy(initControlType);

            Logger.Debug(this, "init success");
            isSet = true;
        }

        /*
        public void Update()
        {
            if (gameObject.name != "Bachi(Clone)") return;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeControlRole(ControlType.Joystick);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeControlRole(ControlType.Auto);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeControlRole(ControlType.Disable);
            }
        }
        */

        public void ChangeControlRole(ControlType controlType)
        {
            if (!isSet) return;
            SetControlStrategy(controlType);
        }

        public void ChangeWeapon(WeaponType weaponType)
        {
            if (!isSet) return;
            heroAnimationController.ChangeWeapon(weaponType);
        }

        public void Disable()
        {
            foreach (var strategy in strategies)
            {
                strategy?.Disable();
            }

            if (heroMain.Hero.IsDead())
            {
                heroAnimationController.ChangeDeathState(() => {}, () =>
                {
                    // 주인공은 Fadeout 되는 등의 죽었을때 효과 안 나타남.
                    if (GetActiveControlType() != ControlType.Joystick)
                    {
                        heroMain.HeroEffectController.OnDeadEffect();
                    }
                });
            }
            else
            {
                heroAnimationController.ChangeDirection(0, 1);
                heroAnimationController.ChangeMovingState(false);
            }
        }

        private void SetControlStrategy(ControlType controlType)
        {
            activeControlType = controlType;
            
            // 없으면 추가해주고
            switch(controlType)
            {
                case ControlType.Auto:
                    if (GetComponent<HeroAutoControlStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<HeroAutoControlStrategy>());
                    break;
                case ControlType.Joystick:
                    if (GetComponent<HeroJoystickControlStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<HeroJoystickControlStrategy>());
                    break;
                case ControlType.Disable:
                    if (GetComponent<HeroNpcControlStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<HeroNpcControlStrategy>());
                    break;
            }
            
            var speed = GetSpeed(controlType);

            activeStrategy?.Disable();
            activeStrategy = GetMoveStrategyFromHeroRole(controlType);
            activeStrategy.Init(speed);
        }

        public ControlType GetActiveControlType()
        {
            return activeControlType;
        }

        public Direction GetMoveDirection()
        {
            return activeStrategy.GetMoveDirection();
        }

        private IHeroMovable GetMoveStrategyFromHeroRole(ControlType controlType)
        {
            IHeroMovable result = null;
            foreach (var strategy in strategies)
            {
                if (controlType == strategy.GetHeroRole())
                {
                    result = strategy;
                }
            }

            return result;
        }

        private float GetSpeed(ControlType controlType)
        {
            return heroMain.HeroStats.GetSpeed(controlType);
        }
    }
}