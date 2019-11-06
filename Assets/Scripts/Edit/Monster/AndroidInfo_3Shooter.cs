using UnityEngine;
using System;

namespace roguelike
{

    [CreateAssetMenu(fileName = "New Android_3Shooter", menuName = "Monster/Android/3Shooter")]
    public class AndroidInfo_3Shooter : AndroidInfo_Basic
    {
        public float damage_Bullet;
        public float bulletSpeed;

        public float reloadTime;
        public float shootingDelay;

        public GameObject BulletPrefab;
    }
}