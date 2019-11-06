using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace roguelike
{
    public class FinalBoss_Drone1_P2 : MonoBehaviour
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
            bool moveLeftDown = Random.Range(0, 2) == 0 ? true : false;
            while (true)
            {
				float angle = (goHere.y - transform.position.y) / (goHere.x - transform.position.x);
				if(moveLeftDown)
				{
					transform.position += new Vector3(-1, -1, 0).normalized * speed * Time.deltaTime;
					if(angle <= -1)
					{
						moveLeftDown = false;
					}
				}
				else
				{
					transform.position += new Vector3(-1, 1, 0).normalized * speed * Time.deltaTime;
					if(angle >= 1)
					{
						moveLeftDown = true;
					}
				}
				if(transform.position.x <= goHere.x)
				{
					transform.position = goHere;
					break;
				}
                yield return null;
            }

            m_Animator.SetTrigger("Explosion");
            yield return new WaitForSeconds(bombDelay);

            SendMessage("Play", "Bomb");
            GetComponent<CircleCollider2D>().enabled = false;
            cameraShake.Shake(0.5f, 0.4f);
            laserSpirteRenderer.color = new Color(1, 1, 1, 1);
            Collider2D[] _overlaps = Physics2D.OverlapBoxAll(transform.position, new Vector2(60, 1), 0f);
            foreach (Collider2D _c in _overlaps)
            {
                if (_c.tag == "Player")
                {
                    StatusManagement _status = _c.gameObject.GetComponent<StatusManagement>();
                    if (_status != null)
                        _status.GetDamage(damage, gameObject);
                }
            }
			_overlaps = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 60), 0f);
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