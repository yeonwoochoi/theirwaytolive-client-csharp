using System;
using System.Collections;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Enemy
{
    public class EnemyTargeting: MonoBehaviour
    {
        private bool isSet = false;
        private Enemy.IEnemyInteractable activeEnemyTarget;
        private Coroutine detectCoroutine;
        
        // TODO (EnemyTargeting) : 여기다 두는게 맞을지
        public const float detectableRange = 5f;
        
        public void Init()
        {
            if (isSet) return;
            if (detectCoroutine != null) StopCoroutine(detectCoroutine);
            detectCoroutine = StartCoroutine(FindTarget());
            isSet = true;
        }

        private IEnumerator FindTarget()
        {
            while (true)
            {
                yield return null;
                CheckEnemyDead();
                if (Hero.Hero.heroList.Count <= 0) continue;
                foreach (var target in Hero.Hero.heroList)
                {
                    if (target.IsDead()) continue;
                    if (Vector3.Distance(GetPosition(), target.GetPosition()) < detectableRange)
                    {
                        if (activeEnemyTarget == null)
                        {
                            activeEnemyTarget = target;
                        }
                        else
                        {
                            if (Vector3.Distance(GetPosition(),activeEnemyTarget.GetPosition()) > 
                                Vector3.Distance(GetPosition(), target.GetPosition()))
                            {
                                activeEnemyTarget = target;
                            }
                        }
                    }
                }
            }
        }

        private void CheckEnemyDead()
        {
            if (activeEnemyTarget != null && activeEnemyTarget.IsDead())
            {
                activeEnemyTarget = null;
            }
        }

        private Vector3 GetPosition()
        {
            return transform.position;
        }
        
        public Enemy.IEnemyInteractable GetTarget()
        {
            return activeEnemyTarget;
        }

        public void Disable()
        {
            if (detectCoroutine != null) StopCoroutine(detectCoroutine);
            activeEnemyTarget = null;
            enabled = false;
        }
    }
}