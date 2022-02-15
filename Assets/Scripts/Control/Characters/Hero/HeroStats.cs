using System;
using System.Collections.Generic;
using Control.Characters.Base;
using Control.Characters.Health;
using Control.Characters.Stat;
using Control.Item;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Hero
{
    public class HeroStats: BaseCharacterStats
    {
        private bool isSet = false;
        
        private HeroMain heroMain;
        private HealthSystem healthSystem;
        public HealthSystem HealthSystem => healthSystem;

        private void Start()
        {
            EquipmentManager.Instance.UpgradeEquipmentEvent += UpdateEquipmentStats;
        }

        private void OnDisable()
        {
            EquipmentManager.Instance.UpgradeEquipmentEvent -= UpdateEquipmentStats;
        }
        
        public void Init()
        {
            if (isSet) return;
            heroMain = GetComponent<HeroMain>();
            
            // default equipment 착용 (주인공만 적용됨)
            // 주인공 아닌 애들은 baseValue 값만 있겠지
            if (GetCharacterType() != MainCharacterType.Etc)
            {
                EquipmentManager.Instance.InitHeroEquips(GetCharacterType());
                SetStatModifiers(EquipmentManager.Instance.GetHeroEquipments(GetCharacterType()));
            }

            healthSystem = new HealthSystem(transform, maxHp.GetValue());

            //Debug.Log($"Critical: {criticalRate.GetValue()}\nDefense: {defense.GetValue()}\nStrength: {strength.GetValue()}\nBlock: {blockRate.GetValue()}\nmaxHp: {maxHp.GetValue()}\nAccuracy: {accuracy.GetValue()}");
            
            isSet = true;
        }

        private void UpdateEquipmentStats(object _, EquipmentManager.UpgradeEquipmentEventArgs e)
        {
            if (!isSet) return;
            if (e.characterType != GetCharacterType() || e.characterType == MainCharacterType.Etc) return;
            SetStatModifiers(e.equipments);
        }

        private void SetStatModifiers(Equipment[] equipments)
        {
            foreach (var e in equipments)
            {
                foreach (var modifier in e.modifiers)
                {
                    GetStatFromEquipment(e.equipmentType).AddModifier(modifier);
                }       
            }
        }

        /// <summary>
        /// Equipment Type으로부터 Stat을 mapping
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Stat.Stat GetStatFromEquipment(EquipmentType type)
        {
            return type switch
            {
                EquipmentType.Head => criticalRate,
                EquipmentType.Chest => defense,
                EquipmentType.Weapon => strength,
                EquipmentType.Shield => blockRate,
                EquipmentType.Legs => maxHp,
                EquipmentType.Feet => accuracy,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private MainCharacterType GetCharacterType()
        {
            return heroMain.Hero.GetMainCharacterType();
        }
        
        public float GetSpeed(ControlType controlType)
        {
            // TODO (HeroStats) : 나중에 Speed 임시 변경시 수정 필요
            switch (controlType)
            {
                case ControlType.Joystick:
                    return 200f;
                case ControlType.Auto:
                    return 3f;
                case ControlType.Disable:
                    return 0f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(controlType), controlType, null);
            }
        }
    }
}