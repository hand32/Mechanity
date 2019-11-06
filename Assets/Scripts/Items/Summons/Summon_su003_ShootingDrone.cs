using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace roguelike
{
    public class Summon_su003_ShootingDrone : Class_SummonObject
    {

        public float damageRatio = 0.3f;
        public float firerateRatio = 0.2f;
        public float bulletCountRatio = 0.1f;

        public GameObject bulletPrefab;

        public GameObject weaponObject;
        public Class_Weapon weapon;

        void OnEnable()
        {
            Class_Weapon.WeaponPickListener.AddListener(PickWeapon);
            weapon = characterPhysics.GetComponentInChildren<Class_Weapon>();
            StartCoroutine("Shooting");
            StartCoroutine("Filp");
        }

        void PickWeapon(Class_Weapon _weapon)
        {
            weapon = _weapon;
        }

        IEnumerator Shooting()
        {
            StartCoroutine("Flip");
            while (true)
            {
                weaponObject = weapon.gameObject;
                float _damage = Mathf.RoundToInt(weapon.weaponInfo.damage * damageRatio);
                float _firerate = weapon.weaponInfo.fireRate * firerateRatio;
                int bulletCount = Mathf.RoundToInt(weapon.weaponInfo.ammo * bulletCountRatio);

                yield return new WaitForSeconds(1 / _firerate);
                Vector3 _toVector = weaponObject.transform.rotation.y == 0 ? Vector3.right : Vector3.left;
                Quaternion dir = Quaternion.Euler(0, weaponObject.transform.rotation.eulerAngles.y, 0);
                for (int i = 0; i < bulletCount; i++)
                {
                    Vector3 randomPos = new Vector3(Random.Range(0, 0.3f), Random.Range(-0.5f, 0.5f), 0);
                    var bullet = Instantiate(bulletPrefab, transform.position + _toVector + randomPos, dir);
                    var hitCheck = bullet.gameObject.GetComponent<HitCheck>();
                    hitCheck.shooter = characterPhysics.gameObject;
                    hitCheck.damage = _damage;
                    hitCheck.targetTag.Add("Monster");
                    hitCheck.targetTag.Add("Platform");

                    Vector2 _bulletVelocity = _toVector * 10f;
                    hitCheck.velocity = _bulletVelocity;

                    hitCheck.StartCoroutine("ShootingRadiusCheck", 7f / _bulletVelocity.magnitude);
                    Class_Weapon.FireBulletListener.Invoke(bullet);
                }
            }
        }
        IEnumerator Filp()
        {
            while (true)
            {
                if(Mathf.Round(weaponObject.transform.rotation.eulerAngles.y) == 0)
                    spriteRenderer.flipX = false;
                else 
                    spriteRenderer.flipX = true;
                yield return null;
            }
        }
    }
}