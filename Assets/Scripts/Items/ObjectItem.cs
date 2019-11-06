using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{
    public class ObjectItem : MonoBehaviour
    {

        public bool isPicked ;
        public bool justHover;

        protected Rigidbody2D m_rigidbody2D;
        public BoxCollider2D m_triggerCollider;
        public CircleCollider2D m_platformCollider;

        public GameObject itemParticle;


        public void ThrowinDrop()
        {
            if (!m_rigidbody2D)
            {
                //Debug.Log("rigidbody find");
                m_rigidbody2D = GetComponent<Rigidbody2D>();
            }
            //Debug.Log(m_rigidbody2D);

            m_rigidbody2D.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(4f, 5f));
            itemParticle.SetActive(true);
                
        }

        void Awake()
        {
            if(!gameObject.name.Contains("Empty"))
            {
                if(itemParticle == null)
                    itemParticle = Instantiate(FindObjectOfType<InventoryController>().itemParticle, transform);
                if(isPicked)
                    itemParticle.SetActive(false);
                if(justHover)
                {
                    itemParticle.SetActive(false);
                    StartCoroutine("Hover");
                }
            }
        }

        public void ThrowinDrop(Vector2 x, Vector2 y)
        {
            if (!m_rigidbody2D)
            {
                m_rigidbody2D = GetComponent<Rigidbody2D>();
            }

            m_rigidbody2D.velocity = new Vector2(Random.Range(x.x, x.y), Random.Range(y.x, y.y));
        }

        IEnumerator Hover()
        {
            float seta = 0;
            float rateTime = 1.8f;
            float radius = 0.05f;
            float preDistance = 0f;
            while(true)
            {
                if(isPicked)
                    break;
                Vector3 center = transform.position;
                Vector3 position = transform.position;
                position.y = Mathf.Sin(seta) * radius + center.y - preDistance;
                preDistance = Mathf.Sin(seta) * radius;
                transform.position = position;
                if(m_platformCollider != null)
                    m_platformCollider.offset += (Vector2)center - (Vector2)position;
                if(m_triggerCollider != null)
                    m_triggerCollider.offset += (Vector2)center - (Vector2)position;
                seta += Mathf.Deg2Rad * 360f / rateTime * Time.deltaTime;
                yield return null;
            }
        }
    }
}