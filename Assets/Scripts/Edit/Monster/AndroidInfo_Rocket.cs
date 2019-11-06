using UnityEngine;
using System;

namespace roguelike
{

    [CreateAssetMenu(fileName = "New Android_Rocket", menuName = "Monster/Android/Rocket")]
    public class AndroidInfo_Rocket : AndroidInfo_Basic
    {
        public float damage_Bullet;
        public float bulletSpeed;

        public float reloadTime;
        public float shootingDelay;

        public GameObject BulletPrefab;
    }
}