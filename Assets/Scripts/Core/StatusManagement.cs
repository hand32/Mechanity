using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace roguelike
{
    public class StatusManagement : MonoBehaviour
    {
        public bool isDamageable;
        public float m_baseHealth;
        public float m_currentHealth;
        public float m_baseArmor;
        public float m_currentArmor;
        public Image healthBar;
        public Text healthText;
        public Image armorBar;
        public Text armorText;

        public GameObject damageTextObjectInput;
        public static GameObject damageTextObject;

        [Serializable]
        public class GetDamageNotifyer : UnityEvent<float> { }

        public GetDamageNotifyer getDamageListeners = new GetDamageNotifyer();

        public class DamageObjectListener : UnityEvent<GameObject> { }

        public static GetDamageNotifyer playerBeforeGetDamageListeners;
        public static GetDamageNotifyer playerAfterGetDamageListeners;

        public static DamageObjectListener playerHitterListeners;
        public static DamageObjectListener playerHitBulletListeners;

        public static List<Func<float, float>> playerDamagedModifyerList;




        public UnityAction[] damageModify;

        void Awake()
        {
            if (tag == "Player")
            {
                damageTextObject = damageTextObjectInput;
                playerBeforeGetDamageListeners = new GetDamageNotifyer();
                playerAfterGetDamageListeners = new GetDamageNotifyer();
                playerHitterListeners = new DamageObjectListener();
                playerHitBulletListeners = new DamageObjectListener();
                playerDamagedModifyerList = new List<Func<float, float>>();
            }
        }

        float PlayerDamageModify(float _damage)
        {
            List<Func<float, float>> _sortedFuncList = new List<Func<float, float>>();
            foreach (GameObject _IGO in ItemProgress.instance.itemEquipList)
            {
                if(_IGO != null)
                {
                    Class_PassiveItem pi = _IGO.GetComponent<Class_PassiveItem>();
                    if (pi != null)
                    {
                        string itemID = pi.passiveItemInfo.ToString();
                        itemID = itemID.Substring(0, itemID.IndexOf(' '));
                        //Debug.Log(itemID);
                        if (playerDamagedModifyerList.Count != 0)
                        {
                            foreach (Func<float, float> _func in playerDamagedModifyerList)
                            {
                                //Debug.Log(_func.Method.DeclaringType.Name);
                                if (itemID == _func.Method.DeclaringType.Name)
                                {
                                    _sortedFuncList.Add(_func);
                                    break;
                                }
                            }
                        }
                    }
                }
            if (_sortedFuncList.Count != 0)
            {
                int i = 0;
                foreach (Func<float, float> _func in _sortedFuncList)
                {
                    i++;
                    Debug.Log("Sorted " + i + ":" + _func.Method.DeclaringType.Name);
                    if (_func != null)
                        _damage = _func(_damage);
                }
            }
        }

            return _damage;
        }

        void OnEnable()
        {
            if (gameObject.tag == "Player")
            {

                GameObject UI = GameObject.FindGameObjectWithTag("UI");
                foreach (Image i in UI.GetComponentsInChildren<Image>())
                {
                    if (i.name == "Current Health")
                    {
                        healthBar = i;
                    }
                    else if (i.name == "Current Armor")
                    {
                        armorBar = i;
                    }
                }
                foreach (Text t in UI.GetComponentsInChildren<Text>())
                {
                    if (t.name == "Health Text")
                    {
                        healthText = t;
                    }
                    else if (t.name == "Armor Text")
                    {
                        armorText = t;
                    }
                }

                //정보 받아오기
                if (PlayerProgress.instance.isStatSaved)
                {
                    m_baseHealth = PlayerProgress.instance.m_baseHealth;
                    m_currentHealth = PlayerProgress.instance.m_currentHealth;
                    m_currentArmor = PlayerProgress.instance.m_currentArmor;
                    m_baseArmor = PlayerProgress.instance.m_baseArmor;
                }
            }
        }

        void SaveData()
        {
            PlayerProgress.instance.m_baseHealth = m_baseHealth;
            PlayerProgress.instance.m_currentHealth = m_currentHealth;
            PlayerProgress.instance.m_baseArmor = m_baseArmor;
            PlayerProgress.instance.m_currentArmor = m_currentArmor;
            PlayerProgress.instance.isStatSaved = true;
        }

        public void GetMaxDamageNoExceptions(GameObject hitter)
        {
            Vector3 damagePosition = transform.position;
            float _damage = m_baseArmor + m_currentHealth;
            float _armorMinusDamage = _damage;
            if (m_currentArmor > 0)
            {
                if (m_currentArmor < _armorMinusDamage)
                {
                    _armorMinusDamage -= m_currentArmor;
                    m_currentArmor = 0;
                }
                else
                {
                    m_currentArmor -= _armorMinusDamage;
                    _armorMinusDamage = 0;
                }
            }
            m_currentHealth -= _armorMinusDamage;
            
            DamageController damageTextController = Instantiate(damageTextObject, damagePosition, Quaternion.Euler(0, 0, 0)).GetComponent<DamageController>();
            damageTextController.DamageUpdate(_damage);
            if (gameObject.tag == "Player")
            {
                damageTextController.damageText.color = new Color(1, 69f / 255f, 69f / 255f, 1);
                damageTextController.transform.localScale += new Vector3(0.2f, 0.2f);
            }

            if (m_currentHealth <= 0)
            {
                m_currentHealth = 0;
                isDamageable = false;
                SendMessage("Death", SendMessageOptions.DontRequireReceiver);
            }
        }
        public void GetDamage(float _damage, GameObject hitter)
        {
            if (!isDamageable)
                return;

            Vector3 damagePosition = transform.position;
            GetDamage(_damage, damagePosition, hitter);
        }

        public void GetDamage(float _damage, Vector3 damagePosition, GameObject hitter)
        {
            if (!isDamageable)
                return;

            Debug.Log(this.name + " Get " + _damage + " Damage!");
			if(tag == "Player")
            	_damage = PlayerDamageModify(_damage);


			if(tag == "Player")
            	playerBeforeGetDamageListeners.Invoke(_damage);

            float _armorMinusDamage = _damage;
            if (m_currentArmor > 0)
            {
                if (m_currentArmor < _armorMinusDamage)
                {
                    _armorMinusDamage -= m_currentArmor;
                    m_currentArmor = 0;
                }
                else
                {
                    m_currentArmor -= _armorMinusDamage;
                    _armorMinusDamage = 0;
                }
            }
            m_currentHealth -= _armorMinusDamage;
			if(tag == "Player")
			{
            	playerAfterGetDamageListeners.Invoke(_damage);
				playerHitterListeners.Invoke(hitter);
			}


            DamageController damageTextController = Instantiate(damageTextObject, damagePosition, Quaternion.Euler(0, 0, 0)).GetComponent<DamageController>();
            damageTextController.DamageUpdate(_damage);
            if (gameObject.tag == "Player")
			{
                damageTextController.damageText.color = new Color(1, 69f / 255f, 69f / 255f, 1);
				damageTextController.transform.localScale += new Vector3(0.2f, 0.2f);
			}

            if (m_currentHealth <= 0)
            {
                m_currentHealth = 0;
                isDamageable = false;
                SendMessage("Death", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                getDamageListeners.Invoke(_damage);
            }
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        void Update()
        {
            if (healthBar != null)
            {
                healthBar.fillAmount = m_currentHealth / m_baseHealth;
                healthText.text = m_currentHealth + "/" + m_baseHealth;
            }
            if (armorBar != null)
            {
                armorBar.fillAmount = m_currentArmor / m_baseArmor;
                armorText.text = m_currentArmor + "/" + m_baseArmor;
            }
        }

    }
}