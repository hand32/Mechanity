using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace roguelike
{
    public class FinalBoss_Drone3 : MonoBehaviour
    {

        Vector3 goHere;
        float speed;
        float damage;
        float bombDelay;

        CameraShake cameraShake;
        Animator m_Animator;


		public float bulletSpeed;
		public GameObject bulletPrefab;

        void Start()
        {
            SendMessage("Play", "Move");
            cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
            m_Animator = GetComponent<Animator>();
            StartCoroutine("Move");
        }

        public void GetInfo(Vector3 _pos, float _speed, float _damage, float _bombDelay)
        {
            goHere = _pos;
            speed = _speed;
            damage = _damage;
            bombDelay = _bombDelay;
        }

        IEnumerator Move()
        {
            bool moveHori = Random.Range(0, 2) == 0 ? true : false;
            while (true)
            {
                if (moveHori)
                {
                    int hori = (goHere - transform.position).x > 0 ? 1 : -1;
                    transform.position += new Vector3(hori * speed * Time.deltaTime, 0, 0);
                    if (hori == -1 ? transform.position.x <= goHere.x : transform.position.x >= goHere.x)
                    {
                        Vector3 result = transform.position;
                        result.x = goHere.x;
                        transform.position = result;
                        moveHori = false;
                    }
                }
                else
                {
                    int verti = (goHere - transform.position).y > 0 ? 1 : -1;
                    transform.position += new Vector3(0, verti * speed * Time.deltaTime, 0);
                    if (verti == -1 ? transform.position.y <= goHere.y : transform.position.y >= goHere.y)
                    {
                        Vector3 result = transform.position;
                        result.y = goHere.y;
                        transform.position = result;
                        moveHori = true;
                    }

                }

                if (transform.position.x == goHere.x && transform.position.y == goHere.y)
                {
                    break;
                }
                yield return null;
            }

            m_Animator.SetTrigger("Explosion");
            yield return new WaitForSeconds(bombDelay);
            GetComponent<CircleCollider2D>().enabled = false;

            SendMessage("Play", "Bomb");
            cameraShake.Shake(0.5f, 0.4f);
            
			for (int i = 0; i < 12; i++)
            {
				Quaternion dir = Quaternion.Euler(0, 0, 30 * i);
                var bullet = Instantiate(bulletPrefab, transform.position, dir);
                var hitCheck = bullet.gameObject.GetComponent<HitCheck>();
                hitCheck.shooter = GameObject.Find("FinalBoss");
                hitCheck.damage = damage;
                hitCheck.targetTag.Add("Player");

				Vector3 _bulletVelocity = bullet.transform.right.normalized * bulletSpeed;
                hitCheck.velocity = _bulletVelocity;

                hitCheck.StartCoroutine("ShootingRadiusCheck", 10);
            }

        }
        void Death()
        {
            Destroy(gameObject);
        }
        public void SpriteDisEnalbe()
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}