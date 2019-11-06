using UnityEngine;

namespace roguelike{
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponInfo : ScriptableObject {
	public WeaponInfo(WeaponInfo weaponInfo)
	{
		name = weaponInfo.name;
		damage = weaponInfo.damage;
		fireRate = weaponInfo.fireRate;
		bulletSpeed = weaponInfo.bulletSpeed;
		relodeSpeed = weaponInfo.relodeSpeed;
		ammo = weaponInfo.ammo;
		bulletPrefab = weaponInfo.bulletPrefab;
	}
	public new string name;
	public float damage;
	[Tooltip("fire/seconds")] 
	public float fireRate;
	public float bulletSpeed;
	public float relodeSpeed;
	public int ammo;
	public GameObject bulletPrefab;
}
}
