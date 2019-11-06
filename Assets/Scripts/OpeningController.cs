using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace roguelike{
public class OpeningController : MonoBehaviour {

	VideoPlayer videoPlayer;
	Canvas UICanvas;
	GameObject playerObject;

	// Use this for initialization
	void Start ()
	{
		videoPlayer = GetComponent<VideoPlayer>();
		UICanvas = GameObject.FindWithTag("UI").GetComponent<Canvas>();
		playerObject = GameObject.FindWithTag("Player");
		UICanvas.enabled = false;
		playerObject.GetComponent<PlayerController>().GamePause();
		Cursor.visible = false;
	}
	
	void LateUpdate()
	{
		if(videoPlayer.isPrepared && !videoPlayer.isPlaying || Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.visible = true;
			videoPlayer.enabled = false;
			playerObject.GetComponent<PlayerController>().GameStart();
			UICanvas.enabled = true;
		}
	}
}
}