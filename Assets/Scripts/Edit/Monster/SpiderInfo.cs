using UnityEngine;
using System;

namespace roguelike{

[CreateAssetMenu(fileName = "New Spider", menuName = "Monster/Spider")]
public class SpiderInfo : MonsterInfo {
	public float turnSpeed;
	public float damage_Bullet;

	public float reloadTime;
	public float shootingDelay;
	public float bulletSpeed;
    public int m_bulletCounts;
	public GameObject BulletPrefab;
}
}