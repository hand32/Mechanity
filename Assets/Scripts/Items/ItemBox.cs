using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike
{
    public class ItemBox : ObjectItem
    {
        public GameObject openPopParticle;

        CapsuleCollider2D playerCollider;

        public bool weaponDrop;
        public bool accessoryDrop;

        public List<GameObject> itemList;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player" && !isPicked)
            {
                //F 띄우기.
                //Debug.Log("Press F");
                StartCoroutine("PlayerEnter");
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                PlayerExit();
            }
        }

        IEnumerator PlayerEnter()
        {
            while (true)
            {
                if (PlayerController.currentInput.interaction == KeyState.Down)
                {
                    if (itemParticle != null)
                        itemParticle.SetActive(false);
                    SendMessage("Play", "Open");
                    isPicked = true;
                    m_rigidbody2D.gravityScale = 0;
                    m_platformCollider.enabled = false;

                    Instantiate(openPopParticle, transform);
                    Destroy(gameObject, 1.5f);
					
                	yield return null;
                    DropItem();

                    break;
                }
                yield return null;
            }
        }

        void PlayerExit()
        {
            //다 없애기.
            //Debug.Log("Player Exit.");
            StopCoroutine("PlayerEnter");
        }

        void DropItem()
        {
			if(!accessoryDrop && !weaponDrop)
				return;
			int index;
            while (true)
            {
				index = Random.Range(0, itemList.Count);
				if(itemList[index].name.Contains("Item") && accessoryDrop)
				{
					break;
				}
				else if(itemList[index].name.Contains("Weapon") && weaponDrop)
				{
					break;
				}
            }
            GameObject dropItem = Instantiate(itemList[index], transform.position, Quaternion.Euler(0, 0, 0));
            dropItem.GetComponent<ObjectItem>().ThrowinDrop();
        }

        void Start()
        {
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
            m_triggerCollider = GetComponent<BoxCollider2D>();
            m_platformCollider = GetComponent<CircleCollider2D>();
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
            Physics2D.IgnoreLayerCollision(11, 10);
            Physics2D.IgnoreLayerCollision(11, 11);
        }
    }

}