using System;
using UnityEngine;

namespace Control.Weapon
{
    public enum WeaponType
    {
        Arrow, Spear, Sword, None
    }
    
    public class WeaponSystem
    {
        private WeaponType currentWeaponType;
        private Action<WeaponType> onChangedWeaponCallback;

        public WeaponSystem(WeaponType weaponType, Action<WeaponType> callback)
        {
            onChangedWeaponCallback = callback;
            SetWeaponType(weaponType);
        }
        
        public WeaponType GetWeaponType() {
            return currentWeaponType;
        }

        public void SetWeaponType(WeaponType type)
        {
            currentWeaponType = type;
            onChangedWeaponCallback?.Invoke(type);
        }
        
        public static float GetWeaponAttackRange(WeaponType type, bool isMainHero = false)
        {
            var baseValue = isMainHero ? 2f : 1f;
            return type switch
            {
                WeaponType.Arrow => baseValue + 3.0f,
                WeaponType.Spear => baseValue + 1f,
                WeaponType.Sword => baseValue + 0.5f,
                WeaponType.None => baseValue + 0f,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}