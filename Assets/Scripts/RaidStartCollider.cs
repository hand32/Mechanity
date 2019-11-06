using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidStartCollider : MonoBehaviour {

	public GameObject Boss;
	bool start = false;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player" && !start)
		{
			start = true;
			Boss.SendMessage("RaidStart");
		}
	}
}
