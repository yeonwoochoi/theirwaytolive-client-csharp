using System;
using System.Collections.Generic;
using Control.Characters.Hero;
using Control.Characters.Stat;
using Control.Stuff;
using Manager;
using UnityEngine;

namespace Control.Item
{
    public class EquipmentManager: MonoSingleton<EquipmentManager>
    {
        // Hero, Player들 Equipments 목록
        private Dictionary<MainCharacterType, Equipment[]> heroEquipments;

        // Equipment 업그레이드 시 호출되는 event
        public event EventHandler<UpgradeEquipmentEventArgs> UpgradeEquipmentEvent;
        public class UpgradeEquipmentEventArgs: EventArgs
        {
            public MainCharacterType characterType;
            public Equipment[] equipments;
        }
        
        /// <summary>
        /// Hero와 Equipment 등록
        /// </summary>
        /// <param name="mainCharacterType"></param>
        public void InitHeroEquips(MainCharacterType mainCharacterType)
        {
            if (mainCharacterType == MainCharacterType.Etc) return;
            heroEquipments ??= new Dictionary<MainCharacterType, Equipment[]>();
            if (!heroEquipments.ContainsKey(mainCharacterType))
            {
                heroEquipments.Add(mainCharacterType, GameAssets.i.defaultEquipments);
            }
        }

        /// <summary>
        /// 장비 업그레이드
        /// </summary>
        /// <param name="target"></param>
        /// <param name="equipmentType"></param>
        /// <param name="modifier"></param>
        public void UpgradeEquipment(MainCharacterType target, EquipmentType equipmentType, StatModifier modifier)
        {
            if (target == MainCharacterType.Etc) return;
            var targetEquipments = heroEquipments[target];
            foreach (var targetEquipment in targetEquipments)
            {
                if (targetEquipment.equipmentType == equipmentType)
                {
                    targetEquipment.modifiers.Add(modifier);
                }
            }
            EmitUpgradeEquipmentEvent(new UpgradeEquipmentEventArgs
            {
                characterType = target,
                equipments = targetEquipments
            });
        }

        /// <summary>
        /// Target Hero's current equipments Getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Equipment[] GetHeroEquipments(MainCharacterType target)
        {
            return heroEquipments[target];
        }

        private void EmitUpgradeEquipmentEvent(UpgradeEquipmentEventArgs e)
        {
            if (UpgradeEquipmentEvent == null) return;
            foreach (var invocation in UpgradeEquipmentEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }
    }
}