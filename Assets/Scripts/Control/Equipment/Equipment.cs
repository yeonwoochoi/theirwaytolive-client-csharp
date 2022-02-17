using System.Collections.Generic;
using Control.Characters.Stat;
using UnityEngine;

namespace Control.Equipment
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
    public class Equipment: ScriptableObject
    {
        public EquipmentType equipmentType;
        public List<StatModifier> modifiers;
    }

    public enum EquipmentType
    {
        Head, Chest, Weapon, Shield, Legs, Feet
    }
}