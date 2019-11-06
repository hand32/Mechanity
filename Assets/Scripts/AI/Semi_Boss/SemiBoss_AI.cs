using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace roguelike
{
    [System.Serializable]
    public class Pattern1Info
    {
        public float bullet_Damage;
        public float bullet_Speed;

        public int shootCount;
        public float oneShootDelay;
        public float sequenceShootDelay;
        public int sequenceCount;
    }
    [System.Serializable]
    public class Pattern2Info
    {
        public float bullet_Damage;
        public float bullet_Speed;

        public int fireRate;
        public float shootingTime;
    }
    public class SemiBoss_AI : MonoBehaviour
    {
        public List<ParticleSystem> dirtParticle;
        public ParticleSystem explosionParticle;
        public GameObject nextPortal;
        public GameObject itemBox;
        [Header("Basic Info")]
        public float moveSpeed = 3.5f;
        public GameObject cameraFollowObject;
        public GameObject playerBlock;
        public float phase1EndHP;

        [Header("Shooter Field")]
        public GameObject ShooterBulletPosition;
        public float shooterCoolTime = 5f;
        bool shooterCanShoot = true;
        bool isShooterShooting;
        int shooterCount = 0;
        public Pattern1Info pattern1Info = new Pattern1Info();
        public Pattern2Info pattern2Info = new Pattern2Info();

        [Header("Missile Field")]
        public GameObject MissileBulletPosition;
        public float missileDamage;
        public float missileHP;
        public float missileSpeed;
        public float missileShootDelay;

        [Header("Laser Field")]
        public GameObject LaserObject;
        public float laserDamage;
        public float laserAlarmTime;
        public float laserShootDelay;
        public RectTransform LaserBackGround;
        public SpriteRenderer Laser;

        Dictionary<Transform, float> rollThings = new Dictionary<Transform, float>();

        bool isDead;
        bool stop;
        bool rushAddSpeedBullet;
        bool raidStart = false;
        int phase = 0;

        bool isRushing;
        [Header("Rush Field")]
        public float rushAlarmTime;
        public float rushSpeed;
        public float rushCoolTime;

        BoxCollider2D MainCollider; // 맞으면 바로 죽는 것.
        Animator m_Animator;
        CameraShake cameraShake;
        GameObject playerObject;
        [Header("Bullet Prefabs")]
        public GameObject bulletPrefab;
        public GameObject missileBulletPrefab;

        public List<float> rushHPList;
        Queue<float> rushHPQueue = new Queue<float>();



        void OnEnable()
        {
            LaserObject.transform.localPosition -= new Vector3(6, 0, 0);
            explosionParticle.Stop();
            CharacterPhysics.DeathListener.AddListener(PlayerDeath);
            GetComponent<StatusManagement>().getDamageListeners.AddListener(HpCheck);
            Laser.color = new Color(1, 1, 1, 0);
            MainCollider = GetComponent<BoxCollider2D>();
            m_Animator = GetComponent<Animator>();
            cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
            playerObject = GameObject.FindGameObjectWithTag("Player");
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Contains("Pattern") || transform.GetChild(i).name.Contains("Shooter"))
                    rollThings.Add(transform.GetChild(i), transform.GetChild(i).localPosition.y);
            }

            foreach (float _f in rushHPList)
            {
                rushHPQueue.Enqueue(_f);
            }
        }

        void RaidStart()
        {
            Vector3 _pos = transform.position;
            _pos.x = playerObject.transform.position.x - 40;
            transform.position = _pos;
            SendMessage("Play", "Effect_1");
            Invoke("PlayEffect_2", 2);
            StartCoroutine("CameraMoving");
        }

        void PlayEffect_2()
        {
            SendMessage("Play", "Effect_2");
        }

        IEnumerator CameraMoving()
        {
            cameraShake.Shake(1.2f, 7f);
            Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera = Camera.main.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            Vector3 prePos = cameraFollowObject.transform.localPosition;
            cameraFollowObject.transform.position = playerObject.transform.position;
            cinemachineVirtualCamera.Follow = cameraFollowObject.transform;

            float elapsed = 0f;
            while (true)
            {
                yield return null;
                elapsed += Time.deltaTime;
                cameraFollowObject.transform.localPosition += (prePos - cameraFollowObject.transform.localPosition).normalized * 4f * Time.deltaTime;
                cinemachineVirtualCamera.m_Lens.OrthographicSize += 4f * Time.deltaTime;
                transform.position += new Vector3(5, 0, 0) * Time.deltaTime;
                if (cinemachineVirtualCamera.m_Lens.OrthographicSize >= 10f)
                {
                    cinemachineVirtualCamera.m_Lens.OrthographicSize = 10f;
                }
                if (elapsed >= 4.5)
                    break;
            }
            cameraFollowObject.transform.localPosition = prePos;
            cinemachineVirtualCamera.m_Lens.OrthographicSize = 10f;
            MoveStart();
        }

        void MoveStart()
        {
            PhaseUpdate();
            raidStart = true;
            StartCoroutine("BossUpdate");
            StartCoroutine("Roll");
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.tag == "Player" && !isDead)
            {
                StatusManagement _status = other.gameObject.GetComponent<StatusManagement>();
                if (_status != null)
                    _status.GetMaxDamageNoExceptions(gameObject);
            }
        }

        void PhaseUpdate()
        {
            phase += 1;
            if (phase == 1)
            {
                /*
                pattern1Info.bullet_Damage = 14;
                pattern1Info.bullet_Speed = 4;
                pattern1Info.shootCount = 4;
                pattern1Info.oneShootDelay = 0.15f;
                pattern1Info.sequenceShootDelay = 2;
                pattern1Info.sequenceCount = 4;

                pattern2Info.bullet_Damage = 14;
                pattern2Info.bullet_Speed = 3.5f;
                pattern2Info.fireRate = 5;
                pattern2Info.shootingTime = 10;
                */

                rushAlarmTime = 4;
                StartCoroutine("Phase1Check");
            }
            else if (phase == 2)
            {
                /*
                pattern1Info.bullet_Damage = 14;
                pattern1Info.bullet_Speed = 4;
                pattern1Info.shootCount = 4;
                pattern1Info.oneShootDelay = 0.15f;
                pattern1Info.sequenceShootDelay = 2;

                pattern2Info.bullet_Damage = 14;
                pattern2Info.bullet_Speed = 2;
                pattern2Info.fireRate = 5;
                */
                pattern1Info.sequenceCount = 5;
                pattern2Info.shootingTime = 13;

                rushAlarmTime = 3;
                StartCoroutine("MoveForward", LaserObject);
                StartCoroutine("MoveBack", MissileBulletPosition.transform.parent.parent.gameObject);
                StartCoroutine("Phase2Check");
            }
        }

        IEnumerator MoveForward(GameObject go)
        {
            float _posX = 6;
            while (true)
            {
                go.transform.localPosition += new Vector3(1, 0, 0) * Time.deltaTime;
                _posX -= 1 * Time.deltaTime;
                if (_posX <= 0)
                    break;
                yield return null;
            }
        }
        IEnumerator MoveBack(GameObject go)
        {
            float _posX = 6;
            while (true)
            {
                go.transform.localPosition -= new Vector3(1, 0, 0) * Time.deltaTime;
                _posX -= 1 * Time.deltaTime;
                if (_posX <= 0)
                    break;
                yield return null;
            }
        }

        void Update()
        {
            if (nextPortal.transform.position.x - 0.3f <= playerBlock.transform.position.x)
            {
                Death();
            }
        }

        IEnumerator BossUpdate()
        {
            float elapsed = 0;
            StartCoroutine("Roll");
            while (!isDead)
            {
                Move();
                ShooterCheck();
                MovePlayerBlock();
                yield return null;
            }
        }

        void Move()
        {
            if (stop)
            {
                if (!isRushing)
                {
                    foreach (ParticleSystem _particle in dirtParticle)
                    {
                        _particle.Stop();
                    }
                }
                return;
            }

            foreach (ParticleSystem _particle in dirtParticle)
            {
                _particle.Play();
            }

            //움직이는 애니메이션.
            transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
        }

        void ShooterCheck()
        {
            if (isRushing || !shooterCanShoot || isShooterShooting)
                return;
            if (shooterCount >= 2)
            {
                Debug.Log("CoolTime");
                StartCoroutine("ShooterCooltime");
                return;
            }
            //pattern 1
            if (Random.Range(0, 2) == 0)
            {
                Debug.Log("Pattern1Shoot");
                isShooterShooting = true;
                StartCoroutine("ShooterPattern1");
                shooterCount += 1;
            }
            //pattern 2
            else
            {
                Debug.Log("Pattern2Shoot");
                isShooterShooting = true;
                StartCoroutine("ShooterPattern2");
                shooterCount += 1;
            }
        }

        IEnumerator ShooterPattern1()
        {
            int sequenceCount = pattern1Info.sequenceCount;
            while (true)
            {
                if (isDead)
                    break;
                int currentBulletCounts = pattern1Info.shootCount;
                int currentSequenceCounts = pattern1Info.sequenceCount;
                while (currentBulletCounts > 0)
                {
                    if (isDead)
                        break;
                    //m_Animator.SetTrigger("Shoot");
                    yield return new WaitForSeconds(pattern1Info.oneShootDelay);

                    Vector2 toPlayerVector2 = (playerObject.transform.position - ShooterBulletPosition.transform.position + new Vector3(10, 0, 0)).normalized;

                    float _zdegrees = Vector2.Angle(playerObject.transform.position.x >= transform.position.x ? Vector2.right : Vector2.left, toPlayerVector2);
                    if (playerObject.transform.position.y < transform.position.y)
                        _zdegrees = -_zdegrees;
                    float _ydegrees = playerObject.transform.position.x >= transform.position.x ? 0f : 180f;
                    Quaternion _dir = Quaternion.Euler(0, _ydegrees, _zdegrees);

                    var bullet = Instantiate(bulletPrefab, (Vector3)ShooterBulletPosition.transform.position, _dir);//.GetComponent<Rigidbody2D>();
                    var hitCheck = bullet.gameObject.GetComponent<HitCheck>();
                    hitCheck.shooter = gameObject;
                    hitCheck.damage = pattern1Info.bullet_Damage;
                    hitCheck.targetTag.Add("Player");
                    hitCheck.targetTag.Add("Platform");
                    Vector2 _bulletVelocity = toPlayerVector2.normalized * pattern1Info.bullet_Speed;
                    _bulletVelocity.x += (stop ? 0 : moveSpeed);
                    if (rushAddSpeedBullet)
                        _bulletVelocity.x += rushSpeed;

                    hitCheck.velocity = _bulletVelocity;

                    //bullet.velocity = bulletVelocity;
                    hitCheck.StartCoroutine("ShootingRadiusCheck", 10f);

                    cameraShake.Shake(0.09f, 0.5f);

                    currentBulletCounts -= 1;
                }
                sequenceCount--;
                if (sequenceCount <= 0)
                    break;
                yield return new WaitForSeconds(pattern1Info.sequenceShootDelay);
            }
            yield return new WaitForSeconds(1.5f);
            isShooterShooting = false;
        }

        IEnumerator ShooterPattern2()
        {
            float fireRate = pattern2Info.fireRate;
            float shootingTime = pattern2Info.shootingTime;
            float elapsed = 0f;
            float shootCheckTime = 0f;
            while (elapsed <= shootingTime)
            {
                if (isDead)
                    break;
                //m_Animator.SetTrigger("Shoot");
                yield return null;
                elapsed += Time.deltaTime;
                shootCheckTime += Time.deltaTime;
                if (shootCheckTime >= 1f / fireRate)
                {
                    float randomSeta = Random.Range(-60f, 60f) * Mathf.Deg2Rad;
                    Vector2 toPlayerVector2 = new Vector2(Mathf.Cos(randomSeta), Mathf.Sin(randomSeta));

                    float _zdegrees = Vector2.Angle(Vector2.right, toPlayerVector2);
                    if (toPlayerVector2.y < 0)
                        _zdegrees = -_zdegrees;

                    Quaternion _dir = Quaternion.Euler(0, 0, _zdegrees);

                    var bullet = Instantiate(bulletPrefab, (Vector3)ShooterBulletPosition.transform.position, _dir);//.GetComponent<Rigidbody2D>();
                    var hitCheck = bullet.gameObject.GetComponent<HitCheck>();
                    hitCheck.shooter = gameObject;
                    hitCheck.damage = pattern2Info.bullet_Damage;
                    hitCheck.targetTag.Add("Player");
                    hitCheck.targetTag.Add("Platform");
                    Vector2 _bulletVelocity = toPlayerVector2.normalized * pattern2Info.bullet_Speed;
                    _bulletVelocity.x += (stop ? 0 : moveSpeed);
                    if (rushAddSpeedBullet)
                        _bulletVelocity.x += rushSpeed;

                    hitCheck.velocity = _bulletVelocity;

                    //bullet.velocity = bulletVelocity;
                    hitCheck.StartCoroutine("ShootingRadiusCheck", 10f);

                    cameraShake.Shake(0.04f, 0.2f);

                    shootCheckTime = 0f;
                }
            }
            yield return new WaitForSeconds(1.5f);
            isShooterShooting = false;
        }

        IEnumerator ShooterCooltime()
        {
            shooterCanShoot = false;
            yield return new WaitForSeconds(shooterCoolTime);
            shooterCanShoot = true;
            shooterCount = 0;
        }


        IEnumerator Phase1Check()
        {
            float _elapsed = 0;
            while (phase == 1 && !isDead)
            {
                yield return null;
                _elapsed += Time.deltaTime;
                if (_elapsed >= missileShootDelay && !isRushing)
                {
                    StartCoroutine("MissileShootingMotion");
                    //m_Animator.SetTrigger("Shoot");

                    Quaternion _dir = Quaternion.Euler(0, 0, 0);

                    //Debug.Log("Shoot! the Rocket! " + gameObject.name);
                    var bullet = Instantiate(missileBulletPrefab, (Vector3)MissileBulletPosition.transform.position, _dir);

                    bullet.GetComponent<Bullet_FollowRocket>().GetInfo(missileSpeed + (stop ? (rushAddSpeedBullet ? rushSpeed : 0) : moveSpeed));
                    bullet.GetComponent<StatusManagement>().m_currentHealth = missileHP;
                    var hitCheck = bullet.GetComponent<HitCheck>();
                    hitCheck.shooter = gameObject;
                    hitCheck.damage = missileDamage;
                    hitCheck.targetTag.Add("Player");
                    hitCheck.targetTag.Add("Platform");

                    hitCheck.StartCoroutine("ShootingRadiusCheck", 10f);

                    cameraShake.Shake(0.5f, 1.2f);
                    _elapsed = 0;
                }
            }
        }
        IEnumerator MissileShootingMotion()
        {
            float _posX = 1;
            Vector3 reset = MissileBulletPosition.transform.parent.localPosition;
            while (true)
            {
                MissileBulletPosition.transform.parent.localPosition -= new Vector3(4, 0, 0) * Time.deltaTime;
                _posX -= 4 * Time.deltaTime;
                if (_posX <= 0)
                    break;
                yield return null;
            }

            _posX = 1;
            while (true)
            {
                MissileBulletPosition.transform.parent.localPosition += new Vector3(1, 0, 0) * Time.deltaTime;
                _posX -= 1 * Time.deltaTime;
                if (_posX <= 0)
                    break;
                yield return null;
            }
            MissileBulletPosition.transform.parent.localPosition = reset;
        }

        IEnumerator Phase2Check()
        {
            LaserBackGround.localScale = new Vector3(1, 0, 1);
            Laser.color = new Color(1, 1, 1, 0);
            float _elapsed = laserShootDelay - 6;
            while (phase == 2 && !isDead)
            {
                yield return null;
                _elapsed += Time.deltaTime;
                if (_elapsed >= laserShootDelay && !isRushing)
                {
                    LaserBackGround.localScale = new Vector3(1, 0, 1);
                    float alarmElapsed = 0;
                    SendMessage("Play", "Laser_Charge_1");
                    bool played1 = false;
                    bool played2 = false;

                    while (alarmElapsed <= laserAlarmTime)
                    {
                        if (alarmElapsed / laserAlarmTime >= 0.2f && !played1)
                        {
                            played1 = true;
                            SendMessage("Play", "Laser_Charge_2");
                        }
                        if (alarmElapsed / laserAlarmTime >= 0.4f && !played2)
                        {
                            played2 = true;
                            SendMessage("Play", "Laser_Charge_3");
                        }
                        yield return null;
                        alarmElapsed += Time.deltaTime;
                        LaserBackGround.localScale += new Vector3(0, 3.3f / laserAlarmTime, 0) * Time.deltaTime;
                        if (LaserBackGround.localScale.y > 3.3f)
                        {
                            LaserBackGround.localScale = new Vector3(1, 3.3f, 1);
                            break;
                        }
                    }
                    LaserBackGround.localScale = new Vector3(1, 0, 1);

                    SendMessage("Play", "Laser_Shoot_1");
                    SendMessage("Play", "Laser_Shoot_2");
                    cameraShake.Shake(1f, 1.4f);
                    Laser.color = new Color(1, 1, 1, 1);
                    Collider2D[] _overlaps = Physics2D.OverlapBoxAll(LaserObject.transform.position, new Vector2(60, 1.66f * 2), 0f);
                    foreach (Collider2D _c in _overlaps)
                    {
                        if (_c.tag == "Player")
                        {
                            StatusManagement _status = _c.gameObject.GetComponent<StatusManagement>();
                            if (_status != null)
                                _status.GetDamage(laserDamage, gameObject);
                        }
                    }

                    float _color = 1;
                    while (true)
                    {
                        yield return null;
                        _color -= Time.deltaTime / 2;
                        if (_color <= 0)
                        {
                            Laser.color = new Color(1, 1, 1, 0);
                            break;
                        }
                        Laser.color = new Color(1, 1, 1, _color);
                    }
                    //m_Animator.SetTrigger("Shoot");

                    _elapsed = 0;
                }
            }
        }

        IEnumerator Roll()
        {
            float seta = Mathf.Deg2Rad * 90f;
            float rateTime = 9.6f;

            Dictionary<Transform, bool> positiveDic = new Dictionary<Transform, bool>();
            foreach (Transform _t in rollThings.Keys)
            {
                positiveDic.Add(_t, _t.localPosition.y >= 0 ? true : false);
            }

            while (!isDead)
            {
                if (!isRushing)
                {
                    foreach (Transform _t in rollThings.Keys)
                    {
                        Vector3 position = _t.localPosition;
                        float radius = rollThings[_t];

                        position.y = Mathf.Sin(seta) * radius;
                        if (position.y - _t.localPosition.y > 0 && !positiveDic[_t])
                        {
                            positiveDic[_t] = true;
                            if (_t.name == "Shooter")
                            {
                                foreach (SpriteRenderer _s in _t.GetComponentsInChildren<SpriteRenderer>())
                                {
                                    _s.sortingOrder = 4;
                                }
                            }
                            _t.GetComponent<SpriteRenderer>().sortingOrder = 5;
                        }
                        else if (position.y - _t.localPosition.y <= 0 && positiveDic[_t])
                        {
                            positiveDic[_t] = false;
                            if (_t.name == "Shooter")
                            {
                                foreach (SpriteRenderer _s in _t.GetComponentsInChildren<SpriteRenderer>())
                                {
                                    _s.sortingOrder = 6;
                                }
                            }
                            _t.GetComponent<SpriteRenderer>().sortingOrder = 7;
                        }
                        _t.localPosition = position;
                        _t.localScale = new Vector3(1, 1 - Mathf.Cos(seta) * 0.05f * (rollThings[_t] > 0 ? 1 : -1), 1);
                        seta += Mathf.Deg2Rad * 360f / rateTime * Time.deltaTime;
                    }
                }
                yield return null;
            }
        }

        void HpCheck(float _damage)
        {
            StartCoroutine("Blink");

            float currentHP = GetComponent<StatusManagement>().m_currentHealth;

            if (phase == 1 && currentHP <= phase1EndHP)
            {
                BroadcastMessage("ChangeSprite");
                cameraShake.Shake(1.5f, 3f);
                SendMessage("Play", "Explosion_0");
                explosionParticle.Play();
                Invoke("ExplosionStop", 2.5f);
                PhaseUpdate();
            }

            //rush Check
            if (rushHPQueue.Count > 0 && currentHP <= rushHPList[rushHPList.Count - rushHPQueue.Count])
            {
                rushHPQueue.Dequeue();
                if (!isRushing)
                {
                    isRushing = true;
                    stop = true;
                    StartCoroutine("Rush");
                }
            }
        }

        IEnumerator Rush()
        {

            //경고 시작
            foreach (ParticleSystem _particle in dirtParticle)
            {
                _particle.Stop();
            }
            m_Animator.SetBool("Stop", true);
            yield return new WaitForSeconds(rushAlarmTime);

            //돌진
            foreach (ParticleSystem _particle in dirtParticle)
            {
                _particle.Play();
            }
            rushAddSpeedBullet = true;
            m_Animator.SetBool("Stop", false);
            m_Animator.SetBool("Rush", true);
            float rushLength = 0;
            while (rushLength <= 13f)
            {
                yield return null;
                float _Xmove = rushSpeed * Time.deltaTime;
                transform.position += new Vector3(_Xmove, 0, 0);
                rushLength += _Xmove;
            }
            rushAddSpeedBullet = false;
            m_Animator.SetBool("Stop", true);
            m_Animator.SetBool("Rush", false);
            foreach (ParticleSystem _particle in dirtParticle)
            {
                _particle.Stop();
            }
            yield return new WaitForSeconds(rushCoolTime);
            m_Animator.SetBool("Stop", false);
            isRushing = false;
            stop = false;
        }

        void ExplosionStop()
        {
            explosionParticle.Stop();
        }

        IEnumerator Blink()
        {
            float preTime = Time.time;
            List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
            spriteRenderers.Add(LaserObject.GetComponent<SpriteRenderer>());
            spriteRenderers.Add(MissileBulletPosition.transform.parent.parent.GetComponent<SpriteRenderer>());
            spriteRenderers.Add(MissileBulletPosition.transform.parent.GetComponent<SpriteRenderer>());
            foreach (SpriteRenderer _s in ShooterBulletPosition.transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderers.Add(_s);
            }
            spriteRenderers.Add(ShooterBulletPosition.transform.parent.GetComponent<SpriteRenderer>());

            spriteRenderers.Add(GetComponent<SpriteRenderer>());
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
            foreach (SpriteRenderer s in spriteRenderers)
            {
                s.color = new Color(1, 1, 1);
            }
        }

        void Death()
        {
            //StartCoroutine("DeathCameraMove");
            if (!isDead)
            {
                BroadcastMessage("ChangeSprite");
                SendMessage("Stop", "Effect_1");
                SendMessage("Stop", "Effect_2");
                foreach (ParticleSystem _particle in dirtParticle)
                {
                    _particle.Stop();
                }
                List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
                spriteRenderers.Add(LaserObject.GetComponent<SpriteRenderer>());
                spriteRenderers.Add(MissileBulletPosition.transform.parent.parent.GetComponent<SpriteRenderer>());
                foreach (SpriteRenderer _s in ShooterBulletPosition.transform.parent.GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteRenderers.Add(_s);
                }
                spriteRenderers.Add(ShooterBulletPosition.transform.parent.GetComponent<SpriteRenderer>());
                spriteRenderers.Add(GetComponent<SpriteRenderer>());

                foreach (SpriteRenderer s in spriteRenderers)
                {
                    s.color = new Color(180f / 255f, 180f / 280f, 180f / 255f);
                }

                SendMessage("Play", "Explosion_1");
                explosionParticle.Play();
                cameraShake.Shake(1.5f, 5f);
                Invoke("ExplosionStop", 9f);
                Instantiate(itemBox, cameraFollowObject.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ObjectItem>().ThrowinDrop();
                Instantiate(itemBox, cameraFollowObject.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ObjectItem>().ThrowinDrop();
            }
            isDead = true;
            m_Animator.SetTrigger("Dead");
            playerBlock.GetComponent<BoxCollider2D>().enabled = false;
            nextPortal.transform.position = playerBlock.transform.position + new Vector3(1, 0f, 0);

        }

        IEnumerator DeathCameraMove()
        {
            Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera = Camera.main.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();


            float elapsed = 0f;
            while (true)
            {
                yield return null;
                elapsed += Time.deltaTime;
                cameraFollowObject.transform.position += (playerObject.transform.position - cameraFollowObject.transform.position).normalized * 5f * Time.deltaTime;
                cinemachineVirtualCamera.m_Lens.OrthographicSize -= 3f * Time.deltaTime;
                if (cinemachineVirtualCamera.m_Lens.OrthographicSize <= 5.6f)
                {
                    cinemachineVirtualCamera.m_Lens.OrthographicSize = 5.6f;
                }
                if (elapsed >= 3f)
                    break;
            }
            cameraFollowObject.transform.position = playerObject.transform.position;
            cinemachineVirtualCamera.m_Lens.OrthographicSize = 5.6f;
            cinemachineVirtualCamera.Follow = playerObject.transform;


        }

        void PlayerDeath()
        {
            stop = true;
        }

        void MovePlayerBlock()
        {
            Vector3 _pos = playerBlock.transform.position;
            _pos.x = Camera.main.transform.position.x + 18.3f;
            playerBlock.transform.position = _pos;
        }
    }
}