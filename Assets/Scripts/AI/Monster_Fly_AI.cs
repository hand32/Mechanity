using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike
{
    public class Monster_Fly_AI : MonoBehaviour
    {
        public LayerMask platformLayerMask;
        public CapsuleCollider2D playerCollider;
        public BoxCollider2D m_BoxCollider2D;
        public SpriteRenderer childSpriteRenderer;

        SpriteRenderer m_spriteRenderer;
        Animator m_Animator;

        public bool findPlayer;
        public bool isBombing;
        public FlyInfo flyInfo;

        float distance;
        float findTime;

        public new GameObject camera;

        public ParticleSystem explostionParticle;


        void OnEnable()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            GetComponent<StatusManagement>().getDamageListeners.AddListener(Damaged);
            m_BoxCollider2D = GetComponent<BoxCollider2D>();
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
            m_Animator = GetComponent<Animator>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        void FindPlayer()
        {
            Vector2 toPlayerVector2 = (playerCollider.transform.position - transform.position).normalized;
            float _playerDistance = ((Vector2)playerCollider.transform.position - (Vector2)transform.position).magnitude;

            if (_playerDistance <= flyInfo.findRadius && !Physics2D.Raycast(transform.position, toPlayerVector2, flyInfo.findRadius, platformLayerMask))
            {
                findPlayer = true;
            }
            else
            {
                findPlayer = false;
            }

            if (findPlayer)
            {
				m_Animator.SetTrigger("Detect");
                findTime = Time.time;
                distance = ((Vector2)playerCollider.transform.position - (Vector2)transform.position).magnitude;
                if(playerCollider.transform.position.x - transform.position.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                StartCoroutine("Rush", playerCollider.transform.position);
            }
        }

        // Update is called once per frame
        void Update()
        {
			if(!findPlayer)
			    FindPlayer();
        }

        IEnumerator Rush(Vector3 _playerPosition)
        {
			while(true)
			{
				if (Time.time - findTime >= flyInfo.rushDelay && !isBombing)
				{
					//Debug.Log("Rushing");
					transform.position += (Vector3)((Vector2)(_playerPosition - transform.position)).normalized * flyInfo.rushSpeed * Time.deltaTime;
					distance -= flyInfo.rushSpeed * Time.deltaTime;
					if (distance <= 0)
					{
						Bomb();
						break;
					}
				}
				yield return null;
			}
        }


        void Bomb()
        {
			//Debug.Log("Bomb!");
            isBombing = true;
            m_Animator.SetTrigger("Explosion");
            camera.GetComponent<CameraShake>().Shake(0.2f, 2f);
        }

        public void SetCollider()
        {
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1.5f);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                if (isBombing)
                {
                    StatusManagement _status = other.gameObject.GetComponent<StatusManagement>();
                    if (_status != null)
                        _status.GetDamage(flyInfo.damage_Bomb, gameObject);
                }
                else if(findPlayer)
                    Bomb();
            }
        }

        public void Damaged(float damage)
        {
            StartCoroutine("Blink");
        }

        IEnumerator Blink()
        {
            float preTime = Time.time;
            while (true)
            {
                m_spriteRenderer.color = Color.gray;
                yield return new WaitForSeconds(0.08f);
                m_spriteRenderer.color = new Color(180f / 255f, 180f / 255f, 180f / 255f);
                yield return new WaitForSeconds(0.08f);
                if (Time.time - preTime >= 0.5f)
                    break;
            }
            m_spriteRenderer.color = Color.white;
        }

        void Death()
        {
            m_Animator.SetTrigger("Death");
            SendMessage("Play", "Dead");
            Destroy(gameObject, 0.15f);
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