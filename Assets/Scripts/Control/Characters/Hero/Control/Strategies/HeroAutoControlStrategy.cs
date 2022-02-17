using System;
using System.Collections;
using Control.Characters.Base;
using Control.Characters.Type;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using Util;

namespace Control.Characters.Hero.Control.Strategies
{
    public class HeroAutoControlStrategy : MonoBehaviour, IHeroMovable
    {
        private HeroMain heroMain;
        private HeroTargeting heroTargeting;
        private HeroAnimationController heroAnimationController;

        private AIPath aiPath;
        private AIDestinationSetter destinationSetter;

        private enum State
        {
            Normal, Chasing, Attacking, Busy
        }
        
        private State state;

        private WeaponType weaponType;

        private float detectableRange;
        private const float wanderRange = 2f;
        private const float wanderCoolTime = 3.5f;
        private const float attackCoolTime = 2f;
        private bool isWanderCool = false;
        private bool isAttackCool = false;
        
        private Vector3 randomPosition;
        private Vector3 moveDir;

        private Coroutine moveCoroutine;

        private Hero.IHeroInteractable target;

        public void Init(float speed = 0)
        {
            heroMain = GetComponent<HeroMain>();
            heroAnimationController = GetComponent<HeroAnimationController>();
            heroTargeting = GetComponent<HeroTargeting>();
            
            aiPath = TryGetComponent<AIPath>(out var pathController)
                ? pathController
                : gameObject.AddComponent<AIPath>();
            destinationSetter = TryGetComponent<AIDestinationSetter>(out var aiDestination)
                ? aiDestination
                : gameObject.AddComponent<AIDestinationSetter>();
            
            aiPath.enabled = true;
            destinationSetter.enabled = true;

            aiPath.orientation = OrientationMode.YAxisForward;
            aiPath.radius = 0.5f;
            aiPath.gravity = Vector3.zero;
            aiPath.slowdownDistance = 1f;
            aiPath.endReachedDistance = 0f;
            aiPath.maxSpeed = speed;
            aiPath.enableRotation = false;
            
            detectableRange = HeroTargeting.detectableRange;
            state = State.Normal;
            isWanderCool = false;
            isAttackCool = false;

            moveDir = UtilsClass.GetRandomDir();
            randomPosition = GetPosition() + moveDir * wanderRange;

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(Move());
        }

        public void Disable()
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            
            ResetTarget();
            aiPath.InitPath();
            
            aiPath.enabled = false;
            destinationSetter.enabled = false;
        }

        public HeroControlType GetHeroControlType()
        {
            return HeroControlType.Auto;
        }

        public Direction GetMoveDirection()
        {
            return UtilsClass.GetMoveDirectionFromVector(moveDir);
        }

        private IEnumerator Move()
        {
            while (true)
            {
                yield return null;
                
                // Set target
                var tempTarget = heroTargeting.GetDetectableTarget();
                
                // Set current state
                SetState(tempTarget, () =>
                {
                    aiPath.InitPath();
            
                    var reachedDistance = state == State.Normal ? 0f : GetAttackRange();
                    aiPath.slowdownDistance = reachedDistance + 1f;
                    aiPath.endReachedDistance = reachedDistance;
                });

                // Set target or targetPosition
                // 여기서 설정하면 pathfinding 해서 그쪽으로 이동한다.
                SetTarget(tempTarget);
                
                // Set animation according to state
                // target을 설정한 후에 와야함
                SetAnimation();

                // Do action according to state
                switch (state)
                {
                    case State.Normal:
                        Wandering();
                        break;
                    case State.Chasing:
                        break;
                    case State.Attacking:
                        break;
                    case State.Busy:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SetState(Hero.IHeroInteractable tempTarget, Action onStateChangedCallback)
        {
            // Calculate target distance for set state
            var targetDistance = 0f;
            if (tempTarget != null) targetDistance = Vector2.Distance(tempTarget.GetPosition(), GetPosition());
            
            var prevState = state;
            if (tempTarget != null)
            {
                if (targetDistance > detectableRange)
                {
                    state = State.Normal;
                }
                else if (targetDistance > GetAttackRange())
                {
                    state = State.Chasing;
                }
                else
                {
                    state = State.Attacking;
                }
            }
            else
            {
                state = State.Normal;
            }
            
            if (prevState != state) onStateChangedCallback?.Invoke();
        }

        private void SetAnimation()
        {
            switch (state)
            {
                case State.Normal:
                    moveDir = (randomPosition - GetPosition()).normalized;
                    if (moveDir.magnitude > 0.01f) heroAnimationController.ChangeDirection(moveDir);
                    heroAnimationController.ChangeMovingState(Vector3.Distance(randomPosition, GetPosition()) >= 0.1f);
                    heroAnimationController.ChangeAttackState(false);
                    break;
                case State.Chasing:
                    moveDir = (target.GetPosition() - GetPosition()).normalized;
                    heroAnimationController.ChangeDirection(moveDir);
                    heroAnimationController.ChangeMovingState();
                    heroAnimationController.ChangeAttackState(false);
                    break;
                case State.Attacking:
                    moveDir = (target.GetPosition() - GetPosition()).normalized;
                    heroAnimationController.ChangeDirection(moveDir);
                    heroAnimationController.ChangeMovingState(false);
                    heroAnimationController.ChangeAttackState(!isAttackCool, Attack, AttackCoolTime);   
                    break;
                case State.Busy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetTarget(Hero.IHeroInteractable tempTarget = null)
        {
            switch (state)
            {
                case State.Normal:
                    ResetTarget();
                    destinationSetter.SetTarget(randomPosition);
                    break;
                case State.Chasing:
                    target = tempTarget;
                    destinationSetter.SetTarget(tempTarget?.GetGameObject().transform);
                    break;
                case State.Attacking:
                    target = tempTarget;
                    destinationSetter.SetTarget(tempTarget?.GetGameObject().transform);
                    break;
                case State.Busy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Wandering()
        {
            if (isWanderCool) return;
            StartCoroutine(StartWanderCoolTime());
        }

        private void Attack()
        {
            if (isAttackCool || target == null) return;
            target.Interact(heroMain.Hero);
        }

        private void AttackCoolTime()
        {
            if (isAttackCool) return;
            StartCoroutine(StartAttackCoolTime());
        }

        private IEnumerator StartWanderCoolTime()
        {
            if (isWanderCool) yield break;
            isWanderCool = true;
            yield return new WaitForSeconds(wanderCoolTime);
            
            // Set new random position
            var randomDir = UtilsClass.GetRandomDir();
            randomPosition = GetPosition() + randomDir * wanderRange;

            isWanderCool = false;
        }

        private IEnumerator StartAttackCoolTime()
        {
            if (isAttackCool) yield break;
            isAttackCool = true;
            yield return new WaitForSeconds(attackCoolTime);
            isAttackCool = false;
        }

        private float GetAttackRange()
        {
            return WeaponSystem.GetWeaponAttackRange(GetWeaponType());
        }

        private WeaponType GetWeaponType()
        {
            return heroMain.WeaponSystem.GetWeaponType();
        }
        
        private Vector3 GetPosition()
        {
            return transform.position;
        }
        
        private void ResetTarget()
        {
            target = null;
            destinationSetter.target = null;
        }
    }
}