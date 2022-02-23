

using System;
using System.Collections.Generic;
using Control;
using Control.Characters.Enemy;
using Control.Characters.Hero;
using Control.Characters.Type;
using Control.Stuff;
using Control.Weapon;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private Camera mainCamera;

        private CameraFollow cameraFollow;

        private List<Enemy> enemyList;
        private List<Hero> heroList;

        private void Start()
        {
            cameraFollow = mainCamera.GetComponent<CameraFollow>();

            enemyList = new List<Enemy>();
            heroList = new List<Hero>();

            var map = Instantiate(GameAssets.i.pfMap1, Vector3.zero, Quaternion.identity);
            PathfindingManager.Instance.Init();
            
            var mainHero = Hero.Create(new Vector3(0, 19, 0), GameAssets.i.pfBachi, HeroControlType.Joystick, WeaponType.Arrow);
            //Hero.Create(new Vector3(0, 22, 0), GameAssets.i.pfDuju, HeroControlType.Auto, WeaponType.Sword, false);
            //Hero.Create(new Vector3(0, 16, 0), GameAssets.i.pfPanno, HeroControlType.Auto, WeaponType.Arrow, false);
            //Hero.Create(new Vector3(-3, 22, 0), GameAssets.i.pfBachi, ControlType.Auto, WeaponType.Spear, false);
            //Hero.Create(new Vector3(-3, 19, 0), GameAssets.i.pfDuju, HeroControlType.Auto, WeaponType.Spear, false);
            //Hero.Create(new Vector3(-3, 16, 0), GameAssets.i.pfPanno, HeroControlType.Auto, WeaponType.Sword, false);
            
            heroList.Add(mainHero);
            
            enemyList.Add(Enemy.Create(new Vector3(6, 19, 0), GameAssets.i.pfEnemy1, WeaponType.Spear, EnemyActionType.Escape, new List<HeroControlType>
            {
                HeroControlType.Joystick,
                HeroControlType.Auto
            }));
            
            enemyList.Add(Enemy.Create(new Vector3(6, 16, 0), GameAssets.i.pfEnemy1, WeaponType.Arrow, EnemyActionType.Escape, new List<HeroControlType>
            {
                HeroControlType.Joystick,
                HeroControlType.Auto
            }));
            
            enemyList.Add(Enemy.Create(new Vector3(6, 13, 0), GameAssets.i.pfEnemy1, WeaponType.Sword, EnemyActionType.Escape, new List<HeroControlType>
            {
                HeroControlType.Joystick,
                HeroControlType.Auto
            }));
            
            enemyList.Add(
                Enemy.Create(new Vector3(6, 16, 0), GameAssets.i.pfEnemy1, WeaponType.Spear, EnemyActionType.Escape,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                })
            );
            enemyList.Add(
                Enemy.Create(new Vector3(6, 13, 0), GameAssets.i.pfEnemy1, WeaponType.Arrow, EnemyActionType.Escape,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                })
            );
            enemyList.Add(
                Enemy.Create(new Vector3(6, 22, 0), GameAssets.i.pfEnemy1, WeaponType.Arrow, EnemyActionType.Escape,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                })
            );
            enemyList.Add(
                Enemy.Create(new Vector3(9, 16, 0), GameAssets.i.pfEnemy1, WeaponType.Sword, EnemyActionType.Escape,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                })
            );
            enemyList.Add(
                Enemy.Create(new Vector3(9, 13, 0), GameAssets.i.pfEnemy1, WeaponType.Sword, EnemyActionType.Escape,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                })  
            );

            cameraFollow.Init(mainHero.gameObject.transform);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                foreach (var enemy in enemyList)
                {
                    enemy.Init();
                }
                foreach (var hero in heroList)
                {
                    hero.Init();
                }
            }
        }
    }
}
