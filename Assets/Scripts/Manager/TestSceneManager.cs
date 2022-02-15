using System;
using Control;
using Control.Characters.Enemy;
using Control.Stuff;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Manager
{
    public class TestSceneManager: MonoSingleton<TestSceneManager>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform pfPathfindingManager;
        [SerializeField] private Transform enemyPrefab;
        [SerializeField] private Transform heroPrefab;

        private Rigidbody2D rb2D;
        private Vector2 moveDir;
        private float speed = 100f;
        private PathfindingManager pathfindingManager;
        
        private void Awake()
        {
            var map = Instantiate(GameAssets.i.pfMap1, Vector3.zero, Quaternion.identity);

            var pathfinding = Instantiate(pfPathfindingManager, Vector3.zero, Quaternion.identity);
            pathfindingManager = pathfinding.GetComponent<PathfindingManager>();
            pathfindingManager.Init();
        }

        private void Update()
        {
            /*
            var tempX = Input.GetAxisRaw("Horizontal");
            var tempY = Input.GetAxisRaw("Vertical");

            moveDir = new Vector3(tempX, tempY);

            if (tempX != 0 || tempY != 0)
            {
                rb2D.velocity = moveDir * speed * Time.fixedDeltaTime;    
            }
            else
            {
                rb2D.velocity = Vector2.zero;
            }
            */
        }
    }
}