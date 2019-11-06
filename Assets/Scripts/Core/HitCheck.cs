using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace roguelike
{
    public class HitCheck : MonoBehaviour
    {

        public float damage;
        public List<string> targetTag = new List<string>();

        Rigidbody2D m_rigidbody;
        Animator m_Animator;
        public bool crash;
        public Vector2 velocity;
        public int penetrateCount = 1;

        public GameObject shooter;


        List<Transform> colliderList = new List<Transform>();

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_rigidbody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            transform.position += (Vector3)velocity * Time.fixedDeltaTime;
        }

        void OnTriggerExit2d(Collider2D hit)
        {
            if (colliderList.Contains(hit.transform))
            {
                colliderList.Remove(hit.transform);
            }
        }

        void OnTriggerEnter2D(Collider2D hit)
        {
            if (crash)
                return;
            //Debug.Log("TriggerEnter " + hit.tag);
            if (targetTag.Contains(hit.tag) && !colliderList.Contains(hit.transform))
            {
                colliderList.Add(hit.transform);
                if (hit.gameObject.GetComponent<StatusManagement>() != null)
                    hit.gameObject.GetComponent<StatusManagement>().GetDamage(damage, transform.position, shooter);
                CrashMessage(hit.transform);
            }
        }

        IEnumerator ShootingRadiusCheck(float time)
        {
            float elapsed = 0f;
            while (elapsed < time)
            {
                yield return null;
                if (crash)
                    break;
                elapsed += Time.deltaTime;
            }
            if (!crash)
            {
                crash = true;
                penetrateCount = 0;
                SendMessage("StopBullet");
                CrashMessage();
            }
        }

        public void CrashMessage(Transform hit = null)
        {
            m_Animator.SetTrigger("Crash");
            if (hit == null)
                SendMessage("Crash", SendMessageOptions.DontRequireReceiver);
            else
                SendMessage("Crash", hit, SendMessageOptions.DontRequireReceiver);

            if (penetrateCount > 0)
            {
                if (hit != null && hit.tag == "Platform")
                    penetrateCount = 0;
                else
                {
                    penetrateCount--;
                }
            }
            if (penetrateCount == 0)
            {
                m_Animator.SetBool("Stop", true);
                SendMessage("StopBullet");
                //Debug.Log("Bullet Hits " + hit.tag);
                crash = true;
            }
        }

        void StopBullet()
        {
            velocity = Vector2.zero;
        }

        void DestroyThis()
        {
            if (crash)
                Destroy(gameObject);
        }
    }
}