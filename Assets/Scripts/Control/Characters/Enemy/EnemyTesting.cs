using System;
using System.Collections;
using Control.Weapon;
using Pathfinding;
using UnityEngine;
using Util;

namespace Control.Characters.Enemy
{
    public class EnemyTesting: MonoBehaviour
    {
        [SerializeField] private Transform enemy;
        private AIPath aiPath;
        private AIDestinationSetter destinationSetter;

        private void Start()
        {
            aiPath = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();
            Init(enemy);
        }

        public void Init(Transform target)
        {
            aiPath = TryGetComponent<AIPath>(out var pathController)
                ? pathController
                : gameObject.AddComponent<AIPath>();

            destinationSetter = TryGetComponent<AIDestinationSetter>(out var aiDestination)
                ? aiDestination
                : gameObject.AddComponent<AIDestinationSetter>();

            aiPath.orientation = OrientationMode.YAxisForward;
            aiPath.radius = 0.5f;
            aiPath.gravity = Vector3.zero;
            aiPath.slowdownDistance = 2.5f;
            aiPath.endReachedDistance = 1.5f;
            aiPath.maxSpeed = 1f;
            
            destinationSetter.target = target;
        }

        private void Update()
        {
            if (enemy == null)
            {
                aiPath.InitPath();
            }
        }
    }
}