using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace roguelike
{
    public class Class_Weapon : ObjectItem
    {
        public bool isFixed = false;
        static bool isMeleeAttacking = false;
        public static float meleeAttackDamage = 40f;
        public static float meleeAttackRange = 1f;
        public WeaponInfo weaponInfo;
        public Transform bulletPosition;
        public float ammo;
        protected Text ammoText;
        public bool reloading;
        public static GameObject reloadFill;
        public static Image reloadFillImage;

        protected Quaternion dir;
        protected float preFireTime;

        protected bool addInputListenerForRotate;

        protected CapsuleCollider2D playerCollider;
        protected Animator m_Animator;
        protected CameraShake cameraShake;

        [HideInInspector]
        public Quaternion preRotate;
        [HideInInspector]
        public WeaponInfo originalWeaponInfo;

        Coroutine playerEnterCoroutine;

        public class Event : UnityEvent<Class_Weapon> { }
        public static Event WeaponPickListener = new Event();
        public static Event WeaponDropListener = new Event();

        public class FireEvent : UnityEvent<GameObject> { }
        public static FireEvent FireBulletListener = new FireEvent();
        public static FireEvent WeaponFireListener = new FireEvent();

        /*
        static void WeaponPickNotify()
        {
            Class_Weapon.WeaponPickListener.Invoke();
        }
        public static void AddWeaponPickListener(UnityAction _listener)
        {
            Class_Weapon.WeaponPickListener.AddListener(_listener);
        }
        public static void RemoveWeaponPickListener(UnityAction _listener)
        {
            Class_Weapon.WeaponPickListener.RemoveListener(_listener);
        }
        */



        void OnTriggerEnter2D(Collider2D other)
        {
            if (!isPicked && other.tag == "Player")
            {
                //F 띄우기.
                //Debug.Log("Press F");
                StartCoroutine("PlayerEnter");
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (!isPicked && other.tag == "Player")
            {
                PlayerExit();
            }
        }

        IEnumerator PlayerEnter()
        {
            while (true)
            {
                yield return null;
                if (PlayerController.currentInput.interaction == KeyState.Down && !isPicked)
                {
                    PickWeapon();
                    break;
                }
            }
        }

        void PlayerExit()
        {
            //다 없애기.
            //Debug.Log("Player Exit.");
            this.StopCoroutine("PlayerEnter");
        }

        public void PickWeapon()
        {
            SendMessage("Play", "PickUp");
            if (itemParticle != null)
                itemParticle.SetActive(false);
            GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
            GameObject pickedWeapon = null;
            foreach (GameObject weapon in weapons)
            {
                if (weapon.GetComponent<Class_Weapon>().isPicked)
                {
                    pickedWeapon = weapon;
                    break;
                }
            }
            this.isPicked = true;
            if (pickedWeapon != null)
                pickedWeapon.GetComponent<Class_Weapon>().SendMessage("DropWeapon");
            transform.SetParent(playerCollider.transform);
            transform.localPosition = new Vector3(0f, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, playerCollider.transform.rotation.eulerAngles.y, 0f);
            m_rigidbody2D.simulated = false;
            WeaponPickListener.Invoke(this);
        }

        public void DropWeapon()
        {
            this.StopCoroutine("Reload_ammo");
            reloadFillImage.fillAmount = 0;
            reloadFill.transform.parent.gameObject.SetActive(false);
            reloading = false;
            isFixed = false;

            m_rigidbody2D.simulated = true;
            this.isPicked = false;
            ThrowinDrop();
            this.StartCoroutine("Hover");
            WeaponDropListener.Invoke(this);
            transform.parent = null;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        void Start()
        {
            cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
            ParticleSystem.MainModule particleMain = itemParticle.GetComponent<ParticleSystem>().main;
            particleMain.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 172f / 255f, 216f / 255f), new Color(1, 100f / 255f, 183f / 255f));
            if (itemParticle != null && isPicked)
                itemParticle.SetActive(false);
            originalWeaponInfo = weaponInfo;
            weaponInfo = new WeaponInfo(weaponInfo);
            preFireTime = 0;
            ammo = weaponInfo.ammo;
            reloading = false;

            GameObject UI = GameObject.FindGameObjectWithTag("UI");
            foreach (Text t in UI.GetComponentsInChildren<Text>())
            {
                if (t.name == "Ammo Text")
                {
                    ammoText = t;
                }
            }

            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
            m_triggerCollider = GetComponent<BoxCollider2D>();
            m_platformCollider = GetComponent<CircleCollider2D>();
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_Animator = GetComponent<Animator>();

            Physics2D.IgnoreCollision(playerCollider, m_platformCollider);
            Physics2D.IgnoreLayerCollision(11, 10);
            Physics2D.IgnoreLayerCollision(11, 11);

            if (isPicked)
            {
                m_rigidbody2D.simulated = false;
            }
            else
                this.StartCoroutine("Hover");
        }

        void Update()
        {
            if (isPicked)
            {
                RotateWeapon(PlayerController.currentInput);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                return;
            }

            if (ammoText)
                ammoText.text = ammo + "/" + weaponInfo.ammo;
        }

        void LateUpdate()
        {
            if(reloadFill != null)
                reloadFill.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
        }

        void FireMessage(CurrentInput _currentInput)
        {
            if (isMeleeAttacking)
                return;

            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position + playerCollider.transform.right * 0.5f, meleeAttackRange);
            
            foreach (Collider2D _c in overlaps)
            {
                if (_c.tag == "Monster")
                {
                    m_Animator.SetTrigger("MeleeAttack");
                    isMeleeAttacking = true;
                    StartCoroutine("MeleeAttacking");
                    List<GameObject> meleeAttackObjects = new List<GameObject>();
                    foreach (Collider2D __c in overlaps)
                    {
                        if (__c.tag == "Monster" && !meleeAttackObjects.Contains(__c.gameObject))
                        {
                            meleeAttackObjects.Add(__c.gameObject);
                            StatusManagement _status = __c.GetComponent<StatusManagement>();
                            if (_status != null)
                                _status.GetDamage(meleeAttackDamage, gameObject);
                        }
                    }
                    break;
                }
            }
            // physics2D 로 주변 콜라이더 다 탐색. 그리고 그 안에 몬스터가있으면 근접공격 하기.
            if (!isMeleeAttacking)
                gameObject.SendMessage("Fire", _currentInput, SendMessageOptions.DontRequireReceiver);
        }

        IEnumerator MeleeAttacking()
        {
            float _elapsed = 0f;
            while (true)
            {
                if (_elapsed >= 0.6f)
                    break;
                yield return null;
                _elapsed += Time.deltaTime;
            }
            isMeleeAttacking = false;
        }

        public void Reload()
        {
            if (reloading || ammo == weaponInfo.ammo)
                return;
            reloading = true;
            reloadFill.transform.parent.gameObject.SetActive(true);
            StartCoroutine("Reload_ammo");
        }

        IEnumerator Reload_ammo()
        {
            SendMessage("Play", "Reload_Start");
            StartCoroutine("FillingReloadBar");
            yield return new WaitForSeconds(weaponInfo.relodeSpeed);
            if (reloading)
                ammo = weaponInfo.ammo;
            SendMessage("Play", "Reload_End");
            reloadFillImage.fillAmount = 0;
            reloadFill.transform.parent.gameObject.SetActive(false);
            reloading = false;
        }

        IEnumerator FillingReloadBar()
        {
            reloadFillImage.fillAmount = 0f;
            while (reloading)
            {
                yield return null;
                reloadFillImage.fillAmount += 1 / weaponInfo.relodeSpeed * Time.deltaTime;
            }
        }

        //PlayerController의 DirectionNotifyer 의 Listener.
        public void RotateWeapon(CurrentInput _currentinput)
        {
            if (_currentinput == null)
                return;
            if (_currentinput.fixMove == KeyState.Down)
            {
                if (isFixed)
                {
                    Vector3 m_eulerA = transform.rotation.eulerAngles;
                    if (Mathf.Round(m_eulerA.y) != Mathf.Round(GetComponentInParent<CharacterPhysics>().transform.rotation.eulerAngles.y))
                    {
                        transform.rotation = Mathf.Round(m_eulerA.y) == 0 ? Quaternion.Euler(m_eulerA.x, 180f, m_eulerA.z) : Quaternion.Euler(m_eulerA.x, 0f, m_eulerA.z);
                    }
                }
                else
                {
                    preRotate = transform.rotation;
                }
                isFixed = !isFixed;
            }
            if (isFixed)
            {
                transform.rotation = preRotate;
                return;
            }

            Vector2 _toVector = _currentinput.ToVector();
            Quaternion dir = new Quaternion();

            if (GetComponentInParent<CharacterPhysics>().onWall)// || GetComponentInParent<CharacterPhysics>().isCrouching)
            {
                dir = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            }
            else
            {
                dir = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, _toVector.x == 0 ? _toVector.y * 90 : _toVector.y * 45);
            }
            transform.rotation = dir;

        }

        void SaveData()
        {
            this.isPicked = false;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            weaponInfo = originalWeaponInfo;
            PlayerProgress.instance.weaponObject = gameObject;
            PlayerProgress.instance.isWeaponSaved = true;
        }

        //앉을시 총 위치 아래로. 그리고 일어서면 다시 위로.
        /*
		protected void PlayerCrouch(bool isCrouching)
		{
			if(isCrouching)
			{
				transform.position -= new Vector3(0f, playerCollider.size.y/6, 0f);
			}
			else if(!isCrouching)
			{
				transform.position += new Vector3(0f, playerCollider.size.y/6, 0f);
			}
		}
        */
    }
}