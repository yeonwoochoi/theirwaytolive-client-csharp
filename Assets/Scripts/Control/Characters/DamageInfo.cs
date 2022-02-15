using Control.Characters.Base;
using UnityEngine;
using Util;

namespace Control.Characters
{
    public class DamageCalculator
    {
        private const float CriticalDamageMultiplier = 1.5f;
        
        private readonly BaseCharacterStats character;
        
        public DamageCalculator(BaseCharacterStats character)
        {
            this.character = character;
        }
        
        public class DamageInfo
        {
            public int amount;
            public bool isCritical;
            public bool isMiss;
        }


        public DamageInfo CalculateDamage(BaseCharacterStats attacker)
        {
            var isMiss = IsMiss(attacker);
            var isCritical = UtilsClass.GetRandomBooleanValueByPercentage((int) attacker.CriticalRate.GetValue());
            var damage = isMiss ? 0 : GetDamage(attacker, isCritical);

            return new DamageInfo
            {
                amount = damage,
                isCritical = isCritical,
                isMiss = isMiss
            };
        }

        private bool IsMiss(BaseCharacterStats attackerStats)
        {
            var attackerAccuracy = attackerStats.Accuracy.GetValue();
            var gap = attackerAccuracy - character.BlockRate.GetValue();
            if (gap <= 0)
            {
                return true;
            }
            else
            {
                return UtilsClass.GetRandomBooleanValueByPercentage((int) ((1f - gap / attackerAccuracy) * 100));
            }
        }
        
        private int GetDamage(BaseCharacterStats attacker, bool isCritical = false)
        {
            var range = attacker.Strength.GetValue() * 0.1f;
            var damage = Random.Range(-range, range) + attacker.Strength.GetValue();
            
            if (!isCritical) return (int) Mathf.Clamp(damage, 0, int.MaxValue);
            
            var criticalDamage = damage * CriticalDamageMultiplier;
            return (int) Mathf.Clamp(criticalDamage, 0, int.MaxValue);
        }
    }
}