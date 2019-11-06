using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace roguelike
{
    public class Monster_Spider_AI : MonoBehaviour
    {


        CircleCollider2D platformCollider;
        BoxCollider2D hitboxCollider;
        Rigidbody2D m_rigidbody2D;
        //SpriteRenderer m_spriteRenderer;
        Animator m_Animator;
        SpriteRenderer m_spriteRenderer;


        public CharacterCollisionState2D collisionState;

        public float colliderRadius;
        public float layLength;
        public LayerMask platformLayerMask;
        public Collider2D playerCollider;
        public Transform m_BulletPosition;
        CameraShake cameraShake;


        public SpiderInfo spiderInfo;
        public bool findPlayer;
        public bool isCornering;
        public bool isShooting;
        public bool isHooking;
        public bool isHooked;
        bool stop;
        bool isDead;
        public bool isFalling;

        Vector2 bulletPosition;
        float preShootTime;
        int currentBulletCounts;

        Vector2 toPlayerVector2;
        DirandRot currentDR = new DirandRot(null, null);

        public ParticleSystem explostionParticle;


        public void PassNextNode(DirandRot _currentDR)
        {
            currentDR = _currentDR;
        }

        void OnEnable()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            GetComponent<StatusManagement>().getDamageListeners.AddListener(Damaged);
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            //m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = GetComponent<Animator>();
            collisionState = new CharacterCollisionState2D();
            preShootTime = Time.time;
            cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
            /*
            foreach(BoxCollider2D c in GetComponents<BoxCollider2D>())
            {
                if(c.isTrigger)
                    hitboxCollider = c;
                else
                    platformCollider = c;
            }//Box Collider 두개로 중복시 이걸로 구분.
            */
            hitboxCollider = GetComponent<BoxCollider2D>();
            platformCollider = GetComponent<CircleCollider2D>();
            colliderRadius = platformCollider.radius;
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();

            Physics2D.IgnoreCollision(platformCollider, playerCollider); //Platform 만 겹치지 않음.
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer); // 몬스터끼리 겹치지 않음.

            //SpriteRenderer.spirte.bounds.size 를 사용하거나 Sprite.rect 를 사용하거나. 골라.
            //Vector2 S = m_spriteRenderer.sprite.bounds.size;
            //hitboxCollider.size = S;
            CollisionCheck();
            if (!collisionState.HasParallelCollision())
            {
                m_rigidbody2D.gravityScale = 2f;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                isFalling = true;
            }
        }

        void Update()
        {
            if (isDead)
                return;

            CollisionCheck();
            if (isFalling)
            {
                if (collisionState.down.HasCollision())
                {
                    isFalling = false;
                    m_rigidbody2D.gravityScale = 0f;
                }
                else
                    return;
            }
            if (!collisionState.HasParallelCollision())
            {
                m_rigidbody2D.gravityScale = 2f;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                isFalling = true;
                return;
            }

            //collisionState.Reset();
            //CollisionCheck();

            toPlayerVector2 = (playerCollider.transform.position - transform.position).normalized; //to player position. nomalized.
            FindPlayer();

            Move();
            Shoot();
        }

        void FindPlayer()
        {
            float _playerDistance = (playerCollider.transform.position - transform.position).magnitude;
            bulletPosition = m_BulletPosition.position;

            if (findPlayer && _playerDistance <= spiderInfo.findRadius * 2)
            {
                findPlayer = true;
            }
            else if (_playerDistance <= spiderInfo.findRadius && !Physics2D.Raycast(bulletPosition, toPlayerVector2, m_rigidbody2D.Distance(playerCollider).distance, platformLayerMask))
            {
                findPlayer = true;
            }
            else
            {
                findPlayer = false;
            }
            //Debug.DrawRay(bulletPosition, toPlayerVector2 * m_rigidbody2D.Distance(playerCollider).distance);
            if (stop)
            {
                findPlayer = false;
            }

            if (!findPlayer)
            {
                m_Animator.SetBool("Walk", false);
            }


        }

        public void CollisionCheck()
        {
            //raycast direction check.
            Vector2 rayOrigin = (Vector2)transform.position;// + platformCollider.offset;
            float _offset = colliderRadius * 0.2f;

            //Right Raycast
            rayOrigin.x += colliderRadius;
            rayOrigin.y += colliderRadius;
            rayOrigin.y -= _offset;
            if (Physics2D.Raycast(rayOrigin, Vector2.right, layLength, platformLayerMask))
            {
                collisionState.right.one = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.right * layLength);

            rayOrigin.y += _offset * 2;
            rayOrigin.y -= colliderRadius * 2;
            if (Physics2D.Raycast(rayOrigin, Vector2.right, layLength, platformLayerMask))
            {
                collisionState.right.two = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.right * layLength);

            //Down Raycast
            rayOrigin.x -= _offset;
            rayOrigin.y -= _offset;
            if (Physics2D.Raycast(rayOrigin, Vector2.down, layLength, platformLayerMask))
            {
                collisionState.down.two = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.down * layLength);

            rayOrigin.x += _offset * 2;
            rayOrigin.x -= colliderRadius * 2;
            if (Physics2D.Raycast(rayOrigin, Vector2.down, layLength, platformLayerMask))
            {
                collisionState.down.one = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.down * layLength);

            //Left Raycast
            rayOrigin.x -= _offset;
            rayOrigin.y += _offset;
            if (Physics2D.Raycast(rayOrigin, Vector2.left, layLength, platformLayerMask))
            {
                collisionState.left.two = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.left * layLength);

            rayOrigin.y -= _offset * 2;
            rayOrigin.y += colliderRadius * 2;
            if (Physics2D.Raycast(rayOrigin, Vector2.left, layLength, platformLayerMask))
            {
                collisionState.left.one = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.left * layLength);

            //Up Raycast
            rayOrigin.x += _offset;
            rayOrigin.y += _offset;
            if (Physics2D.Raycast(rayOrigin, Vector2.up, layLength, platformLayerMask))
            {
                collisionState.up.one = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.up * layLength);

            rayOrigin.x -= _offset * 2;
            rayOrigin.x += colliderRadius * 2;
            if (Physics2D.Raycast(rayOrigin, Vector2.up, layLength, platformLayerMask))
            {
                collisionState.up.two = true;
            }
            Debug.DrawRay(rayOrigin, Vector2.up * layLength);

        }

        void Shoot()
        {
            if (!findPlayer)
                return;

            if (isHooking || isShooting || isCornering)
                return;


            if (spiderInfo.reloadTime <= Time.time - preShootTime && !Physics2D.Raycast(bulletPosition, toPlayerVector2, m_rigidbody2D.Distance(playerCollider).distance, platformLayerMask))
            {
                isShooting = true;
                StartCoroutine("CreatBullet", toPlayerVector2);
            }
            else
                isShooting = false;
        }

        IEnumerator CreatBullet()
        {
            while (true)
            {
                if(isDead)
                    break;
                m_Animator.SetBool("Walk", false);
                if (currentBulletCounts <= 0)
                {
                    currentBulletCounts = spiderInfo.m_bulletCounts;
                    isShooting = false;

                    break;
                }
                m_Animator.SetTrigger("Shoot");
                yield return new WaitForSeconds(spiderInfo.shootingDelay);

                float _zdegrees = Vector2.Angle(playerCollider.transform.position.x >= transform.position.x ? Vector2.right : Vector2.left, toPlayerVector2);
                if (playerCollider.transform.position.y < transform.position.y)
                    _zdegrees = -_zdegrees;
                float _ydegrees = playerCollider.transform.position.x >= transform.position.x ? 0f : 180f;
                Quaternion _dir = Quaternion.Euler(0, _ydegrees, _zdegrees);

                var bullet = Instantiate(spiderInfo.BulletPrefab, (Vector3)bulletPosition, _dir);//.GetComponent<Rigidbody2D>();
                var hitCheck = bullet.gameObject.GetComponent<HitCheck>();
                hitCheck.shooter = gameObject;
                hitCheck.damage = spiderInfo.damage_Bullet;
                hitCheck.targetTag.Add("Player");
                hitCheck.targetTag.Add("Platform");
                Vector2 _bulletVelocity = toPlayerVector2.normalized * spiderInfo.bulletSpeed;

                hitCheck.velocity = _bulletVelocity;

                //bullet.velocity = bulletVelocity;
                hitCheck.StartCoroutine("ShootingRadiusCheck", 10f / _bulletVelocity.magnitude);

                cameraShake.Shake(0.08f, 0.5f);

                currentBulletCounts -= 1;
                preShootTime = Time.time;
            }
        }

        void Move()
        {
            if (isCornering)
                return;


            if (!findPlayer || isHooking || isShooting)
            {
                return;
            }


            //is this have to Cornering?
            if (currentDR.Equals(null))
                currentDR = new DirandRot(new Vector2(), Quaternion.Euler(0, 0, 0));

            if (currentDR.dir != null && (Mathf.Round(transform.right.x) != currentDR.dir.x && Mathf.Round(transform.right.y) != currentDR.dir.y))
            {
                if (!isCornering)
                    Cornering(currentDR);
            }
            else
            {
                if (currentDR.dir != Vector2.zero && currentDR.dir != null)
                    m_Animator.SetBool("Walk", true);
                else
                {
                    m_Animator.SetBool("Walk", false);
                    return;
                }
                transform.position += (Vector3)currentDR.dir * spiderInfo.moveSpeed * Time.deltaTime;
                transform.rotation = currentDR.rot;
            }


        }
        void Cornering(DirandRot dr)
        {
            isCornering = true;
            m_rigidbody2D.velocity = Vector2.zero;
            if (Mathf.Round(Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90f).eulerAngles.z)
                == Mathf.Round(dr.rot.eulerAngles.z))
            {
                StartCoroutine("InCornering", dr);
            }
            else if (Mathf.Round(Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 90f).eulerAngles.z)
                    == Mathf.Round(dr.rot.eulerAngles.z))
            {
                StartCoroutine("OutCornering", dr);
            }
        }
        IEnumerator OutCornering(DirandRot dr)
        {
            //Debug.Log("OutCornering");
            float rotate = 0f;
            float checkRotate = 0f;
            float turnRadius = 0f;

            if (Mathf.Round(transform.up.y) == 0f)
            {
                turnRadius = 0.5f - Mathf.Abs(Mathf.FloorToInt(transform.position.x) + 0.5f - transform.position.x);
                rotate = spiderInfo.turnSpeed
                            / turnRadius / Mathf.PI * 180f;
            }
            else if (Mathf.Round(transform.up.x) == 0f)
            {
                turnRadius = 0.5f - Mathf.Abs(Mathf.FloorToInt(transform.position.y) + 0.5f - transform.position.y);
                rotate = spiderInfo.turnSpeed
                            / turnRadius / Mathf.PI * 180f;
            }
            while (true)
            {
                if (!isShooting)
                {
                    m_Animator.SetBool("Walk", true);
                    if (checkRotate <= -90f)
                        break;
                    //Debug.Log("InCornering: " + -rotate);
                    transform.Rotate(0, 0, -rotate * Time.deltaTime);
                    checkRotate += -rotate * Time.deltaTime;
                    transform.position += transform.right.normalized * Mathf.Sin((rotate * Mathf.PI / 180f * Time.deltaTime) / 2) * turnRadius * 2;
                }
                yield return null;
            }
            transform.rotation = dr.rot;
            isCornering = false;
            yield break;

        }

        IEnumerator InCornering(DirandRot dr)
        {
            //Debug.Log("InCornering");
            float rotate = 0f;
            float checkRotate = 0f;
            float turnRadius = 0f;

            if (Mathf.Round(transform.up.y) == 0f)
            {
                turnRadius = 0.5f + Mathf.Abs(Mathf.FloorToInt(transform.position.x) + 0.5f - transform.position.x);
                rotate = spiderInfo.turnSpeed
                            / turnRadius / Mathf.PI * 180f;
            }
            else if (Mathf.Round(transform.up.x) == 0f)
            {
                turnRadius = 0.5f + Mathf.Abs(Mathf.FloorToInt(transform.position.y) + 0.5f - transform.position.y);
                rotate = spiderInfo.turnSpeed
                            / turnRadius / Mathf.PI * 180f;
            }

            while (true)
            {
                if (!isShooting)
                {
                    m_Animator.SetBool("Walk", true);
                    if (checkRotate >= 90f)
                        break;
                    //Debug.Log("InCornering: " + rotate);
                    transform.Rotate(0, 0, rotate * Time.deltaTime);
                    checkRotate += rotate * Time.deltaTime;
                    transform.position += transform.right.normalized * Mathf.Sin((rotate * Mathf.PI / 180f * Time.deltaTime) / 2) * turnRadius * 2;
                }
                yield return null;
            }
            transform.rotation = dr.rot;
            isCornering = false;
            yield break;
        }

        //Charge Damaging
        void OnTriggerStay2D(Collider2D other)
        {
            if (isDead)
                return;

            if (other.tag == "Player")
            {
                if (other.gameObject.GetComponent<StatusManagement>().isDamageable && !isFalling)
                    Stop();
            }
        }

        public void Stop()
        {
            StartCoroutine("MoveStopCoroutine");
        }

        IEnumerator MoveStopCoroutine()
        {
            m_Animator.SetBool("Walk", false);
            stop = true;
            yield return new WaitForSeconds(1f);
            stop = false;
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
            if (isDead)
                return;
            findPlayer = false;
            isDead = true;

            cameraShake.Shake(0.2f, 2f);
            m_Animator.SetBool("DeathFlag", true);
            m_Animator.SetTrigger("Death");
        }

        void Fall()
        {
            m_rigidbody2D.gravityScale = 2f;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            isFalling = true;
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