using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{
    public class Bullet_FollowRocket : MonoBehaviour
    {
        [HideInInspector]
        public float bulletSpeed;
        [HideInInspector]
        public GameObject playerObject;

        SpriteRenderer m_spriteRenderer;

        public float splashRadius;

        
        public ParticleSystem explostionParticle;

        public void GetInfo(float _bulletSpeed)
        {
            bulletSpeed = _bulletSpeed;
        }

        void OnEnable()
        {
            SendMessage("Play", "Shoot_1");
            SendMessage("Play", "Shoot_2");
            playerObject = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine("RotateBullet");
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            GetComponent<StatusManagement>().getDamageListeners.AddListener(Damaged);
        }

        void FixedUpdate()
        {
            transform.position += transform.right * bulletSpeed * Time.fixedDeltaTime;
        }

        IEnumerator RotateBullet()
        {
            while (true)
            {
                yield return null;
                Vector3 toVector = (playerObject.transform.position - transform.position).normalized;
                if (Vector3.Angle(toVector, transform.up) <= 90)
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 80 * Time.deltaTime));
                else
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, -80 * Time.deltaTime));
            }
        }

        void Crash(Transform _hit = null)
        {
            //setAnimation Boom
            bulletSpeed = 0f;
            StopCoroutine("RotateBullet");
            HitCheck hitCheck = GetComponent<HitCheck>();
            List<string> targetTag = hitCheck.targetTag;

            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, splashRadius);
            foreach (Collider2D c in overlaps)
            {
                StatusManagement _status = c.gameObject.GetComponent<StatusManagement>();
                if(_status == null)
                    continue;
                if (targetTag.Contains(c.tag) && (_hit == null || _hit.transform != c.transform))
                    _status.GetDamage(hitCheck.damage, hitCheck.shooter);
            }
        }

        public void Damaged(float damage)
        {
            StartCoroutine("Blink");
        }

        IEnumerator Blink()
        {
            float preTime = Time.time;
            while (true)
            {
                m_spriteRenderer.color = Color.gray;
                yield return new WaitForSeconds(0.08f);
                m_spriteRenderer.color = new Color(180f / 255f, 180f / 255f, 180f / 255f);
                yield return new WaitForSeconds(0.08f);
                if (Time.time - preTime >= 0.5f)
                    break;
            }
            m_spriteRenderer.color = Color.white;
        }

        void StopBullet()
        {
            StopCoroutine("RotateBullet");
            bulletSpeed = 0;
        }

        void Death()
        {
            GetComponent<HitCheck>().crash = true;
            SendMessage("StopBullet");
            GetComponent<Animator>().SetTrigger("Crash");
        }

        public void ExplosionPlay()
        {
            explostionParticle.Play();
        }
        public void ExplosionStop()
        {
            explostionParticle.Stop();
        }
    }
}