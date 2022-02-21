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
            switch (type)
            {
                case WeaponType.Arrow:
                    return baseValue + 3.5f;
                case WeaponType.Spear:
                    return baseValue + 2f;
                case WeaponType.Sword:
                    return baseValue + 1.5f;
                case WeaponType.None:
                    return baseValue + 1f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}