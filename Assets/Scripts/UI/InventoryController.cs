using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace roguelike
{
    public class InventoryController : MonoBehaviour
    {
        public GameObject inventoryPanelObject;
        public GameObject currentWeapon;
        public GameObject equipitemPos;
        public GameObject itemPos;
        public GameObject itemName;
        public GameObject detailText;
        public GameObject itemParticle;

        bool gameStart = true;
        [SerializeField] private bool inventoryPopup;

        Vector3 preWeaponPos;
        private static InventoryController m_instance;
        public static InventoryController instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<InventoryController>();

                    if (m_instance == null)
                    {
                        m_instance = new GameObject("InventoryControl").AddComponent<InventoryController>();
                    }
                    m_instance.GetComponent<InventoryController>().enabled = true;
                }
                return m_instance;
            }
        }

        void Awake()
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        IEnumerator AwakeState()
        {
            while (!SceneController.isSceneLoaded)
            {
                yield return null;
            }
            if(gameObject.scene != SceneManager.GetActiveScene())
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            gameObject.transform.SetParent(Camera.main.transform);
            gameObject.transform.position = Camera.main.transform.position;
        }
        void OnEnable()
        {
            inventoryPanelObject = GameObject.Find("Inventory");
            currentWeapon = GameObject.Find("CurrentWeapon");
            equipitemPos = GameObject.Find("EquipItemPos");
            itemPos = GameObject.Find("ItemPos");
            itemName = GameObject.Find("ItemName");
            detailText = GameObject.Find("DetailText");
            preWeaponPos = currentWeapon.transform.localPosition;
            Class_Weapon.WeaponPickListener.AddListener(UpdateWeaponSprite);
            UpdateWeaponSprite(new Class_Weapon());
            currentWeapon.GetComponent<SpriteRenderer>().enabled = false;

            ItemProgress.instance.staticInventoryPanelObject = inventoryPanelObject;
            for (int i = 0; i < ItemProgress.instance.itemEquipMax; i++)
            {
                if(ItemProgress.instance.itemEquipList[i] == null)
                {
                    GameObject _emptyItem = Instantiate(ItemProgress.instance.emptyItemObject, inventoryPanelObject.transform);
                    _emptyItem.GetComponent<Class_PassiveItem>().isPicked = true;
                    
                    CapsuleCollider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
                    CircleCollider2D m_platformCollider = _emptyItem.GetComponent<CircleCollider2D>();
                    if(m_platformCollider != null && playerCollider != null)
                        Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
                    
                    ItemProgress.instance.itemEquipList[i] = _emptyItem;
                }
            }
            for (int i = 0; i < ItemProgress.instance.itemListMax; i++)
            {
                if(ItemProgress.instance.itemList[i] == null)
                {
                    GameObject _emptyItem = Instantiate(ItemProgress.instance.emptyItemObject, inventoryPanelObject.transform);
                    _emptyItem.GetComponent<Class_PassiveItem>().isPicked = true;
                    
                    CapsuleCollider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
                    CircleCollider2D m_platformCollider = _emptyItem.GetComponent<CircleCollider2D>();
                    if(m_platformCollider != null && playerCollider != null)
                        Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
                    
                    ItemProgress.instance.itemList[i] = _emptyItem;
                }
            }
            
            if (inventoryPanelObject.GetComponent<SpriteRenderer>().enabled)
                inventoryPanelObject.GetComponent<SpriteRenderer>().enabled = false;
            detailText.transform.parent.gameObject.SetActive(false);
            
            inventoryPopup = inventoryPanelObject.GetComponent<SpriteRenderer>().enabled;
        }

        void DetailOpen(Class_PassiveItem itemInfo)
        {
            if(detailText.transform.parent.gameObject.activeInHierarchy || !inventoryPopup)
                return;
            detailText.transform.parent.gameObject.SetActive(true);
            itemInfo.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            
            Text itemText = itemName.GetComponent<Text>();
            itemText.text = itemInfo.passiveItemInfo.name;
            Text textUI = detailText.GetComponent<Text>();
            textUI.text = itemInfo.passiveItemInfo.details;
            StartCoroutine("DetailFollow", itemInfo.transform);
        }
        void DetailClose()
        {
            detailText.transform.parent.gameObject.SetActive(false);
            StopCoroutine("DetailFollow");
        }
        IEnumerator DetailFollow(Transform _transform)
        {
            while(detailText.transform.parent.gameObject.activeInHierarchy)
            {
                yield return null;
                detailText.transform.parent.transform.position = _transform.position;
            }
        }

        void Update()
        {
            if(gameObject.transform.parent == null)
            {
                InActiveInventory();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!inventoryPopup)
                    ActiveInventory();
                else
                    InActiveInventory();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                InActiveInventory();

            Collider2D[] hit;
            hit = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            bool inactiveInventory = true;
            bool exitbutton = false;
            foreach (Collider2D _hit in hit)
            {
                Class_PassiveItem _class = _hit.transform.GetComponent<Class_PassiveItem>();
                if (_class != null && _class.m_triggerCollider)
                {
                    _class.MouseEnter();
                    if (Input.GetMouseButtonDown(0))
                    {
                        _class.MouseDown();
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        _class.OnMouseDrag();
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        _class.MouseUpAsButton();
                    }
                }

                if (!exitbutton)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_hit.name == "ExitButton")
                            exitbutton = true;
                        else if (_hit.name == "Inventory")
                            inactiveInventory = false;
                    }
                    else
                        inactiveInventory = false;
                }
            }
            if(inventoryPopup && (inactiveInventory || exitbutton))
                InActiveInventory();

            foreach (GameObject _go in ItemProgress.instance.itemEquipList)
            {
                bool _exist = false;
                foreach (Collider2D _hit in hit)
                {
                    if (_go != null && _hit.transform.gameObject == _go.transform.gameObject)
                    {
                        _exist = true;
                        break;
                    }
                }
                if (!_exist && _go != null)
                    _go.GetComponent<Class_PassiveItem>().MouseExit();
            }
            foreach (GameObject _go in ItemProgress.instance.itemList)
            {
                bool _exist = false;
                foreach (Collider2D _hit in hit)
                {
                    if (_go != null && _hit.transform.gameObject == _go.transform.gameObject)
                    {
                        _exist = true;
                        break;
                    }
                }
                if (!_exist && _go != null)
                    _go.GetComponent<Class_PassiveItem>().MouseExit();
            }
        }

        public void UpdateWeaponSprite(Class_Weapon _weapon)
        {
            GameObject pickedWeapon = null;
            if (_weapon.isPicked)
                pickedWeapon = _weapon.gameObject;
            else
            {
                GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
                foreach (GameObject weapon in weapons)
                {
                    if (weapon.GetComponent<Class_Weapon>().isPicked)
                    {
                        pickedWeapon = weapon;
                        break;
                    }
                }
            }

            currentWeapon.transform.localPosition = preWeaponPos;
            if (pickedWeapon == null)
                currentWeapon.GetComponent<SpriteRenderer>().sprite = null;
            else
            {
                currentWeapon.GetComponent<SpriteRenderer>().sprite = pickedWeapon.GetComponent<SpriteRenderer>().sprite;
                currentWeapon.transform.localPosition -= (Vector3)pickedWeapon.GetComponent<Collider2D>().offset * 2;
            }

        }


        public void ActiveInventory()
        {
            foreach (GameObject _go in ItemProgress.instance.itemEquipList)
            {
                if (_go != null)
                    _go.GetComponent<SpriteRenderer>().enabled = true;
            }
            foreach (GameObject _go in ItemProgress.instance.itemList)
            {
                if (_go != null)
                    _go.GetComponent<SpriteRenderer>().enabled = true;
            }
            currentWeapon.GetComponent<SpriteRenderer>().enabled = true;
            inventoryPanelObject.GetComponent<SpriteRenderer>().enabled = true;
            inventoryPopup = true;
            StartCoroutine("FollowingPanel");
        }

        public void InActiveInventory()
        {
            foreach (GameObject _go in ItemProgress.instance.itemEquipList)
            {
                if (_go != null)
                {
                    _go.GetComponent<Class_PassiveItem>().isDragging = false;
                    _go.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            foreach (GameObject _go in ItemProgress.instance.itemList)
            {
                if (_go != null)
                {
                    _go.GetComponent<Class_PassiveItem>().isDragging = false;
                    _go.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            currentWeapon.GetComponent<SpriteRenderer>().enabled = false;
            inventoryPanelObject.GetComponent<SpriteRenderer>().enabled = false;
            inventoryPopup = false;
            DetailClose();
            StopCoroutine("FollowingPanel");
        }

        IEnumerator FollowingPanel()
        {
            while (true)
            {
                //Vector3 _inventoryPosition = Camera.main.ScreenToWorldPoint
                int i = 0;
                foreach (GameObject _go in ItemProgress.instance.itemEquipList)
                {
                    if (_go == null)
                        break;
                    if (!_go.GetComponent<SpriteRenderer>().enabled)
                        _go.GetComponent<SpriteRenderer>().enabled = true;
                    if (!_go.GetComponent<Class_PassiveItem>().isDragging)
                    {
                        Vector3 _position = new Vector3(1.09f, 0, 0) * i;
                        _position.z = -1;
                        _go.transform.localPosition = equipitemPos.transform.localPosition + _position;
                    }
                    i += 1;
                }

                i = 0;
                int j = 0;
                foreach (GameObject _go in ItemProgress.instance.itemList)
                {
                    if (_go == null)
                        break;
                    if (i == 5)
                    {
                        i = 0;
                        j += 1;
                    }
                    if (!_go.GetComponent<SpriteRenderer>().enabled)
                        _go.GetComponent<SpriteRenderer>().enabled = true;
                    if (!_go.GetComponent<Class_PassiveItem>().isDragging)
                    {
                        Vector3 _position = new Vector3(1.09f, 0, 0) * i + new Vector3(0, -1, 0) * j;
                        _position.z = -1;
                        _go.transform.localPosition = itemPos.transform.localPosition + _position;
                    }
                    i += 1;
                }
                yield return null;
            }

        }

        void DontDestroyOnLoad()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}