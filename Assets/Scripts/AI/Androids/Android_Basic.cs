using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace roguelike
{
    public class Android_Basic : MonoBehaviour
    {
        protected CircleCollider2D platformCollider;
        protected BoxCollider2D hitboxCollider;
        protected Rigidbody2D m_rigidbody2D;
        protected SpriteRenderer m_spriteRenderer;
        protected Animator m_Animator;

        public AndroidInfo_Basic androidInfo_Basic;

        public bool findPlayer;
        public bool isBasicSmashing;
        public bool isDead;

        public GameObject playerObject;

        protected float distance;
        protected bool isCheckingBasicSmash;
        protected CameraShake cameraShake;

        public ParticleSystem explostionParticle;

        void Awake()
        {
            cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
            platformCollider = GetComponent<CircleCollider2D>();
            hitboxCollider = GetComponent<BoxCollider2D>();
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = GetComponent<Animator>();
            playerObject = GameObject.FindGameObjectWithTag("Player");

            GetComponent<StatusManagement>().getDamageListeners.AddListener(Damaged);

            Physics2D.IgnoreCollision(platformCollider, playerObject.GetComponent<CapsuleCollider2D>()); //Platform 만 겹치지 않음.
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer); // 몬스터끼리 겹치지 않음.
        }

        void Update()
        {
            if (isDead)
                return;
            distance = (playerObject.transform.position - transform.position).magnitude;
            if (distance <= androidInfo_Basic.findRadius && !Physics2D.Raycast(transform.position, (playerObject.transform.position - transform.position).normalized, distance, LayerMask.NameToLayer("Platform")))
            {
                findPlayer = true;
            }
            else
                findPlayer = false;
            ToDoThings();

        }

        public virtual void ToDoThings()
        {
            MoveToPlayer();
            CheckBasicSmash();
        }

        protected void MoveToPlayer()
        {
            Vector3 moveDir = new Vector2();
            if (findPlayer && !isBasicSmashing)
            {
                moveDir = (playerObject.transform.position - transform.position);
                moveDir.y = moveDir.z = 0f;
                moveDir.Normalize();
                transform.position += (moveDir * androidInfo_Basic.moveSpeed * Time.deltaTime);
                m_Animator.SetBool("Walk", true);
            }
            else
            {
                m_Animator.SetBool("Walk", false);
            }
            //Debug.Log(moveDir);

            transform.rotation = playerObject.transform.position.x - transform.position.x > 0f ?
                                Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180f, 0);

        }

        protected void CheckBasicSmash()
        {
            if (distance <= androidInfo_Basic.basicSmashRadius && !isCheckingBasicSmash)
            {
                isCheckingBasicSmash = true;
                StartCoroutine("CheckBasicSmashCoroutine");
            }
            else if (distance > androidInfo_Basic.basicSmashRadius)
            {
                isCheckingBasicSmash = false;
            }
        }

        IEnumerator CheckBasicSmashCoroutine()
        {
            float _time = Time.time;
            while (true)
            {
                if (!isCheckingBasicSmash)
                    break;
                if (Time.time - _time >= androidInfo_Basic.basicSmashPlayerCheckTime)
                {
                    m_Animator.SetBool("Alert", true);
                    m_Animator.SetTrigger("Smash");
                    StartCoroutine("Smash");
                    break;
                }
                yield return null;
            }
        }

        protected IEnumerator Smash()
        {
            isBasicSmashing = true;
            float _time = Time.time;
            while (true)
            {
                if (Time.time - _time >= androidInfo_Basic.basicSmashStartTime)
                    break;
                yield return null;
            }
            //Animator Smash trigger
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Alert", false);
            if (distance <= androidInfo_Basic.basicSmashRadius && (playerObject.transform.position.x - transform.position.x >= 0 ? 0 : 180f) == Mathf.Round(transform.rotation.eulerAngles.y))
            {
                StatusManagement _status = playerObject.GetComponent<StatusManagement>();
                if(_status != null)
                    _status.GetDamage(androidInfo_Basic.damage_basicSmash, gameObject);
            }
            isBasicSmashing = false;
            isCheckingBasicSmash = false;
        }

        public void Damaged(float damage)
        {
            StartCoroutine("Blink");
        }

        IEnumerator Blink()
        {
            float preTime = Time.time;
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            while (true)
            {
                foreach (SpriteRenderer s in spriteRenderers)
                {
                    s.color = Color.gray;
                }
                    yield return new WaitForSeconds(0.08f);
                foreach (SpriteRenderer s in spriteRenderers)
                {
                    s.color = new Color(180f / 255f, 180f / 255f, 180f / 255f);
                }
                yield return new WaitForSeconds(0.08f);
                
                if (Time.time - preTime >= 0.5f)
                    break;
            }
            m_spriteRenderer.color = Color.white;
        }

        void Death()
        {
            isDead = true;
            m_Animator.SetBool("DeadFlag", true);
            m_Animator.SetTrigger("Dead");
        }

        public void ExplosionPlay()
        {
            explostionParticle.Play();
        }
        public void ExplosionStop()
        {
            explostionParticle.Stop();
        }

    }
}