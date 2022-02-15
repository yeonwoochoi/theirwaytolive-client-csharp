using System;
using Control.Characters.Stat;
using Control.Layer;
using UnityEngine;

namespace Control.Characters.Health
{
    public class HealthSystem
    {
        private HealthBar healthBar;
        private Stat.Stat maxHp;
        public Stat.Stat MaxHp => maxHp;
        private float hp;

        public HealthSystem(Transform parent, float healthMax = 100f)
        {
            maxHp = new Stat.Stat(healthMax);
            healthBar = new HealthBar(parent, new Vector3(0, 1.5f), new Vector3(1.2f, 0.18f), Color.grey, Color.red, 1f,
                LayerType.Layer3,10000, new HealthBar.Outline { color = Color.black, size = .1f });
            hp = maxHp.GetValue();
        }

        public bool IsDead()
        {
            return hp <= 0;
        }

        public void Damaged(int amount, Action onDeadCallback = null)
        {
            if (IsDead()) return;
            hp -= amount;
            if (hp < 0) hp = 0;
            SetHealthBar();
            if (IsDead()) onDeadCallback?.Invoke();
        }

        public void Heal(int amount)
        {
            hp += amount;
            if (hp > maxHp.GetValue()) hp = maxHp.GetValue();
            SetHealthBar();
        }

        public void AddMaxHpModifier(StatModifier modifier, bool fullHealth)
        {
            maxHp.AddModifier(modifier);
            if (fullHealth) hp = maxHp.GetValue();
            SetHealthBar();
        }

        public void RemoveMaxHpModifier(StatModifier modifier, bool fullHealth)
        {
            maxHp.RemoveModifier(modifier);
            if (hp > maxHp.GetValue()) hp = maxHp.GetValue();
            SetHealthBar();
        }

        private void SetHealthBar()
        {
            healthBar.SetSize(GetHealthNormalized());
        }
        
        private float GetHealthNormalized()
        {
            return hp / maxHp.GetValue();
        }
    }
}