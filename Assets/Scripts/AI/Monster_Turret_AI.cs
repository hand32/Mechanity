using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike
{
    public class Monster_Turret_AI : MonoBehaviour
    {


        public LayerMask platformLayerMask;
        public bool findPlayer;
        public float findRadius;
        public bool isShooting;
        float preShootTime;

        public float reloadTime;
        public float damage_Bullet;
        public float bulletSpeed;


        Vector3 bulletPosition;


        Rigidbody2D m_rigidbody2D;
        public CapsuleCollider2D playerCollider;

        public GameObject BulletPrefab;



        Vector2 toPlayerVector2;

        // Use this for initialization
        void OnEnable()
        {
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
        }

        void FindPlayer()
        {
            float _playerDistance = (playerCollider.transform.position - transform.position).magnitude;
            if (_playerDistance <= findRadius)
            {
                findPlayer = true;
            }
            else
            {
                findPlayer = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //collisionState.Reset();
            //CollisionCheck();
            FindPlayer();


            toPlayerVector2 = (playerCollider.transform.position - transform.position); //to player position. nomalized.
            Shoot();
        }

        void Shoot()
        {
            if (!findPlayer)
                return;


            bulletPosition = transform.position;

            if (reloadTime <= Time.time - preShootTime)
            {
                isShooting = true;
                CreatBullet();
            }
        }

        void CreatBullet()
        {
            if (Physics2D.Raycast(bulletPosition, toPlayerVector2, m_rigidbody2D.Distance(playerCollider).distance, platformLayerMask))
            {
                return;
            }

            float _degrees = Vector2.Angle(toPlayerVector2, toPlayerVector2.x > 0 ? Vector2.right : Vector2.left);
            Quaternion _dir = Quaternion.Euler(0, transform.rotation.eulerAngles.y, _degrees);
            var bullet = Instantiate(BulletPrefab, (Vector3)bulletPosition, _dir).GetComponent<Rigidbody2D>();
            var hitCheck = bullet.gameObject.GetComponent<HitCheck>();
            hitCheck.damage = damage_Bullet;
            hitCheck.targetTag.Add("Player");
            hitCheck.targetTag.Add("Platform");

            Vector2 bulletVelocity = toPlayerVector2.normalized * bulletSpeed;
            bullet.velocity = bulletVelocity;
            Destroy(bullet.gameObject, 10f);

            preShootTime = Time.time;

        }
        void Death()
        {
            GetComponent<StatusManagement>().DestroyObject();
        }


    }

}