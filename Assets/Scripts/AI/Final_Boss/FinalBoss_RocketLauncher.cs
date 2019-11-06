using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace roguelike
{
    public class FinalBoss_RocketLauncher : MonoBehaviour
    {

        public bool endShooting = false;

        float firerate;
        float shootingTime;
        float bulletSpeed;
        float bulletDamage;

        public Transform bulletPos1;
        public Transform bulletPos2;
        public Transform bulletPos3;


        public GameObject bulletPrefab;

		CameraShake cameraShake;
		void Start()
		{
			cameraShake = Camera.main.GetComponent<CameraShake>();
		}

        public void GetInfo(float _firerate, float _shootingTime, float _bulletSpeed, float _bulletDamage)
        {
            firerate = _firerate;
            shootingTime = _shootingTime;
            bulletSpeed = _bulletSpeed;
            bulletDamage = _bulletDamage;
        }

        public void Shoot()
        {
            endShooting = false;
            StartCoroutine("MoveForward");
        }

        IEnumerator MoveForward()
        {
            while (true)
            {
                transform.localPosition -= new Vector3(1, 0, 0) * Time.deltaTime;
                if (transform.localPosition.x <= 10.93f)
                {
                    Vector3 pos = transform.localPosition;
                    pos.x = 10.93f;
                    transform.localPosition = pos;
                    StartCoroutine("Shooting");
                    break;
                }
                yield return null;
            }
        }

        IEnumerator Shooting()
        {
			float timeFlow = 0;
            float _elapsed = 0;
            while (timeFlow <= shootingTime)
            {
                yield return null;
				timeFlow += Time.deltaTime;
                _elapsed += Time.deltaTime;
                if (_elapsed >= 1f/firerate)
                {
					int m = Random.Range(1, 4);
					Transform bulletPosition = bulletPos3;
					if(m == 1)
						bulletPosition = bulletPos1;
					else if(m == 2)
						bulletPosition = bulletPos2;
						
                    StartCoroutine("MissileShootingMotion", bulletPosition);

                    Quaternion _dir = Quaternion.Euler(0, 0, 180);

                    var bullet = Instantiate(bulletPrefab, (Vector3)bulletPosition.position, _dir);

                    bullet.GetComponent<Bullet_FollowRocket>().GetInfo(bulletSpeed);
                    bullet.GetComponent<StatusManagement>().m_currentHealth = 20;
                    var hitCheck = bullet.GetComponent<HitCheck>();
                    hitCheck.shooter = gameObject;
                    hitCheck.damage = bulletDamage;
                    hitCheck.targetTag.Add("Player");
                    hitCheck.targetTag.Add("Platform");

                    hitCheck.StartCoroutine("ShootingRadiusCheck", 10f);

                    cameraShake.Shake(0.5f, 1.2f);
                    _elapsed = 0;
                }
            }
			StartCoroutine("MoveBack");
        }

		IEnumerator MissileShootingMotion(Transform MissileBulletPosition)
        {
            float _posX = 0.3f;
            Vector3 reset = MissileBulletPosition.transform.parent.localPosition;
            while (true)
            {
                MissileBulletPosition.transform.parent.localPosition += new Vector3(3, 0, 0) * Time.deltaTime;
                _posX -= 3 * Time.deltaTime;
                if (_posX <= 0)
                {
                    break;
                }
                yield return null;
            }

            _posX = 0.3f;
            while (true)
            {
                MissileBulletPosition.transform.parent.localPosition -= new Vector3(2.5f, 0, 0) * Time.deltaTime;
                _posX -= 2.5f * Time.deltaTime;
                if (_posX <= 0)
                    break;
                yield return null;
            }
            MissileBulletPosition.transform.parent.localPosition = reset;
        }

        IEnumerator MoveBack()
        {
            while (true)
            {
                transform.localPosition += new Vector3(1, 0, 0) * Time.deltaTime;
                if (transform.localPosition.x >= 12.37f)
                {
                    Vector3 pos = transform.localPosition;
                    pos.x = 12.37f;
                    transform.localPosition = pos;
                    endShooting = true;
                    break;
                }
                yield return null;
            }
        }
    }
}