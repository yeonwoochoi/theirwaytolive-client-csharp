using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Control.Characters.Stat
{
    // 이렇게 따로 index를 설정해주는 이유: Flat 보다 늦게 계산되어야하고 PercentAdd보다 먼저 계산되어야하는 경우에 Order를 150 이렇게 하면 된다.
    // 훨씬 flexible 하다.
    public enum StatModifierType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300
    }

    
    [Serializable]
    public class Stat
    {
        public float baseValue;

        // 최근에 modifier가 업데이트된 경우에 최종값 한번 더 계산하게끔
        // 아니면 걍 finalValue값 return
        protected bool isDirty = true;
        protected float finalValue;
        protected float lastBaseValue = float.MinValue;

        // ReadOnlyCollection: 말그대로 참조만 할 수 있음 statModifiers랑 동기화됨. (statModifiers가 수정되면 알아서 반영됨)
        protected readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        public Stat()
        {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public Stat(float baseValue): this()
        {
            this.baseValue = baseValue;
        }

        public virtual float GetValue()
        {
            if (isDirty || baseValue != lastBaseValue)
            {
                lastBaseValue = baseValue;
                finalValue = CalculateFinalValue();
                isDirty = false;
            }

            return finalValue;            
        }

        public virtual void AddModifier(StatModifier modifier)
        {
            isDirty = true;
            statModifiers.Add(modifier);
            statModifiers.Sort(ComparedModifierOrder);
        }

        public virtual bool RemoveModifier(StatModifier modifier)
        {
            if (statModifiers.Remove(modifier))
            {
                isDirty = true;
                return true;
            }

            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            var isRemoved = false;
            for (var i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].Source == source)
                {
                    isDirty = true;
                    isRemoved = true;
                    statModifiers.RemoveAt(i);
                }
            }

            return isRemoved;
        }

        protected virtual float CalculateFinalValue()
        {
            var result = baseValue;
            var sumPercentAdd = 0f;
            for (var i = 0; i < statModifiers.Count; i++)
            {
                var modifier = statModifiers[i];
                switch (modifier.Type)
                {
                    case StatModifierType.Flat:
                        result += modifier.Value;
                        break;
                    case StatModifierType.PercentAdd:
                        sumPercentAdd += modifier.Value;
                        if (i+1 >= statModifiers.Count || statModifiers[i+1].Type != StatModifierType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }
                        break;
                    case StatModifierType.PercentMult:
                        finalValue *= 1 + modifier.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // 12.12386 => 12.1239 
            return (float) Math.Round(result, 4);
        }

        protected virtual int ComparedModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order) return -1;
            else if (a.Order > b.Order) return 1;
            return 0;
        }
    }
    
    [Serializable]
    public class StatModifier
    {
        public float Value;
        public StatModifierType Type;
        
        /// <summary>
        /// 퍼센트 계산과 덧셈 계산에서 순서를 다르게 하면 다른 값이 나옴 -> 그래서 Type에 맞게 Order를 정해주고 그 순서에 맞게 계산해야함
        /// Flat 값부터 Base Value 에 싹다 더해놓고
        /// PercentAdd 값들 즉 퍼센트값 끼리 싹다 더해놓고 마지막에 최종 Flat 값까지 다 더해놓은 값 * 최종 Percent 계산
        /// PercentMult 값들은 바로 value 에 곱해버리는 것들인데
        /// PercentAdd 와 PercentMult 의 차이는 같은 종류로 Bonus Percent가 곱해지는건지 아니면 다른 종류로 Bonus Percent가 곱해지는 건지 구분하기 위함
        /// ex. 기본값 30, 갑옷 20%, 투구 15% = 35%
        /// ex1. PercentAdd => 30 * (1 + 0.2 + 0.15) = 40.5
        /// ex2. PercentMult => 30 * 1.2 * 1.15 = 41.4
        /// ex1과 ex2의 결과는 다르게 나옴.
        /// </summary>
        public int Order;
        public object Source;

        public StatModifier(float value, StatModifierType type, int order, object source)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }

        public StatModifier(float value, StatModifierType type) : this(value, type, (int) type, null) { }

        public StatModifier(float value, StatModifierType type, int order) : this(value, type, order, null) { }

        public StatModifier(float value, StatModifierType type, object source) : this(value, type, (int) type, source) { }
    }
}