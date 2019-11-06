using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{
    public class Android_Rocket : Android_Basic
    {
        public AndroidInfo_Rocket androidInfo;
        public Transform bulletPosition;
        public bool isShooting;
        public GameObject m_weaponObject;

        float preShootTime;

        public override void ToDoThings()
        {
            if(isDead)
                return;
            if(!isShooting)
                MoveToPlayer();
                CheckBasicSmash();
            if(findPlayer && !isBasicSmashing && !isShooting)
            {
                m_Animator.SetBool("Alert", true);
                RotateWeapon();
            }
            else
                m_Animator.SetBool("Alert", false);
            CheckShoot();

        }

        void OnEnable()
        {
            preShootTime = Time.time;
            androidInfo_Basic = androidInfo;
        }

        void CheckShoot()
        {
            if (!findPlayer || isShooting || isBasicSmashing || isCheckingBasicSmash)
                return;

            Vector2 toPlayerVector2 = (playerObject.transform.position - transform.position);
            if (androidInfo.reloadTime <= Time.time - preShootTime && !Physics2D.Raycast(bulletPosition.position, toPlayerVector2.normalized, toPlayerVector2.magnitude, 8))
            {
                isShooting = true;
                //m_Animator.SetTrigger("Shoot");
                StartCoroutine("CreatBullet");
            }
        }

        IEnumerator CreatBullet()
        {
            Vector2 toPlayerVector2 = (playerObject.transform.position - transform.position).normalized;
            transform.rotation = toPlayerVector2.x > 0f ?
                                Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180f, 0);
            //m_Animator.SetBool("Walk", false);
            
            m_Animator.SetBool("Alert", false);
            m_Animator.SetBool("Walk", false);
            yield return new WaitForSeconds(androidInfo.shootingDelay);
            m_Animator.SetTrigger("Shoot");
            
            float _zdegrees = Vector2.Angle(playerObject.transform.position.x >= transform.position.x ? Vector2.right : Vector2.left, toPlayerVector2);
            if (playerObject.transform.position.y < transform.position.y)
                _zdegrees = -_zdegrees;
            float _ydegrees = playerObject.transform.position.x >= transform.position.x ? 0f : 180f;
            Quaternion _dir = Quaternion.Euler(0, _ydegrees, _zdegrees);

            //Debug.Log("Shoot! the Rocket! " + gameObject.name);
            var bullet = Instantiate(androidInfo.BulletPrefab, (Vector3)bulletPosition.position, _dir);
            bullet.GetComponent<Bullet_Rocket>().GetInfo(androidInfo.bulletSpeed);


            var hitCheck = bullet.GetComponent<HitCheck>();
            hitCheck.shooter = gameObject;
            hitCheck.damage = androidInfo.damage_Bullet;
            hitCheck.targetTag.Add("Player");
            hitCheck.targetTag.Add("Platform");

            hitCheck.StartCoroutine("ShootingRadiusCheck", 14f/ androidInfo.bulletSpeed);

            cameraShake.Shake(0.12f, 1.5f);

            preShootTime = Time.time;
            yield return new WaitForSeconds(0.5f);
            isShooting = false;
        }

        public void RotateWeapon()
		{
            if(isShooting || isCheckingBasicSmash)
                return;

            Vector2 toPlayerVector2 = (playerObject.transform.position - transform.position).normalized;
			float _zdegrees = Vector2.Angle(playerObject.transform.position.x >= transform.position.x ? Vector2.right : Vector2.left, toPlayerVector2);
            if(playerObject.transform.position.y < transform.position.y)
                _zdegrees = -_zdegrees;

            float _ydegrees = playerObject.transform.position.x >= transform.position.x ? 0f : 180f;
            Quaternion _dir = Quaternion.Euler(0, _ydegrees, _zdegrees);

			m_weaponObject.transform.rotation = _dir;
		}
    }
}