using System;
using System.Collections;
using Control.Characters.Base;
using Control.Characters.Type;
using Control.Weapon;
using UnityEngine;
using Util;

namespace Control.Characters.Hero.Control.Strategies
{
    public class HeroJoystickControlStrategy: MonoBehaviour, IHeroMovable
    {
        private enum State
        {
            Idle, Walking, Attack, Death, Talking, Casting, Etc
        }

        private float speed;
        private Vector3 moveDir;
        private Direction direction;
        private State state;
        private readonly float attackCoolTime = 1.1f;
        private bool isAttackCool = false;

        private HeroMain heroMain;
        private HeroTargeting heroTargeting;
        private HeroAnimationController heroAnimationController;
        private Rigidbody2D rb2D;

        private Coroutine moveCoroutine;

        public void Init(float speed = 0)
        {
            this.speed = speed;

            heroMain = GetComponent<HeroMain>();
            heroTargeting = GetComponent<HeroTargeting>();
            heroAnimationController = GetComponent<HeroAnimationController>();
            rb2D = GetComponent<Rigidbody2D>();

            // TODO (HeroJoystickControlStrategy): WeaponType Init
            
            state = State.Idle;
            enabled = true;
            
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (true)
            {
                yield return null;

                if (state == State.Death) yield break;
                
                while (state == State.Talking) yield return null;
                while (state == State.Casting) yield return null;

                var tempX = Input.GetAxisRaw("Horizontal");
                var tempY = Input.GetAxisRaw("Vertical");

                // check attack input
                if (Input.GetKeyDown(KeyCode.Space)) SetState(State.Attack);
                
                // check character is moving or idle
                if (tempX != 0 || tempY != 0)
                {
                    // set direction first
                    SetMoveDirection(tempX, tempY);
                    heroAnimationController.ChangeDirection(tempX, tempY);

                    // 움직이고 있는 상태 아니면 움직이게끔
                    if (state != State.Walking)
                    {
                        SetState(State.Walking);
                    }
                }
                else
                {
                    // 움직일때만 멈춰주면 되니까 나머지 state 일때는 정지한 상태에서 하기 때문에 냅둬도 됨.
                    if (state == State.Walking)
                    {
                        SetState(State.Idle);
                    }
                }

                moveDir = new Vector3(tempX, tempY).normalized;

                // 마지막 종합적으로.. start moving
                if (CanMoving())
                {
                    StartMoving();
                }
            }
        }

        public void Disable()
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            SetState(heroMain.Hero.IsDead() ? State.Death : State.Idle);
            enabled = false;
        }

        public HeroControlType GetHeroControlType()
        {
            return HeroControlType.Joystick;
        }

        public Direction GetMoveDirection()
        {
            return direction;
        }

        private void SetState(State state)
        {
            if (heroMain.Hero.IsDead())
            {
                this.state = State.Death;
                return;
            }
            
            this.state = state;
            heroAnimationController.ChangeMovingState(false);
            heroAnimationController.ChangeAttackState(false);

            switch (state)
            {
                case State.Idle:
                    StopMoving();
                    break;
                case State.Walking:
                    heroAnimationController.ChangeMovingState();
                    break;
                case State.Attack:
                    StopMoving();
                    heroAnimationController.ChangeAttackState(true,
                        Attack,
                        () => SetState(State.Idle));
                    break;
            }
        }
        
        private void StartMoving()
        {
            rb2D.velocity = moveDir * speed * Time.fixedDeltaTime;    
        }

        private void StopMoving()
        {
            rb2D.velocity = Vector3.zero;
        }

        private void Attack()
        {
            if (isAttackCool) return;
            if (IsMultipleTarget())
            {
                var targets = heroTargeting.GetAttackableTargets();
                if (targets.Count > 0)
                {
                    foreach (var target in targets)
                    {
                        target.Interact(heroMain.Hero);
                    }
                    StartCoroutine(StartCoolTime());
                }
            }
            else
            {
                var target = heroTargeting.GetAttackableTarget();
                if (target != null)
                {
                    heroTargeting.GetAttackableTarget().Interact(heroMain.Hero);
                    StartCoroutine(StartCoolTime());   
                }
            }
        }
        
        private IEnumerator StartCoolTime()
        {
            if (isAttackCool) yield break;
            isAttackCool = true;
            yield return new WaitForSeconds(attackCoolTime);
            isAttackCool = false;
        }

        /// <summary>
        /// 움직이면서 공격하고 싶으면 state == HeroState.Attack 을 추가하셈 뒤에
        /// </summary>
        /// <returns></returns>
        private bool CanMoving()
        {
            return state == State.Walking || state == State.Idle;
            // return state == HeroState.Walking || state == HeroState.Idle || state == HeroState.Attack;
        }
        
        private void SetMoveDirection(float x, float y)
        {
            direction = GetDirectionFromVector(x, y);
        }

        private Direction GetDirectionFromVector(float x, float y)
        {
            return UtilsClass.GetMoveDirectionFromVector(new Vector3(x, y));
        }

        private WeaponType GetWeaponType()
        {
            return heroMain.WeaponSystem.GetWeaponType();
        }

        private bool IsMultipleTarget()
        {
            // TODO (HeroJoystickControlStrategy): WeaponSystem 되면 구현하기
            switch (GetWeaponType())
            {
                case WeaponType.Arrow:
                    return false;
                case WeaponType.Spear:
                    return true;
                case WeaponType.Sword:
                    return true;
                case WeaponType.None:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}