using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour {

	public Transform cloudParent;

	List<Transform> clouds = new List<Transform>();

	public float moveSpeed;

	public GameObject left;
	public GameObject right;

	int randomDirection;
	
	void OnEnable()
	{
		for(int i = 0; i < cloudParent.transform.childCount; i++)
			clouds.Add(cloudParent.transform.GetChild(i));
		randomDirection = Random.Range(0, 2);
		if(randomDirection == 0)
			randomDirection = -1;
	}

	void Update ()
	{
		foreach(Transform _trans in clouds)
		{
			_trans.position += new Vector3(randomDirection * moveSpeed, 0, 0) * Time.deltaTime;
			if(_trans.position.x <= left.transform.position.x)
			{
				Vector3 _pos = _trans.position;
				_pos.x = right.transform.position.x;
				_trans.position = _pos;
			}
			else if(_trans.position.x >= right.transform.position.x)
			{
				Vector3 _pos = _trans.position;
				_pos.x = left.transform.position.x;
				_trans.position = _pos;
			}
		}
	}
}
