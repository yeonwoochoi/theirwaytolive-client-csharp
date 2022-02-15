﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Util
{
    public class SpritePivotSetter : MonoBehaviour
    {
        [MenuItem("Sprites/Set Pivot(s)")]
        public static void SetPivots()
        {
            var textures = GetSelectedTextures();
            Selection.objects = new Object[0];

            foreach (var texture in textures)
            {
                var path = AssetDatabase.GetAssetPath(texture);
                Debug.Log(path);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                ti.isReadable = false;
                ti.isReadable = true;
                var newData = new List<SpriteMetaData>();
                for (int i = 0; i < ti.spritesheet.Length; i++)
                {
                    SpriteMetaData d = ti.spritesheet[i];
                    
                    // custom화 한다는 뜻
                    d.alignment = 9;
                    
                    // 원하는 pivot 여기다 설정하기
                    d.pivot = new Vector2(0.5f, 0.25f);
                    //d.pivot = new Vector2(0.5f, 0.4165f);
                    Debug.Log($"{i}: {d.pivot}");
                    
                    newData.Add(d);
                }

                ti.spritesheet = newData.ToArray();
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
        
        static Object[] GetSelectedTextures()
        {
            var results =  Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            return results;
        }
    }
}