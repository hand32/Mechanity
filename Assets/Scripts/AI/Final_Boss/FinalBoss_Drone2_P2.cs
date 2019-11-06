using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace roguelike
{
    public class FinalBoss_Drone2_P2 : MonoBehaviour
    {

        Vector3 goHere;
        float speed;
        float damage;

        CameraShake cameraShake;
        public SpriteRenderer laserSpirteRenderer;
        Animator m_Animator;

        float bombDelay;

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
            laserSpirteRenderer.color = new Color(1, 1, 1, 1);
            Collider2D[] _overlaps = Physics2D.OverlapBoxAll(transform.position, new Vector2(60, 1), 45f);
            foreach (Collider2D _c in _overlaps)
            {
                if (_c.tag == "Player")
                {
                    StatusManagement _status = _c.gameObject.GetComponent<StatusManagement>();
                    if (_status != null)
                        _status.GetDamage(damage, gameObject);
                }
            }
			_overlaps = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 60), 45f);
            foreach (Collider2D _c in _overlaps)
            {
                if (_c.tag == "Player")
                {
                    StatusManagement _status = _c.gameObject.GetComponent<StatusManagement>();
                    if (_status != null)
                        _status.GetDamage(damage, gameObject);
                }
            }

            float _color = 1;
            while (true)
            {
                yield return null;
                _color -= Time.deltaTime;
                if (_color <= 0)
                {
                    laserSpirteRenderer.color = new Color(1, 1, 1, 0);
                    break;
                }
                laserSpirteRenderer.color = new Color(1, 1, 1, _color);
            }
			Destroy(gameObject);

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