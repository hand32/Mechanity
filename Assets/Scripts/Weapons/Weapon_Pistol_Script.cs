using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace roguelike
{
    public class Weapon_Pistol_Script : Class_Weapon
    {

        public void Fire(CurrentInput _currentinput)
        {
            if (!isPicked || reloading || _currentinput.shoot != KeyState.Down)
                return;

            if (ammo <= 0)
            {
                Reload();
                return;
            }

            if (1 / weaponInfo.fireRate > Time.time - preFireTime)
                return;
            preFireTime = Time.time;

            m_Animator.SetTrigger("Shoot");

            ammo -= 1;

            Vector2 _toVector = transform.right;
            Quaternion dir = Quaternion.Euler(0, transform.rotation.eulerAngles.y, Mathf.Round(_toVector.x) == 0f ? _toVector.y * 90 : _toVector.y * 45);


            var bullet = Instantiate(weaponInfo.bulletPrefab, bulletPosition.position, dir);
            var hitCheck = bullet.gameObject.GetComponent<HitCheck>();
            hitCheck.shooter = playerCollider.gameObject;
            hitCheck.damage = weaponInfo.damage;
            hitCheck.targetTag.Add("Monster");
            hitCheck.targetTag.Add("Platform");

            Vector2 _bulletVelocity = _toVector.normalized * weaponInfo.bulletSpeed;
            /*
            if((currentInput.right == KeyState.Held || currentInput.right == KeyState.Down) && !playerCollider.GetComponent<CharacterPhysics>().onWall)
            {
                _bulletVelocity += new Vector2(playerCollider.GetComponent<CharacterPhysics>().moveSpeed, 0f);
            }
            else if((currentInput.left == KeyState.Held || currentInput.left == KeyState.Down) && !playerCollider.GetComponent<CharacterPhysics>().onWall)
            {
                _bulletVelocity += new Vector2(-playerCollider.GetComponent<CharacterPhysics>().moveSpeed, 0f);
            }
            _bulletVelocity += new Vector2(0f, playerCollider.GetComponent<Rigidbody2D>().velocity.y);
            */
            Vector2 playerSpeed = playerCollider.GetComponent<Rigidbody2D>().velocity;
            float playerRotationYEuler = Mathf.Round(transform.rotation.eulerAngles.y);
            if ((playerSpeed.x > 0 && playerRotationYEuler == 0) || (playerSpeed.x < 0 && playerRotationYEuler != 0))
                _bulletVelocity.x += playerSpeed.x;
            if (!(Mathf.Round(_toVector.x) != 0) && ((_toVector.y > 0 && playerSpeed.y > 0) || (_toVector.y < 0 && playerSpeed.y < 0)))
                _bulletVelocity.y += playerSpeed.y;

            hitCheck.velocity = _bulletVelocity;

            cameraShake.Shake(0.1f, 0.8f);

            hitCheck.StartCoroutine("ShootingRadiusCheck", 10f / _bulletVelocity.magnitude);

            FireBulletListener.Invoke(bullet);
            WeaponFireListener.Invoke(gameObject);
        }

    }
}
