using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace roguelike
{
    public class Class_PassiveItem : ObjectItem
    {

        public PassiveItemInfo passiveItemInfo;

        protected CapsuleCollider2D playerCollider;
        protected SpriteRenderer m_spriteRenderer;

        public bool isDragging;
        public static bool somethingDragging;

        public GameObject inventoryObject;

        public void MouseEnter()
        {
            if (isPicked)
            {
                //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                m_spriteRenderer.color = new Color(180f / 255f, 180f / 255f, 180f / 255f);
                if(!somethingDragging && isPicked && !gameObject.name.Contains("EmptyItem"))
                    inventoryObject.transform.parent.SendMessage("DetailOpen", this);
            }
        }

        public void MouseDown()
        {
            if (isPicked && !somethingDragging && !gameObject.name.Contains("EmptyItem"))
            {
                transform.localPosition += new Vector3(0, 0, 1f);
                isDragging = true;
                somethingDragging = true;
                m_spriteRenderer.sortingOrder = 3;
            }
        }

        public void OnMouseDrag()
        {
            if (isPicked && isDragging)
            {
                //Debug.Log("MouseDrag");
                Vector3 _position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _position.z = transform.position.z;
                m_spriteRenderer.color = new Color(120f / 255f, 120f / 255f, 120f / 255f);
                transform.position = _position;
            }
        }

        public void MouseUpAsButton()
        {
            if (isPicked && isDragging)
            {
                Collider2D[] _overlaps = Physics2D.OverlapPointAll((Vector2)transform.position);
                if (_overlaps.Length > 0)
                {
                    bool isInInventory = false;
                    bool alreadySwitch = false;
                    foreach (Collider2D _c in _overlaps)
                    {
                        if (_c.gameObject.name == "Inventory")
                        {
                            isInInventory = true;
                        }
                        Class_PassiveItem _itemClass = _c.GetComponent<Class_PassiveItem>();
                        if (!alreadySwitch && _itemClass != null && _itemClass.isPicked && _itemClass.gameObject != gameObject)
                        {
                            alreadySwitch = true;
                            ItemProgress.instance.SwitchItem(gameObject, _c.gameObject);
                            inventoryObject.transform.parent.SendMessage("DetailClose", passiveItemInfo.details);
                        }
                    }
                    if (!isInInventory)
                    {
                        ItemProgress.instance.DropItem(gameObject);
                        inventoryObject.transform.parent.SendMessage("DetailClose", passiveItemInfo.details);
                    }
                }
            }
            isDragging = false;
            somethingDragging = false;
        }

        public void MouseExit()
        {
            if (!m_spriteRenderer)
                m_spriteRenderer = GetComponent<SpriteRenderer>();
            if (gameObject.name.Contains("EmptyItem"))
                m_spriteRenderer.color = new Color(1, 1, 1, 0f);
            else
                m_spriteRenderer.color = new Color(1, 1, 1f);
            if(isPicked)
                m_spriteRenderer.sortingOrder = 2;
        }

        void OnMouseExit()
        {
            if(isPicked && !somethingDragging)
                inventoryObject.transform.parent.SendMessage("DetailClose");
        }

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
            if (other.tag == "Player" && !isPicked)
            {
                PlayerExit();
            }
        }

        IEnumerator PlayerEnter()
        {
            while (true)
            {
                if (PlayerController.currentInput.interaction == KeyState.Down &&
                    ItemProgress.instance.equipListCnt + ItemProgress.instance.itemListCnt < ItemProgress.instance.itemEquipMax + ItemProgress.instance.itemListMax)
                {
                    if(itemParticle != null)
                        itemParticle.SetActive(false);
                    SendMessage("Play", "Item_Effect");
                    isPicked = true;
                    m_rigidbody2D.gravityScale = 0;
                    m_platformCollider.enabled = false;
                    gameObject.transform.SetParent(inventoryObject.transform);
                    ItemProgress.instance.PickItem(gameObject);
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

        public void DropThis()
        {
            m_rigidbody2D.gravityScale = 2f;
            m_platformCollider.enabled = true;
            Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
            this.isPicked = false;
            this.transform.parent = null;
            ThrowinDrop();
            this.StartCoroutine("Hover");
        }

        void Start()
        {
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
            m_triggerCollider = GetComponent<BoxCollider2D>();
            m_platformCollider = GetComponent<CircleCollider2D>();
            Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
            Physics2D.IgnoreLayerCollision(11, 10);
            Physics2D.IgnoreLayerCollision(11, 11);
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            inventoryObject = GameObject.Find("Inventory");
            this.StartCoroutine("Hover");

            m_spriteRenderer = GetComponent<SpriteRenderer>();
            if (passiveItemInfo.sprite == null)
            {
                Texture2D tex = new Texture2D(32, 32, Texture2D.whiteTexture.format, false);
                Sprite _sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
                m_spriteRenderer.sprite = _sprite;
                m_spriteRenderer.color = new Color(1, 1, 1, 0);
            }
            else
                m_spriteRenderer.sprite = passiveItemInfo.sprite;
        }

    }
}