using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace roguelike
{
    public class PortalScript : MonoBehaviour
    {
		GameObject sceneManager;

		bool portalIn;

		void Start()
		{
			Scene s = SceneManager.GetSceneByName("SceneManager");
			GameObject[] _gameObjects = s.GetRootGameObjects();
			if(_gameObjects == null)
				Debug.Log("Error: Can't find Gameobjects in SceneManager!");
			foreach(GameObject _gameObject in _gameObjects)
			{
				Debug.Log(_gameObject.name);
				if(_gameObject.tag == "SceneManager")
				{
					sceneManager = _gameObject;
					break;
				}
			}
			if(sceneManager == null)
			{
				Debug.Log("Error: Can't Find SceneManager!");
			}
		}

        void OnTriggerEnter2D(Collider2D other)
        {
			if(portalIn)
				return;
			
            if (other.tag == "Player")
            {
				portalIn = true;
				CharacterPhysics.stop = true;
				other.gameObject.BroadcastMessage("SaveData", SendMessageOptions.DontRequireReceiver);
				other.GetComponent<Rigidbody2D>().gravityScale = 0;
				sceneManager.SendMessage("NextLevel");
            }
        }
    }
}