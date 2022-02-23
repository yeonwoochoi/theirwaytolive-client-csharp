using System;
using Control.Characters.Animation;
using Control.Weapon;
using UnityEngine;

namespace Control.Characters.Base
{
    public abstract class BaseCharacterAnimationController: MonoBehaviour
    {
        private Animator animator;
        protected AnimationEventTrigger animationEventTrigger;
        protected bool isSet = false;

        private readonly int animatorIdX = Animator.StringToHash(AnimatorParamX);
        private readonly int animatorIdY = Animator.StringToHash(AnimatorParamY);
        private readonly int animatorIdIsMoving = Animator.StringToHash(AnimatorParamIsMoving);
        private readonly int animatorIdIsAttack = Animator.StringToHash(AnimatorParamIsAttack);
        private readonly int animatorIdWeaponType = Animator.StringToHash(AnimatorParamWeaponType);
        private readonly int animatorIdIsDeath = Animator.StringToHash(AnimatorParamIsDeath);
        private readonly int animatorIdIsSpellCast = Animator.StringToHash(AnimatorParamIsSpellCast);
        private readonly int animatorIdIsSpawned = Animator.StringToHash(AnimatorParamIsSpawned);
        
        private const string AnimatorParamX = "X";
        private const string AnimatorParamY = "Y";
        private const string AnimatorParamIsMoving = "IsMoving";
        private const string AnimatorParamIsAttack = "IsAttack";
        private const string AnimatorParamWeaponType = "WeaponType";
        private const string AnimatorParamIsDeath = "IsDeath";
        private const string AnimatorParamIsSpellCast = "IsSpellCast";
        private const string AnimatorParamIsSpawned = "IsSpawn";

        public virtual void Init(bool isSpawned = false)
        {
            if (isSet) return;
            animator = GetComponent<Animator>();
            animationEventTrigger = TryGetComponent<AnimationEventTrigger>(out var eventTrigger) 
                ? eventTrigger 
                : gameObject.AddComponent<AnimationEventTrigger>();
        }
        
        public void ChangeDirection(float xDir, float yDir)
        {
            animator.SetFloat(animatorIdX, xDir);
            animator.SetFloat(animatorIdY, yDir);
        }
        public void ChangeDirection(Vector3 dir)
        {
            if (Math.Abs(Math.Abs(dir.x) - Math.Abs(dir.y)) < 0.1f)
            {
                animator.SetFloat(animatorIdX, dir.x);
                animator.SetFloat(animatorIdY, 0);
                return;
            }
            animator.SetFloat(animatorIdX, dir.x);
            animator.SetFloat(animatorIdY, dir.y);
        }

        public void ChangeWeapon(WeaponType weaponType)
        {
            animator.SetInteger(animatorIdWeaponType, (int) weaponType);
        }

        public void ChangeAttackState(bool isAttacking, Action midCallback = null, Action endCallback = null)
        {
            animationEventTrigger.Play(() =>
            {
                animator.SetBool(animatorIdIsAttack, isAttacking);
            }, () => {}, midCallback, endCallback);
        }
        
        public void ChangeSpellCastState(bool isSpellCasting)
        {
            animator.SetBool(animatorIdIsSpellCast, isSpellCasting);
        }

        public void ChangeMovingState(bool isMoving = true)
        {
            animator.SetBool(animatorIdIsMoving, isMoving);
        }

        public void ChangeDeathState(Action midCallback = null, Action endCallback = null)
        {
            animationEventTrigger.Play(() =>
            {
                animator.SetBool(animatorIdIsDeath, true);
            }, () => {}, midCallback, endCallback);
        }

        public void StartSpawnAnimation()
        {
            animator.SetTrigger(animatorIdIsSpawned);
        }
    }
}