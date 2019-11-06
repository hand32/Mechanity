using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace roguelike
{
    public class ItemProgress : MonoBehaviour
    {

        public static ItemProgress instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<ItemProgress>();

                    if (m_instance == null)
                    {
                        m_instance = new GameObject("Item Progress").AddComponent<ItemProgress>();
                    }
                }
                return m_instance;
            }
        }

        private static ItemProgress m_instance;

        public int itemListMax = 10;
        public int itemEquipMax = 5;
        public List<GameObject> itemList = new List<GameObject>();
        public List<GameObject> itemEquipList = new List<GameObject>();
        public int itemListCnt = 0;
        public int equipListCnt = 0;

        public GameObject emptyItemObject;

        public GameObject staticInventoryPanelObject;



        void Awake()
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                for (int i = 0; i < itemListMax; i++)
                {
                    itemList.Add(null);
                }
                for (int i = 0; i < itemEquipMax; i++)
                {
                    itemEquipList.Add(null);
                }
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Reset()
        {
            for (int i = 0; i < itemListMax; i++)
            {
                if (i < itemList.Count)
                {
                    itemList[i] = null;
                }
            }
            for (int i = 0; i < itemEquipMax; i++)
            {
                if (i < itemEquipList.Count)
                {
                    if(itemEquipList[i] != null)
                        itemEquipList[i].SendMessage("UnEquipThis", SendMessageOptions.DontRequireReceiver);
                    itemEquipList[i] = null;
                }
            }
            itemListCnt = 0;
            equipListCnt = 0;
        }

        public void PickItem(GameObject _item)
        {
            if (equipListCnt < itemEquipMax)
            {
                for (int i = 0; i < itemEquipMax; i++)
                {
                    if (itemEquipList[i] == null || (itemEquipList[i].name.Contains("EmptyItem") && !_item.name.Contains("EmptyItem")))
                    {
                        Destroy(itemEquipList[i]);
                        equipListCnt++;
                        itemEquipList[i] = _item;
                        _item.GetComponent<SpriteRenderer>().sortingLayerName = "Inventory";
                        if (!_item.name.Contains("EmptyItem"))
                        {
                            _item.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            EquipItem(_item);
                        }
                        _item.GetComponent<SpriteRenderer>().enabled = false;
                        break;
                    }
                }
            }
            else if (itemListCnt < itemListMax)
            {
                for (int i = 0; i < itemListMax; i++)
                {
                    if (itemList[i] == null || itemList[i].name.Contains("EmptyItem") && !_item.name.Contains("EmptyItem"))
                    {
                        Destroy(itemList[i]);
                        itemListCnt++;
                        itemList[i] = _item;
                        _item.GetComponent<SpriteRenderer>().sortingLayerName = "Inventory";
                        if (!_item.name.Contains("EmptyItem"))
                            _item.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        _item.GetComponent<SpriteRenderer>().enabled = false;
                        break;
                    }
                }
            }
        }

        public void EquipItem(GameObject _item)
        {
            if(_item != null)
                _item.SendMessage("EquipThis", SendMessageOptions.DontRequireReceiver);
        }

        public void UnEquipItem(GameObject _item)
        {
            if(_item != null)
                _item.SendMessage("UnEquipThis", SendMessageOptions.DontRequireReceiver);
        }

        public void DropItem(GameObject _item)
        {
            if (itemList.Contains(_item))
            {
                int i = itemList.IndexOf(_item);
                GameObject _empty = Instantiate(emptyItemObject, staticInventoryPanelObject.transform);
                _empty.GetComponent<Class_PassiveItem>().isPicked = true;
                
                CapsuleCollider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
                CircleCollider2D m_platformCollider = _empty.GetComponent<CircleCollider2D>();
                if(m_platformCollider != null && playerCollider != null)
                    Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
                
                itemList[itemList.IndexOf(_item)] = _empty;
                for (; i < itemListCnt - 1; i++)
                {
                    //SwitchItem(itemList[i], itemList[i + 1]);
                }
                itemListCnt -= 1;
            }
            else if (itemEquipList.Contains(_item))
            {
                int i = itemEquipList.IndexOf(_item);
                GameObject _empty = Instantiate(emptyItemObject, staticInventoryPanelObject.transform);
                
                CapsuleCollider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
                CircleCollider2D m_platformCollider = _empty.GetComponent<CircleCollider2D>();
                if(m_platformCollider != null && playerCollider != null)
                    Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
                
                _empty.GetComponent<Class_PassiveItem>().isPicked = true;
                itemEquipList[itemEquipList.IndexOf(_item)] = _empty;

                for (; i < equipListCnt - 1; i++)
                {
                    //SwitchItem(itemEquipList[i], itemEquipList[i + 1]);
                }
                equipListCnt -= 1;
            }

            UnEquipItem(_item);

            _item.GetComponent<SpriteRenderer>().enabled = true;
            _item.GetComponent<SpriteRenderer>().sortingLayerName = "Item";
            _item.GetComponent<SpriteRenderer>().sortingOrder = 1;
            _item.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            _item.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
            _item.SendMessage("DropThis");
        }

        public void SwitchItem(GameObject _go1, GameObject _go2)
        {
            if (itemEquipList.Contains(_go1))
            {
                if (itemEquipList.Contains(_go2))
                {
                    int indexGo1 = ItemProgress.instance.itemEquipList.IndexOf(_go1);
                    int indexGo2 = ItemProgress.instance.itemEquipList.IndexOf(_go2);

                    List<GameObject> a= Class_SummonObject.summonList;
                    GameObject _tmp = Class_SummonObject.summonList[indexGo1];
                    Class_SummonObject.summonList[indexGo1] = Class_SummonObject.summonList[indexGo2];
                    Class_SummonObject.summonList[indexGo2] = _tmp;
                

                    int _index1 = itemEquipList.IndexOf(_go1);
                    itemEquipList[itemEquipList.IndexOf(_go2)] = _go1;
                    itemEquipList[_index1] = _go2;
                }
                else
                {
                    itemEquipList[itemEquipList.IndexOf(_go1)] = _go2;
                    itemList[itemList.IndexOf(_go2)] = _go1;
                    if (!_go2.name.Contains("EmptyItem"))
                        EquipItem(_go2);
                    if (!_go1.name.Contains("EmptyItem"))
                        UnEquipItem(_go1);
                }
            }
            else if (itemEquipList.Contains(_go2))
            {
                if (!itemEquipList.Contains(_go1))
                {
                    itemList[itemList.IndexOf(_go1)] = _go2;
                    itemEquipList[itemEquipList.IndexOf(_go2)] = _go1;
                    if (!_go1.name.Contains("EmptyItem"))
                        EquipItem(_go1);
                    if (!_go2.name.Contains("EmptyItem"))
                        UnEquipItem(_go2);
                }
            }
            else
            {
                int _index1 = itemList.IndexOf(_go1);
                itemList[itemList.IndexOf(_go2)] = _go1;
                itemList[_index1] = _go2;
            }
            if (_go1.name.Contains("EmptyItem"))
                _go1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            if (_go2.name.Contains("EmptyItem"))
                _go2.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }


        public void SceneSaveItem()
        {
            staticInventoryPanelObject.transform.parent.parent = null;
            DontDestroyOnLoad(staticInventoryPanelObject.transform.parent);
        }

        public void SceneLoadItem()
        {
            Physics2D.IgnoreLayerCollision(11, 10);
            Physics2D.IgnoreLayerCollision(11, 11);
            CapsuleCollider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
            foreach(GameObject _go in itemEquipList)
            {
                if (_go != null)
                {
                    CircleCollider2D m_platformCollider = _go.GetComponent<CircleCollider2D>();
                    if(m_platformCollider != null && playerCollider != null)
                        Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
                    _go.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
                    _go.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);
                    _go.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
                    if (!_go.name.Contains("Empty"))
                        EquipItem(_go);
                }
            }
            foreach(GameObject _go in itemList)
            {
                if (_go != null)
                {
                    CircleCollider2D m_platformCollider = _go.GetComponent<CircleCollider2D>();
                    if(m_platformCollider != null && playerCollider != null)
                        Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
                    _go.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
                    _go.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);
                    _go.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
                }
            }
        }


    }
}