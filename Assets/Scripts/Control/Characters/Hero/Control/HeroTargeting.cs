using System.Collections.Generic;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Hero.Control
{
    public class HeroTargeting: MonoBehaviour
    {
        private bool isSet = false;
        private HeroMain heroMain;
        private WeaponTargetArea weaponTargetArea;
        private HeroControlStrategySelector strategySelector;
        
        // TODO (HeroTargeting): 여기다 두는게 맞는지
        public const float detectableRange = 5f;

        public void Init(HeroControlStrategySelector heroControlStrategySelector)
        {
            if (isSet) return;

            heroMain = GetComponent<HeroMain>();
            weaponTargetArea = new WeaponTargetArea(transform);
            strategySelector = heroControlStrategySelector;
            
            isSet = true;
        }

        public Hero.IHeroInteractable GetAttackableTarget()
        {
            return GetClosestEnemy(GetAttackableTargets());
        }

        public List<Hero.IHeroInteractable> GetAttackableTargets()
        {
            var targets = new List<Hero.IHeroInteractable>();
            foreach (var enemy in Enemy.Enemy.enemyList)
            {
                if (enemy.IsDead()) continue;
                if (IsInAttackArea(enemy))
                {
                    targets.Add(enemy);
                }
            }
            return targets;
        }
        
        public Hero.IHeroInteractable GetDetectableTarget()
        {
            return GetClosestEnemy(GetDetectableTargets());
        }
        
        private List<Hero.IHeroInteractable> GetDetectableTargets()
        {
            var targets = new List<Hero.IHeroInteractable>();
            foreach (var enemy in Enemy.Enemy.enemyList)
            {
                if (enemy.IsDead()) continue;
                if (IsInDetectableArea(enemy))
                {
                    targets.Add(enemy);
                }
            }
            return targets;
        }

        
        private Hero.IHeroInteractable GetClosestEnemy(List<Hero.IHeroInteractable> enemies)
        {
            Hero.IHeroInteractable closest = null;
            var position = GetPosition();
            foreach (Hero.IHeroInteractable enemy in enemies) {
                if (enemy.IsDead()) continue;
                if (closest == null) {
                    closest = enemy;
                } else {
                    if (Vector3.Distance(position, enemy.GetPosition()) < Vector3.Distance(position, closest.GetPosition())) {
                        closest = enemy;
                    }
                }
            }
            return closest;
        }
        
        private bool IsInAttackArea(Hero.IHeroInteractable enemy)
        {
            return weaponTargetArea
                .IsTargetable(GetWeaponType(), strategySelector.GetMoveDirection())
                .Invoke(enemy.GetPosition());
        }

        private bool IsInDetectableArea(Hero.IHeroInteractable enemy)
        {
            var targetDistance = Vector3.Distance(enemy.GetPosition(), GetPosition());
            return targetDistance <= detectableRange;
        }

        private WeaponType GetWeaponType()
        {
            return heroMain.WeaponSystem.GetWeaponType();
        }

        private Vector3 GetPosition()
        {
            return transform.position;
        } 
    }
}