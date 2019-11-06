using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace roguelike
{
    public class FinalBoss_Missile : MonoBehaviour
    {

        float damage;
        float speed;

        float posX;

        bool bombed;

        Animator m_Animator;

        public Transform bombPosition;

        public void GetInfo(float _damage, float _speed)
        {
            damage = _damage;
            speed = _speed;
        }

        void Start()
        {
            SendMessage("Play", "Move");
            m_Animator = GetComponent<Animator>();
            posX = transform.position.y;
            StartCoroutine("AddSpeed");
        }

        void Update()
        {
            transform.position -= new Vector3(0, speed, 0) * Time.deltaTime;
            if (transform.position.y <= -2.62f)
                Bomb();
        }

        IEnumerator AddSpeed()
        {
            float _bulletSpeed = speed;
            float ratio = 0f;
            while (true)
            {
                if (ratio >= 1f)
                {
                    speed = _bulletSpeed;
                    break;
                }
                speed = _bulletSpeed * ratio;
                ratio += Time.deltaTime * 1 / 0.4f;
                yield return null;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                Bomb(other);
            }
        }

        void Bomb(Collider2D other = null)
        {
            if(bombed)
                return;
            SendMessage("RandomPlay", "Bomb");
            bombed = true;
            speed = 0;
            if(Random.Range(0, 2) == 0)
                m_Animator.SetTrigger("Bomb1");
            else
                m_Animator.SetTrigger("Bomb2");
            Collider2D[] overlaps = Physics2D.OverlapBoxAll(bombPosition.position + new Vector3(0, 1 - 0.6f, 0), new Vector2(3, 2), 0);
            foreach (Collider2D c in overlaps)
            {
                StatusManagement _status = c.gameObject.GetComponent<StatusManagement>();
                if (_status == null)
                    continue;
                if (c.tag == "Player" || (other != null && other.tag == "Player"))
                {
                    _status.GetDamage(damage, GameObject.Find("FinalBoss"));
                    break;
                }
            }

        }

        void DestroyThis()
        {
            Destroy(gameObject);
        }
        public void SpriteDisEnalbe()
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}