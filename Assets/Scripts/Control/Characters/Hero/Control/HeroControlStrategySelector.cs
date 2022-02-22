using System;
using System.Collections.Generic;
using Control.Characters.Base;
using Control.Characters.Hero.Control.Strategies;
using Control.Characters.Type;
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
        
        private HeroControlType activeHeroControlType;
        private IHeroMovable activeStrategy;
        
        private List<IHeroMovable> strategies;

        public void Init(HeroControlType initHeroControlType)
        {
            if (isSet) return;
            
            heroMain = GetComponent<HeroMain>();
            
            heroAnimationController = GetComponent<HeroAnimationController>();
            heroTargeting = TryGetComponent<HeroTargeting>(out var targeting)
                ? targeting
                : gameObject.AddComponent<HeroTargeting>();
            
            heroTargeting.Init(this);
            
            strategies = new List<IHeroMovable>();
            SetControlStrategy(initHeroControlType);

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

        public void ChangeControlRole(HeroControlType heroControlType)
        {
            if (!isSet) return;
            SetControlStrategy(heroControlType);
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
                    if (GetActiveControlType() != HeroControlType.Joystick)
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

        private void SetControlStrategy(HeroControlType heroControlType)
        {
            activeHeroControlType = heroControlType;
            
            // 없으면 추가해주고
            switch(heroControlType)
            {
                case HeroControlType.Auto:
                    if (GetComponent<HeroAutoControlStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<HeroAutoControlStrategy>());
                    break;
                case HeroControlType.Joystick:
                    if (GetComponent<HeroJoystickControlStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<HeroJoystickControlStrategy>());
                    break;
                case HeroControlType.Disable:
                    if (GetComponent<HeroNpcControlStrategy>() == null)
                        strategies.Add(gameObject.AddComponent<HeroNpcControlStrategy>());
                    break;
            }
            
            var speed = GetSpeed(heroControlType);

            activeStrategy?.Disable();
            activeStrategy = GetMoveStrategyFromHeroRole(heroControlType);
            activeStrategy.Init(speed);
        }

        public HeroControlType GetActiveControlType()
        {
            return activeHeroControlType;
        }

        public Direction GetMoveDirection()
        {
            return activeStrategy.GetMoveDirection();
        }

        private IHeroMovable GetMoveStrategyFromHeroRole(HeroControlType heroControlType)
        {
            IHeroMovable result = null;
            foreach (var strategy in strategies)
            {
                if (heroControlType == strategy.GetHeroControlType())
                {
                    result = strategy;
                }
            }

            return result;
        }

        private float GetSpeed(HeroControlType heroControlType)
        {
            return heroMain.HeroStats.GetSpeed(heroControlType);
        }
    }
}