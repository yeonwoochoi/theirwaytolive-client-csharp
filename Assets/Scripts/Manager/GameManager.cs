

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
        
        private void Start()
        {
            cameraFollow = mainCamera.GetComponent<CameraFollow>();

            var map = Instantiate(GameAssets.i.pfMap1, Vector3.zero, Quaternion.identity);
            PathfindingManager.Instance.Init();
            
            var mainHero = Hero.Create(new Vector3(0, 19, 0), GameAssets.i.pfBachi, HeroControlType.Joystick, WeaponType.Spear, true);
            Hero.Create(new Vector3(0, 22, 0), GameAssets.i.pfDuju, HeroControlType.Auto, WeaponType.Sword, true);
            //Hero.Create(new Vector3(0, 16, 0), GameAssets.i.pfPanno, HeroControlType.Auto, WeaponType.Arrow, true);
            //Hero.Create(new Vector3(-3, 22, 0), GameAssets.i.pfBachi, ControlType.Auto, WeaponType.Spear, true);
            //Hero.Create(new Vector3(-3, 19, 0), GameAssets.i.pfDuju, HeroControlType.Auto, WeaponType.Spear, true);
            //Hero.Create(new Vector3(-3, 16, 0), GameAssets.i.pfPanno, HeroControlType.Auto, WeaponType.Sword, true);
            
            
            Enemy.Create(new Vector3(6, 19, 0), GameAssets.i.pfEnemy1, WeaponType.Spear, EnemyActionType.Attack, new List<HeroControlType>
                {
                    HeroControlType.Joystick,
                    HeroControlType.Auto
                }, true); 
            Enemy.Create(new Vector3(6, 16, 0), GameAssets.i.pfEnemy1, WeaponType.Arrow, EnemyActionType.Attack, new List<HeroControlType>
                {
                    HeroControlType.Joystick,
                    HeroControlType.Auto
                }, true); 
            Enemy.Create(new Vector3(6, 13, 0), GameAssets.i.pfEnemy1, WeaponType.Sword, EnemyActionType.Attack, new List<HeroControlType>
                {
                    HeroControlType.Joystick,
                    HeroControlType.Auto
                }, true);
            /*
            Enemy.Create(new Vector3(6, 16, 0), GameAssets.i.pfEnemy1, WeaponType.Spear, EnemyActionType.Attack,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                }, true);
            Enemy.Create(new Vector3(6, 13, 0), GameAssets.i.pfEnemy1, WeaponType.Arrow, EnemyActionType.Attack,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                }, true);
            Enemy.Create(new Vector3(6, 22, 0), GameAssets.i.pfEnemy1, WeaponType.Arrow, EnemyActionType.Attack,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                }, true);
            Enemy.Create(new Vector3(9, 16, 0), GameAssets.i.pfEnemy1, WeaponType.Sword, EnemyActionType.Attack,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                }, true);
            Enemy.Create(new Vector3(9, 13, 0), GameAssets.i.pfEnemy1, WeaponType.Sword, EnemyActionType.Attack,  new List<HeroControlType>
                {
                    HeroControlType.Auto,
                    HeroControlType.Joystick
                }, true);
            */
            
            cameraFollow.Init(mainHero.gameObject.transform);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }
}
