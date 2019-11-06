using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike
{
    public class FinalBoss_AI : MonoBehaviour
    {
        Animator m_Animator;
        GameObject playerObject;
        int phase = 1;
        bool isDoingPattern;
        CameraShake cameraShake;
        [Header("Basic Thing")]
        public bool isDead;
        public int phase1EndHP;
        public ParticleSystem explosionParticle;
        public BoxCollider2D playerBlock;
        [Header("Drone Field")]
        public Transform DronePoolPos;
        public Transform D1LeftUp;
        public Transform D1LeftDown;
        public Transform D1Right;
        public Transform D2LeftDown;
        public Transform D2RightDown;
        public Transform D2RightUp;
        public Transform D3UP;
        public Transform D3Middle;
        public Transform D3Down;
        public GameObject P1Drone1;
        public GameObject P1Drone2;
        public GameObject P1Drone3;
        public GameObject P2Drone1;
        public GameObject P2Drone2;
        public GameObject P2Drone3;
        public float droneSpeed = 6;
        public float bombDelay = 0.7f;
        public float droneLaserDamage = 16;
        public float droneBulletDamage = 14;
        public float droneBulletSpeed = 4;

        [Header("Missile Field")]
        public GameObject missilePrefab;
        public Transform rocketLeftPoolPosition;
        public Transform rocketRightPoolPosition;
        public float missileDamage = 22;
        public float missileSpeed = 10;
        public float missileDistance = 4;
        public float missileDelay = 1;
        int missileCount = 4;

        [Header("RocketShooting Filed")]
        public GameObject rocketShooter;
        public float rocketFirerate = 5;
        public float rocketShootingTime = 4;
        public float rocketSpeed = 1.5f;
        public float rocketDamage = 16;


        public GameObject cameraFollowObject;
        Cinemachine.CinemachineVirtualCamera VCam;

        void OnEnable()
        {
            GetComponent<StatusManagement>().getDamageListeners.AddListener(HpCheck);
            m_Animator = GetComponent<Animator>();
            cameraShake = Camera.main.GetComponent<CameraShake>();
            VCam = Camera.main.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            playerObject = GameObject.FindWithTag("Player");
        }

        void RaidStart()
        {
            playerBlock.isTrigger = false;
            VCam.Follow = cameraFollowObject.transform;

            StartCoroutine("CameraMoving");
        }

        IEnumerator CameraMoving()
        {
            cameraShake.Shake(1f, 1f);
            Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera = Camera.main.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            Vector3 prePos = cameraFollowObject.transform.position;
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.z = prePos.z;
            cameraFollowObject.transform.position = cameraPos;
            cinemachineVirtualCamera.Follow = cameraFollowObject.transform;


            Cinemachine.CinemachineFramingTransposer body = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
            float xDamping = body.m_XDamping;
            float yDamping = body.m_YDamping;
            float DZWidth = body.m_DeadZoneWidth;
            float DZHeight = body.m_DeadZoneHeight;

            body.m_XDamping = 0;
            body.m_YDamping = 0;
            body.m_DeadZoneWidth = body.m_DeadZoneHeight = 0;

            float elapsed = 0f;
            while (true)
            {
                yield return null;
                elapsed += Time.deltaTime;
                cameraFollowObject.transform.position += (prePos - cameraFollowObject.transform.position).normalized * 2f * Time.deltaTime;
                if (elapsed >= 4f)
                    break;
            }
            
            cameraFollowObject.transform.position = prePos;
            StartCoroutine("BossUpdate");
        }

        public void PhaseUpdate()
        {
            phase++;
            if (phase == 2)
            {
                bombDelay = 0.4f;
                droneLaserDamage = 22;
                droneBulletDamage = 20;
                droneBulletSpeed = 5;
                missileDistance = 1;
                missileDelay = 0.3f;
                missileDamage = 32;
                missileCount = 8;
                rocketFirerate = 7;
                rocketShootingTime = 4;
                rocketDamage = 19;
            }
        }

        IEnumerator BossUpdate()
        {
            while (!isDead)
            {
                int select = Random.Range(1, 7);
                if (phase == 1)
                {
                    if (select == 1)
                    {
                        MissileFallCheck();
                    }
                    else if (select == 2)
                    {
                        MissileShoot();
                    }
                    else if (select == 3)
                    {

                    }
                    else
                    {
                        Phase1DronePatternCheck();
                    }
                }
                else if (phase == 2)
                {
                    if (select == 1)
                    {
                        MissileFallCheck();
                    }
                    else if (select == 2)
                    {
                        MissileShoot();
                    }
                    else if (select == 3)
                    {

                    }
                    else
                    {
                        Phase2DronePatternCheck();
                    }
                }
                yield return null;
            }
        }


        void MissileFallCheck()
        {
            if (isDoingPattern)
                return;

            isDoingPattern = true;
            StartCoroutine("MissileFall");
        }

        IEnumerator MissileFall()
        {
            List<GameObject> missiles = new List<GameObject>();

            int hori = Random.Range(0, 2) == 0 ? -1 : 1;
            Vector3 spawnPos = hori == 1 ? rocketLeftPoolPosition.position : rocketRightPoolPosition.position;

            for (int i = 0; i < missileCount; i++)
            {
                GameObject missile = Instantiate(missilePrefab, spawnPos + new Vector3((missileDistance + 1) * i * hori, 0, 0), new Quaternion());
                missiles.Add(missile);
                missile.GetComponent<FinalBoss_Missile>().GetInfo(missileDamage, missileSpeed);
                yield return new WaitForSeconds(missileDelay);
            }

            while (true)
            {
                bool next = true;
                for (int i = 0; i < missiles.Count; i++)
                {
                    if (missiles[0])
                        next = false;
                }
                if (next)
                {
                    isDoingPattern = false;
                    break;
                }
                yield return null;
            }
        }

        void MissileShoot()
        {
            if (isDoingPattern)
                return;
            isDoingPattern = true;
            rocketShooter.GetComponent<FinalBoss_RocketLauncher>().GetInfo(rocketFirerate, rocketShootingTime, rocketSpeed, rocketDamage);
            rocketShooter.GetComponent<FinalBoss_RocketLauncher>().Shoot();
            StartCoroutine("MissileEndCheck");
        }

        IEnumerator MissileEndCheck()
        {
            FinalBoss_RocketLauncher rocketLauncherScript = rocketShooter.GetComponent<FinalBoss_RocketLauncher>();
            while (true)
            {
                if (rocketLauncherScript.endShooting)
                    break;
                yield return null;
            }
            isDoingPattern = false;
        }


        void Phase1DronePatternCheck()
        {
            if (isDoingPattern)
                return;

            isDoingPattern = true;
            switch (Random.Range(1, 4))
            {
                case 1:
                    StartCoroutine("P1Drone1Creat");
                    break;

                case 2:
                    StartCoroutine("P1Drone2Creat");
                    break;

                case 3:
                    StartCoroutine("Drone3Creat");
                    break;

                default:
                    isDoingPattern = false;
                    Phase1DronePatternCheck();
                    return;
            }
        }

        IEnumerator P1Drone1Creat()
        {
            List<GameObject> drones = new List<GameObject>();
            Vector3 playerPos = playerObject.transform.position;
            switch (Random.Range(1, 4))
            {
                case 1:
                    for (int i = -1; i < 2; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        var drone = Instantiate(P1Drone1, DronePoolPos.position, new Quaternion());
                        drone.GetComponent<FinalBoss_Drone1>().GetInfo(
                            new Vector3(playerPos.x + 2 * i, D1LeftUp.position.y, 0), droneSpeed, droneLaserDamage, bombDelay);
                        drones.Add(drone);
                    }
                    break;
                case 2:
                    for (int i = -1; i < 2; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        var drone = Instantiate(P1Drone1, DronePoolPos.position, new Quaternion());
                        drone.GetComponent<FinalBoss_Drone1>().GetInfo(
                            new Vector3(playerPos.x + 2 * i, D1LeftDown.position.y, 0), droneSpeed, droneLaserDamage, bombDelay);
                        drones.Add(drone);
                    }
                    break;
                case 3:
                    for (int i = -1; i < 2; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        var drone = Instantiate(P1Drone1, DronePoolPos.position, new Quaternion());
                        drone.GetComponent<FinalBoss_Drone1>().GetInfo(
                            new Vector3(D1Right.position.x, playerPos.y + 2 * i, 0), droneSpeed, droneLaserDamage, bombDelay);
                        drones.Add(drone);
                    }
                    break;
                default:
                    break;
            }
            while (true)
            {
                bool next = true;
                for (int i = 0; i < drones.Count; i++)
                {
                    if (drones[0])
                        next = false;
                }
                if (next)
                {
                    isDoingPattern = false;
                    break;
                }
                yield return null;
            }

        }

        IEnumerator P1Drone2Creat()
        {
            List<GameObject> drones = new List<GameObject>();

            var drone = Instantiate(P1Drone2, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone2>();
            drone.GetInfo(D2LeftDown.position, droneSpeed, droneLaserDamage, bombDelay);
            drones.Add(drone.gameObject);
            drone = Instantiate(P1Drone2, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone2>();
            drones.Add(drone.gameObject);
            drone.GetInfo(D2RightDown.position, droneSpeed, droneLaserDamage, bombDelay);
            drone = Instantiate(P1Drone2, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone2>();
            drones.Add(drone.gameObject);
            drone.GetInfo(D2RightUp.position, droneSpeed, droneLaserDamage, bombDelay);

            while (true)
            {
                bool next = true;
                for (int i = 0; i < drones.Count; i++)
                {
                    if (drones[0])
                        next = false;
                }
                if (next)
                {
                    isDoingPattern = false;
                    break;
                }
                yield return null;
            }
        }

        IEnumerator Drone3Creat()
        {
            List<GameObject> drones = new List<GameObject>();

            int except = Random.Range(1, 4);
            GameObject Drone3 = P1Drone3;

            if (phase == 2)
            {
                Drone3 = P2Drone3;
            }

            FinalBoss_Drone3 drone = new FinalBoss_Drone3();

            if (except != 1)
            {
                drone = Instantiate(Drone3, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone3>();
                drone.GetInfo(D3UP.position, droneSpeed, droneBulletDamage, bombDelay);
                drones.Add(drone.gameObject);
            }
            if (except != 2)
            {
                drone = Instantiate(Drone3, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone3>();
                drone.GetInfo(D3Middle.position, droneSpeed, droneBulletDamage, bombDelay);
                drones.Add(drone.gameObject);
            }
            if (except != 3)
            {
                drone = Instantiate(Drone3, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone3>();
                drone.GetInfo(D3Down.position, droneSpeed, droneBulletDamage, bombDelay);
                drones.Add(drone.gameObject);
            }


            while (true)
            {
                bool next = true;
                for (int i = 0; i < drones.Count; i++)
                {
                    if (drones[0])
                        next = false;
                }
                if (next)
                {
                    isDoingPattern = false;
                    break;
                }
                yield return null;
            }
        }

        void Phase2DronePatternCheck()
        {
            if (isDoingPattern)
                return;

            isDoingPattern = true;
            switch (Random.Range(1, 4))
            {
                case 1:
                    StartCoroutine("P2Drone1Creat");
                    break;

                case 2:
                    StartCoroutine("P2Drone2Creat");
                    break;

                case 3:
                    StartCoroutine("Drone3Creat");
                    break;

                default:
                    isDoingPattern = false;
                    Phase2DronePatternCheck();
                    return;
            }
        }

        IEnumerator P2Drone1Creat()
        {
            List<GameObject> drones = new List<GameObject>();
            Vector3 playerPos = playerObject.transform.position;
            switch (Random.Range(1, 4))
            {
                case 1:
                    for (int i = -1; i < 2; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        var drone = Instantiate(P2Drone1, DronePoolPos.position, new Quaternion());
                        drone.GetComponent<FinalBoss_Drone1_P2>().GetInfo(
                            new Vector3(playerPos.x + 2 * i, D1LeftUp.position.y, 0), droneSpeed, droneLaserDamage, bombDelay);
                        drones.Add(drone);
                    }
                    break;
                case 2:
                    for (int i = -1; i < 2; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        var drone = Instantiate(P2Drone1, DronePoolPos.position, new Quaternion());
                        drone.GetComponent<FinalBoss_Drone1_P2>().GetInfo(
                            new Vector3(playerPos.x + 2 * i, D1LeftDown.position.y, 0), droneSpeed, droneLaserDamage, bombDelay);
                        drones.Add(drone);
                    }
                    break;
                case 3:
                    for (int i = -1; i < 2; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        var drone = Instantiate(P2Drone1, DronePoolPos.position, new Quaternion());
                        drone.GetComponent<FinalBoss_Drone1_P2>().GetInfo(
                            new Vector3(D1Right.position.x, playerPos.y + 2 * i, 0), droneSpeed, droneLaserDamage, bombDelay);
                        drones.Add(drone);
                    }
                    break;
                default:
                    break;
            }
            while (true)
            {
                bool next = true;
                for (int i = 0; i < drones.Count; i++)
                {
                    if (drones[0])
                        next = false;
                }
                if (next)
                {
                    isDoingPattern = false;
                    break;
                }
                yield return null;
            }

        }

        IEnumerator P2Drone2Creat()
        {
            List<GameObject> drones = new List<GameObject>();

            var drone = Instantiate(P2Drone2, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone2_P2>();
            drone.GetInfo(D2LeftDown.position, droneSpeed, droneLaserDamage, bombDelay);
            drones.Add(drone.gameObject);
            drone = Instantiate(P2Drone2, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone2_P2>();
            drones.Add(drone.gameObject);
            drone.GetInfo(D2RightDown.position, droneSpeed, droneLaserDamage, bombDelay);
            drone = Instantiate(P2Drone2, DronePoolPos.position, new Quaternion()).GetComponent<FinalBoss_Drone2_P2>();
            drones.Add(drone.gameObject);
            drone.GetInfo(D2RightUp.position, droneSpeed, droneLaserDamage, bombDelay);

            while (true)
            {
                bool next = true;
                for (int i = 0; i < drones.Count; i++)
                {
                    if (drones[0])
                        next = false;
                }
                if (next)
                {
                    isDoingPattern = false;
                    break;
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
                cameraShake.Shake(1.5f, 3f);
                SendMessage("Play", "Explosion_0");
                explosionParticle.Play();
                Invoke("ExplosionStop", 2.5f);
                PhaseUpdate();
            }
        }

        IEnumerator Blink()
        {
            float preTime = Time.time;
            List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
            foreach (SpriteRenderer _s in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderers.Add(_s);
            }

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

        void ExplosionStop()
        {
            explosionParticle.Stop();
        }

        void Death()
        {
            //StartCoroutine("DeathCameraMove");
            if (!isDead)
            {
                List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
                foreach (SpriteRenderer _s in GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteRenderers.Add(_s);
                }

                foreach (SpriteRenderer s in spriteRenderers)
                {
                    s.color = new Color(180f / 255f, 180f / 280f, 180f / 255f);
                }

                SendMessage("Play", "Explosion_1");
                explosionParticle.Play();
                cameraShake.Shake(1.5f, 9f);
                Invoke("ExplosionStop", 9f);
            }
            isDead = true;
            m_Animator.SetTrigger("Dead");
        }

    }

}