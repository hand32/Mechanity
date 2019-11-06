using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{
    public class Bullet_Rocket : MonoBehaviour
    {
        [HideInInspector]
        public float bulletSpeed;
        public float splashRadius;

        public ParticleSystem explostionParticle;

        public void GetInfo(float _bulletSpeed)
        {
            bulletSpeed = _bulletSpeed;
        }
        
        void Start()
        {
            StartCoroutine("AddSpeed");
        }

        void FixedUpdate()
        {
            transform.position += transform.right.normalized * bulletSpeed * Time.fixedDeltaTime;
        }

        IEnumerator AddSpeed()
        {
            float _bulletSpeed = bulletSpeed;
            float ratio = 0f;
            while(true)
            {
                if(ratio >= 1f)
                {
                    bulletSpeed = _bulletSpeed;
                    break;
                }
                bulletSpeed = _bulletSpeed * ratio;
                ratio += Time.deltaTime * 1 / 0.4f;
                yield return null;
            }
        }

        void Crash(Transform _hit = null)
        {
            //setAnimation Boom
            HitCheck hitCheck = GetComponent<HitCheck>();
            List<string> targetTag = hitCheck.targetTag;

            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, splashRadius);
            foreach(Collider2D c in overlaps)
            {
                StatusManagement _status = c.gameObject.GetComponent<StatusManagement>();
                if(_status == null)
                    continue;
                if(targetTag.Contains(c.tag) && (_hit == null || _hit.transform != c.transform))
                    _status.GetDamage(hitCheck.damage, hitCheck.shooter);
            }
        }
        

        void StopBullet()
        {
            StopCoroutine("AddSpeed");
            bulletSpeed = 0;
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