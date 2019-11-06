using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace roguelike
{
    public class ItemPickUps : MonoBehaviour
    {
        /*
        public PassiveItemInfo passiveItemInfo;

        [SerializeField]
        protected CapsuleCollider2D playerCollider;
        [SerializeField]
        protected BoxCollider2D m_triggerCollider;
        [SerializeField]
        protected CircleCollider2D m_platformCollider;
        
		void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                //F 띄우기.
                Debug.Log("Press F");
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
            while(true)
            {
                yield return null;
                if(PlayerController.currentInput.interaction == KeyState.Down) 
                {
                    PickItem();
                }
            }
        }

        void PlayerExit()
        {
            //다 없애기.
            Debug.Log("Player Exit.");
            StopCoroutine("PlayerEnter");
        }

        public void PickItem()
        {
            if(ItemProgress.instance.equipListCnt < ItemProgress.instance.itemEquipMax)
            {
                for(int i =0; i<ItemProgress.instance.itemEquipMax; i++)
                {
                    if(ItemProgress.instance.itemEquipList[i].name == "")
                    {
                        ItemProgress.instance.equipListCnt ++;
                        ItemProgress.instance.itemEquipList[i] = this.passiveItemInfo;
                        StopCoroutine("PlayerEnter");
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            else if(ItemProgress.instance.itemListCnt < ItemProgress.instance.itemListMax)
            {
                for(int i =0; i<ItemProgress.instance.itemListMax; i++)
                {
                    if(ItemProgress.instance.itemList[i].name == "")
                    {
                        ItemProgress.instance.itemListCnt ++;
                        ItemProgress.instance.itemList[i] = this.passiveItemInfo;
                        StopCoroutine("PlayerEnter");
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            
        }

        void Awake()
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
            if(passiveItemInfo.sprite == null)
            {
                Texture2D tex = new Texture2D(32, 32, Texture2D.whiteTexture.format, false);
                Sprite _sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
                GetComponent<SpriteRenderer>().sprite = _sprite;
            }
            else
                GetComponent<SpriteRenderer>().sprite = passiveItemInfo.sprite;
        }
        */


    }
}