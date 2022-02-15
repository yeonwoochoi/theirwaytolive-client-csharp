using Control.Characters.Stat;
using UnityEngine;

namespace Control.Characters.Base
{
    public abstract class BaseCharacterStats: MonoBehaviour
    {
        [SerializeField] protected Stat.Stat criticalRate;
        [SerializeField] protected Stat.Stat defense;
        [SerializeField] protected Stat.Stat strength;
        [SerializeField] protected Stat.Stat blockRate;
        [SerializeField] protected Stat.Stat maxHp;
        [SerializeField] protected Stat.Stat accuracy;

        public Stat.Stat CriticalRate => criticalRate;
        public Stat.Stat Defense => defense;
        public Stat.Stat Strength => strength;
        public Stat.Stat BlockRate => blockRate;
        public Stat.Stat MaxHp => maxHp;
        public Stat.Stat Accuracy => accuracy;
    }
}