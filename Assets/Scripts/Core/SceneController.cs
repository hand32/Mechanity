using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace roguelike
{
    public class SceneController : MonoBehaviour
    {
        public int combatMapCount;
        public int caveMapCount;
        public bool[] combatMapUsed;
        public bool[] caveMapUsed;
        public static bool isSceneLoaded;
        enum MapType
        {
            Start,
            ComBat,
            Shelter,
            ComBat_Cave,
            BeforeSemiBoss,
            SemiBoss,
            BeforeFinalBoss,
            FinalBoss,
        }


        int sceneIndex = 1;
        string sceneName;
        UnityAction function;
        Animator m_Animator;
        bool gameOver;

        int comBatSceneCnt = 0;
        int caveSceneCnt = 0;

        public GameObject ExitPanel;

        bool isShelterTime;
        bool isSemiBossTime;
        bool isFinalBossTime;


        private static string NameFromIndex(int BuildIndex)
        {
            string path = SceneManager.GetSceneByBuildIndex(BuildIndex).name;
            int slash = path.LastIndexOf('/');
            string name = path.Substring(slash + 1);
            int dot = name.LastIndexOf('.');
            return name.Substring(0, dot);
        }

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
            ExitPanel.SetActive(false);

            RestartLevel();
        }

        void Start()
        {
            combatMapUsed = new bool[combatMapCount];
            caveMapUsed = new bool[caveMapCount];
        }


        public void EndFadeOut()
        {
            if(function != null)
            {
                StopAllCoroutines();
                function();
            }
        }

        public void NextLevel()
        {
            function = LoadNextLevel;
            m_Animator.SetTrigger("FadeOut");
        }
        public void RestartLevel()
        {
            caveSceneCnt = comBatSceneCnt = 0;
            isFinalBossTime = false;
            isSemiBossTime = false;
            isShelterTime = false;
            ExitPanel.SetActive(false);
            gameOver = false;
            function = LoadRestartLevel;
            m_Animator.SetTrigger("FadeOut");
        }
        
        void LoadNextLevel()
        {
            ExitPanel.SetActive(false);
            ItemProgress.instance.SceneSaveItem();
            GameObject[] _gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject _gameObject in _gameObjects)
            {
                if (_gameObject.tag == "Player")
                {
                    _gameObject.BroadcastMessage("SaveData", SendMessageOptions.DontRequireReceiver);
                }
            }

            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            int loopcount = 0;
            bool getSceneIndex = false;
            while (!getSceneIndex)
            {
                if(isShelterTime)
                    break;
                loopcount++;
                if (loopcount > 10000)
                {
                    sceneIndex = 0;
                    break;
                }

                if (comBatSceneCnt < 3)
                {
                    sceneIndex = Random.Range(0, combatMapCount);
                    if (!combatMapUsed[sceneIndex])
                    {
                        getSceneIndex = true;
                        combatMapUsed[sceneIndex] = true;
                    }
                }
                else
                {
                    sceneIndex = Random.Range(0, caveMapCount);
                    if (!caveMapUsed[sceneIndex])
                    {
                        getSceneIndex = true;
                        caveMapUsed[sceneIndex] = true;
                    }
                }
            }

            if (isShelterTime)
            {
                if (isSemiBossTime)
                    StartCoroutine("AsyncChecker", "Map_" + MapType.BeforeSemiBoss);
                else if (isFinalBossTime)
                    StartCoroutine("AsyncChecker", "Map_" + MapType.BeforeFinalBoss);
                else if(comBatSceneCnt < 3)
                    StartCoroutine("AsyncChecker", "Map_" + MapType.Shelter + "0");
                else
                    StartCoroutine("AsyncChecker", "Map_" + MapType.Shelter + "1");
                isShelterTime = false;
            }
            else if (isSemiBossTime)
            {
                isShelterTime = true;
                isSemiBossTime = false;
                StartCoroutine("AsyncChecker", "Map_" + MapType.SemiBoss);
            }
            else if (isFinalBossTime)
            {
                isFinalBossTime = false;
                StartCoroutine("AsyncChecker", "Map_" + MapType.FinalBoss);
            }
            else if (comBatSceneCnt < 3)
            {
                comBatSceneCnt++;
                if (comBatSceneCnt == 3)
                    isSemiBossTime = true;
                isShelterTime = true;
                StartCoroutine("AsyncChecker", "Map_" + MapType.ComBat + sceneIndex);
            }
            else if (caveSceneCnt < 3)
            {
                caveSceneCnt ++;
                if (caveSceneCnt == 3)
                    isFinalBossTime = true;
                isShelterTime = true;
                StartCoroutine("AsyncChecker", "Map_" + MapType.ComBat_Cave + sceneIndex);
            }
        }

        void LoadRestartLevel()
        {
            PlayerProgress.instance.Reset();
            ItemProgress.instance.Reset();
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            sceneIndex = -1;
            StartCoroutine("AsyncChecker", "Map_" + MapType.Start.ToString());
        }

        IEnumerator AsyncChecker(string _sceneName)
        {
            isSceneLoaded = false;
            function = null;
            AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            while (!operation.isDone)
            {
                yield return null;
            }
            InventoryController.instance.StartCoroutine("AwakeState");

            isSceneLoaded = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
            /*
            GameObject[] _gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject _gameObject in _gameObjects)
            {
                if (_gameObject.tag == "Player")
                {
                    _gameObject.GetComponent<CharacterPhysics>().AddDeathListener(GameOver);
                }
            }
            */
            CharacterPhysics.DeathListener.AddListener(GameOver);
            if (function != LoadRestartLevel)
                ItemProgress.instance.SceneLoadItem();
        }

        void GameOver()
        {
            gameOver = true;
            // 플레이어 죽음 이벤트 실행
            RestartLevel();
        }

        void Update()
        {
            
            /*
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextLevel();
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                RestartLevel();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                if (caveSceneCnt > 0)
                    caveSceneCnt--;
                else if (comBatSceneCnt > 0)
                    comBatSceneCnt--;
                NextLevel();
            }
            */
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ExitPanel.active)
                    ExitPanel.SetActive(false);
                else
                    ExitPanel.SetActive(true);
            }

        }

        public void GameExit()
        {
            StartCoroutine("ExitingGame");
        }

        IEnumerator ExitingGame()
        {
            m_Animator.SetTrigger("FadeOut");   
            yield return new WaitForSeconds(1.5f);
            
            Application.Quit();
        }
    }
}