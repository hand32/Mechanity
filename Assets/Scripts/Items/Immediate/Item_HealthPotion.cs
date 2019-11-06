using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike
{
    public class Item_HealthPotion : ObjectItem
    {
        public ImmediateItemInfo itemInfo;
		
		Animator m_Animator;
        CapsuleCollider2D playerCollider;

        public GameObject pickPopParticle;

        bool used = false;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player" && !used)
            {
                used = true;
                SendMessage("Play", "Glup");
				StatusManagement status = other.GetComponent<StatusManagement>();
				float baseHealth = status.m_baseHealth;
				if(status.m_currentHealth + itemInfo.hpHeal > baseHealth)
					status.m_currentHealth = status.m_baseHealth;
				else
					status.m_currentHealth += itemInfo.hpHeal;
                StopCoroutine("Hover");
                GetComponent<SpriteRenderer>().enabled = false;
                itemParticle.SetActive(false);
                Instantiate(pickPopParticle, transform);
				Destroy(gameObject, 0.8f);
            }
        }
        void OnEnable()
        {
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
            m_triggerCollider = GetComponent<BoxCollider2D>();
            m_platformCollider = GetComponent<CircleCollider2D>();


            Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
            Physics2D.IgnoreLayerCollision(11, 10);
            Physics2D.IgnoreLayerCollision(11, 11);
            
        }

        void Start()
        {
            StartCoroutine("Hover");
            ThrowinDrop();
        }
    }
}